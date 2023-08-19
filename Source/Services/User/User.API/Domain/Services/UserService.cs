using Common;
using Common.Gprc;
using System.Text.Json;
using System.Text;
using User.API.Domain.Entities;
using User.API.Domain.Exceptions;
using User.API.Domain.Specifications;
using User.API.Infrastructure.Data;
using RabbitMQ.Client;
using Constants = Common.Constants;
using Common.Saga.User;
using Common.Saga.User.Dto;

namespace User.API.Domain.Services;

/// <summary>
/// User Service used to manage complex domain logic.
/// </summary>
public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly Repository<UserEntity> _userRepository;
    private readonly Repository<Role> _roleRepository;
    private readonly IConnection _eventQueueConnection;
    private readonly IModel _eventQueueChannel;
    [ActivatorUtilitiesConstructor]
    public UserService(Repository<UserEntity> userRepository, Repository<Role> roleRepository, IConfiguration configuration, ILogger<UserService> logger)
    {
        _logger = logger;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        var split = configuration[Constants.EventQueueAddress]?.Split(":")!;
        var factory = new ConnectionFactory
        {
            HostName = split[0],
            Port = int.Parse(split[1])
        };
        _eventQueueConnection = factory.CreateConnection();
        _eventQueueChannel = _eventQueueConnection.CreateModel();
    }

    public UserService(Repository<UserEntity> userRepository, Repository<Role> roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _eventQueueConnection = null!;
        _eventQueueChannel = null!;
        _logger = null!;
    }

    ~UserService()
    {
        Dispose(false);
    }

    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        _eventQueueChannel.Dispose();
        _eventQueueConnection.Dispose();
    }

    public async Task<UserEntity> Create(string firstName, string lastName, string email, string password)
    {
        ValidationExceptionBuilder exceptionBuilder = new();
        Guid id = Guid.NewGuid();
        Role? role = await _roleRepository.FirstOrDefaultAsync(new RoleSpecification("User"));
        if (role == null)
        {
            throw new RoleNotFoundException("Name", "User");
        }
        PasswordValidator.ValidatePassword(password, exceptionBuilder);
        CheckEmailPresence(email, exceptionBuilder);
        UserEntity user = new()
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Password = PasswordValidator.HashPassword(password)
        };
        user.Roles.Add(role);
        user.ValidateData(exceptionBuilder);
        if (exceptionBuilder.HasErrors())
        {
            throw exceptionBuilder.Build();
        }
        await _userRepository.AddAsync(user);
        return user;
    }

    public async Task<UserEntity> Update(Guid userId, string firstName, string lastName, string? email, string? password)
    {
        ValidationExceptionBuilder exceptionBuilder = new();
        UserEntity? user = await _userRepository.FirstOrDefaultAsync(new UserSpecification(userId));
        if (user == null)
        {
            throw new UserNotFoundException(userId);
        }
        if (password != null)
        {
            PasswordValidator.ValidatePassword(password, exceptionBuilder);
        }
        if (email != null && !email.Equals(user.Email))
        {
            CheckEmailPresence(email, exceptionBuilder);
        }

        user.FirstName = firstName;
        user.LastName = lastName;
        user.Email = email ?? user.Email;
        user.Password = password != null ? PasswordValidator.HashPassword(password) : user.Password;
        user.ValidateData(exceptionBuilder);
        if (exceptionBuilder.HasErrors())
        {
            throw exceptionBuilder.Build();
        }
        await _userRepository.UpdateAsync(user);
        return user;
    }

    public void StartCreateUserSaga(UserData userData, Guid userId)
    {
        var command = new CreateUserCommand
        {
            Type = CreateUserCommandType.CreateUserStart,
            UserData = userData,
            UserId = userId
        };
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(command));
        _eventQueueChannel.BasicPublish(exchange: nameof(CreateUserCommand), routingKey: string.Empty, basicProperties: null, body: body);
        _logger.LogInformation("Published command to start the create user saga");
    }

    public void StartDeleteUserSaga(Guid userToDeleteId, Guid loggedUserId)
    {
        var command = new DeleteUserCommand
        {
            Type = DeleteUserCommandType.DeleteUserStart,
            UserId = loggedUserId,
            UserToDeleteId = userToDeleteId
        };
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(command));
        _eventQueueChannel.BasicPublish(exchange: nameof(DeleteUserCommand), routingKey: string.Empty, basicProperties: null, body: body);
        _logger.LogInformation("Published command to start the delete user saga");
    }

    public void StartUpdateUserSaga(UserData data, Guid userId)
    {
        var command = new UpdateUserCommand
        {
            Type = UpdateUserCommandType.UpdateUserStart,
            UserData = data,
            UserId = userId
        };
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(command));
        _eventQueueChannel.BasicPublish(exchange: nameof(UpdateUserCommand), routingKey: string.Empty, basicProperties: null, body: body);
        _logger.LogInformation("Published command to start the update user saga");
    }

    /// <summary>
    /// Method used for checking if email already exists in user repository.
    /// </summary>
    /// <param name="email">Email of the user.</param>
    /// <param name="validationExceptionBuilder">Validation exception builder that will contain error messages.</param>
    private void CheckEmailPresence(string email, ValidationExceptionBuilder validationExceptionBuilder)
    {
        if (_userRepository.FirstOrDefaultAsync(new EmailSpecification(email)).Result != null)
        {
            validationExceptionBuilder.AddError(nameof(email), "User with the given email already exists");
        }
    }
}
