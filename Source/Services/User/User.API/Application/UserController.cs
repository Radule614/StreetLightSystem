using AutoMapper;
using Common;
using Common.Auth;
using Common.Gprc.Exceptions;
using Common.Saga.User.Dto;
using Grpc.Core;
using User.API.Domain.Entities;
using User.API.Domain.Exceptions;
using User.API.Domain.Services;
using User.API.Domain.Specifications;
using User.API.Infrastructure.Data;
using UserProto;

namespace User.API.Application;

/// <summary>
/// UserController class used for specifying gRPC endpoints for user micro service
/// </summary>
public class UserController : UserGrpc.UserGrpcBase
{
    private readonly Repository<UserEntity> _userRepository;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public UserController(Repository<UserEntity> userRepository, IMapper mapper, IUserService userService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _userService = userService;
    }

    /// <summary>
    /// Rpc endpoint for retrieving all user entities
    /// </summary>
    /// <param name="request">Empty request</param>
    /// <param name="context"></param>
    /// <returns>All user entities</returns>
    [Auth(permissions: "ReadUsers")]
    public override async Task<Users> GetAll(Empty request, ServerCallContext context)
    {
        ICollection<UserEntity> userCollection = await _userRepository.ListAsync(new UserSpecification());
        var response = new Users();
        response.Data.AddRange(_mapper.Map<ICollection<UserDto>>(userCollection));
        return response;
    }

    /// <summary>
    /// Rpc endpoint for retrieving an user entity by Id
    /// </summary>
    /// <param name="request">Request that contains user entity's id</param>
    /// <param name="context"></param>
    /// <returns>User entity that matches the given id</returns>
    [Auth(permissions: "ReadUsers")]
    public override async Task<UserDto> GetById(ID request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var userId))
        {
            throw new InvalidArgumentException(nameof(userId), request.Id, Constants.GuidFormat);
        }
        var user = await _userRepository.FirstOrDefaultAsync(new UserSpecification(userId));
        return user == null ? throw new UserNotFoundException(userId) : _mapper.Map<UserDto>(user);
    }

    /// <summary>
    /// Rpc endpoint for deleting a user entity by Id
    /// </summary>
    /// <param name="request">Request that contains user entity's id</param>
    /// <param name="context"></param>
    /// <returns>Empty response</returns>
    [Auth(permissions: "ModifyUsers")]
    public override Task<Empty> Delete(ID request, ServerCallContext context)
    {
        var loggedUserIdString = context.UserState["UserId"] as string;
        if (!Guid.TryParse(loggedUserIdString, out var loggedUserId))
        {
            throw new InvalidArgumentException(nameof(loggedUserId), loggedUserIdString ?? "", Constants.GuidFormat);
        }
        if (!Guid.TryParse(request.Id, out var userId))
        {
            throw new InvalidArgumentException(nameof(userId), request.Id, Constants.GuidFormat);
        }
        _userService.StartDeleteUserSaga(userId, loggedUserId);
        return Task.FromResult(new Empty());
    }

    /// <summary>
    /// Rpc endpoint for creating user entity
    /// </summary>
    /// <param name="request">Request that contains data necessary for creating user entity</param>
    /// <param name="context"></param>
    /// <returns>Empty response</returns>
    [Auth(permissions: "ModifyUsers")]
    public override Task<Empty> Create(CreateDto request, ServerCallContext context)
    {
        var userIdString = context.UserState["UserId"] as string;
        if (!Guid.TryParse(userIdString, out var userId))
        {
            throw new InvalidArgumentException(nameof(userId), userIdString ?? "", Constants.GuidFormat);
        }
        UserData userData = new()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Password = request.Password
        };
        _userService.StartCreateUserSaga(userData, userId);
        return Task.FromResult(new Empty());
    }

    /// <summary>
    /// Rpc endpoint for updating existing user entity
    /// </summary>
    /// <param name="request">Request that contains data necessary for updating user entity</param>
    /// <param name="context"></param>
    /// <returns>Empty response</returns>
    [Auth(permissions: "ModifyUsers")]
    public override Task<Empty> Update(UpdateDto request, ServerCallContext context)
    {
        var loggedUserIdString = context.UserState["UserId"] as string;
        if (!Guid.TryParse(loggedUserIdString, out var loggedUserId))
        {
            throw new InvalidArgumentException(nameof(loggedUserId), loggedUserIdString ?? "", Constants.GuidFormat);
        }
        if (!Guid.TryParse(request.Id, out var userId))
        {
            throw new InvalidArgumentException(nameof(userId), request.Id, Constants.GuidFormat);
        }
        UserData userData = new()
        {
            Id = userId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email.Equals("") ? null : request.Email,
            Password = request.Password.Equals("") ? null : request.Password
        };
        _userService.StartUpdateUserSaga(userData, loggedUserId);
        return Task.FromResult(new Empty());
    }

    /// <summary>
    /// Method for fetching user data necessary for authentication.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns>Response containing all permissions for the given email.</returns>
    /// <exception cref="UserNotFoundException"></exception>
    public override async Task<UserAuthDto> GetByAuthEmail(Email request, ServerCallContext context)
    {
        var user = await _userRepository.FirstOrDefaultAsync(new EmailSpecification(request.Email_));
        if (user == null)
        {
            throw new UserNotFoundException(request.Email_);
        }

        List<string> permissions = new();
        foreach (var role in user.Roles)
        {
            permissions.AddRange(role.Permissions.Select(permission => permission.Name));
        }
        var data = _mapper.Map<UserAuthDto>(user);
        data.Permissions.AddRange(permissions);
        return data;
    }

    /// <summary>
    /// Method for fetching logged in user data.
    /// </summary>
    /// <param name="request">Empty request.</param>
    /// <param name="context"></param>
    /// <returns>User data from user Id retrieved from auth token.</returns>
    /// <exception cref="InvalidArgumentException"></exception>
    /// <exception cref="UserNotFoundException"></exception>
    [Auth]
    public override async Task<UserDto> GetUserData(Empty request, ServerCallContext context)
    {
        var userIdString = context.UserState["UserId"] as string;
        if (!Guid.TryParse(userIdString, out var userId))
        {
            throw new InvalidArgumentException(nameof(userId), userIdString ?? "", Constants.GuidFormat);
        }
        var user = await _userRepository.FirstOrDefaultAsync(new UserSpecification(userId));
        return user == null ? throw new UserNotFoundException(userId) : _mapper.Map<UserDto>(user);
    }

    public override async Task<Users> GetAllByIds(IdCollection request, ServerCallContext context)
    {
        var userCollection = await _userRepository.ListAsync(new UserSpecification(request.Ids));
        var response = new Users();
        response.Data.AddRange(_mapper.Map<ICollection<UserDto>>(userCollection));
        return response;
    }
}
