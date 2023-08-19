using AutoMapper;
using Common.Gprc;
using Common.Gprc.Exceptions;
using Common.Notification;
using Common.Saga;
using Common.Saga.User;
using Common.Saga.User.Dto;
using Grpc.Core;
using User.API.Domain.Entities;
using User.API.Domain.Specifications;
using User.API.Infrastructure.Data;
using Constants = Common.Constants;

namespace User.API.Domain.Services;

public class CreateUserSagaHandler
    : SagaHandler<CreateUserSagaHandler, CreateUserCommand, CreateUserReply, CreateUserCommandType, CreateUserReplyType>
{
    private readonly INotificationClient _notificationClient;
    private readonly IMapper _mapper;
    public CreateUserSagaHandler(IConfiguration configuration, ILogger<CreateUserSagaHandler> logger, IServiceProvider serviceProvider)
        : base(configuration, logger, serviceProvider)
    {
        _notificationClient = _serviceProvider.GetRequiredService<INotificationClient>();
        _mapper = _serviceProvider.GetRequiredService<IMapper>();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _notificationClient.Dispose();
        }
        base.Dispose(true);
    }

    protected override async Task<CreateUserReply> HandleCommand(CreateUserCommand command)
    {
        return command.Type switch
        {
            CreateUserCommandType.CreateUserStart => await CreateUser(command),
            CreateUserCommandType.RollbackUser => await RollbackUser(command),
            CreateUserCommandType.ConcludeSuccessfully => await ConcludeSuccessfully(command),
            CreateUserCommandType.ConcludeWithFailure => await ConcludeWithFailure(command),
            _ => new CreateUserReply(command)
        };
    }

    private async Task<CreateUserReply> CreateUser(CreateUserCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        using var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var reply = new CreateUserReply(command);
        var data = command.UserData;
        try
        {
            if (data == null)
            {
                throw new InvalidArgumentException(nameof(command.UserData), "null", "User Data Object");
            }
            var user = await userService.Create(data.FirstName, data.LastName, data.Email!, data.Password!);
            reply.UserData = _mapper.Map<UserData>(user);
            reply.Type = CreateUserReplyType.CreateUserSuccess;
        }
        catch (RpcException e)
        {
            reply.Type = CreateUserReplyType.CreateUserFailure;
            var rpcError = RpcError.ParseRpcErrorMessage(e.Message);
            await _notificationClient.SendNotification(rpcError.Detail, command.UserId ?? Guid.Empty, Constants.CreateUserFailureAction);
        }
        catch (Exception e)
        {
            reply.Type = CreateUserReplyType.CreateUserFailure;
            await _notificationClient.SendNotification(e.Message, command.UserId ?? Guid.Empty, Constants.CreateUserFailureAction);
        }
        return reply;
    }

    private async Task<CreateUserReply> RollbackUser(CreateUserCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<UserContext>();
        var userRepository = new Repository<UserEntity>(dbContext);
        var reply = new CreateUserReply(command)
        {
            Type = CreateUserReplyType.UserRolledBack
        };
        var data = command.UserData;
        if (data == null)
        {
            return reply;
        }
        var user = await userRepository.FirstOrDefaultAsync(new UserSpecification(data.Id));
        if (user != null)
        {
            await userRepository.DeleteAsync(user);
        }
        return reply;
    }

    private async Task<CreateUserReply> ConcludeSuccessfully(CreateUserCommand command)
    {
        await _notificationClient.SendNotification("User created successfully.", command.UserId ?? Guid.Empty, Constants.CreateUserSuccessAction);
        return new CreateUserReply(command);
    }

    private Task<CreateUserReply> ConcludeWithFailure(CreateUserCommand command)
    {
        return Task.FromResult(new CreateUserReply(command));
    }
}
