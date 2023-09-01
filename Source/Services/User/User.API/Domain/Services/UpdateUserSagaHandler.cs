using Common;
using Common.Gprc;
using Common.Saga.User.Dto;
using Common.Saga.User;
using Common.Saga;
using Grpc.Core;
using User.API.Domain.Entities;
using User.API.Domain.Specifications;
using User.API.Infrastructure.Data;
using AutoMapper;
using User.API.Domain.Exceptions;
using Common.Gprc.Exceptions;
using Common.Notification;

namespace User.API.Domain.Services;

public class UpdateUserSagaHandler
    : SagaHandler<UpdateUserSagaHandler, UpdateUserCommand, UpdateUserReply, UpdateUserCommandType, UpdateUserReplyType>
{
    private readonly INotificationClient _notificationClient;
    private readonly IMapper _mapper;
    public UpdateUserSagaHandler(IConfiguration configuration, ILogger<UpdateUserSagaHandler> logger, IServiceProvider serviceProvider)
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

    protected override async Task<UpdateUserReply> HandleCommand(UpdateUserCommand command)
    {
        return command.Type switch
        {
            UpdateUserCommandType.UpdateUserStart => await UpdateUser(command),
            UpdateUserCommandType.RollbackUser => await RollbackUser(command),
            UpdateUserCommandType.ConcludeSuccessfully => await ConcludeSuccessfully(command),
            UpdateUserCommandType.ConcludeWithFailure => await ConcludeWithFailure(command),
            _ => new UpdateUserReply(command)
        };
    }

    private async Task<UpdateUserReply> UpdateUser(UpdateUserCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<UserContext>();
        var userRepository = new Repository<UserEntity>(dbContext);
        using var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var reply = new UpdateUserReply(command);
        var data = command.UserData;
        try
        {
            if (data == null)
            {
                throw new InvalidArgumentException(nameof(command.UserData), "null", "User Data Object");
            }
            var user = await userRepository.FirstOrDefaultAsync(new UserSpecification(data.Id));
            if (user == null)
            {
                throw new UserNotFoundException(data.Id);
            }
            reply.OldUserData = _mapper.Map<UserData>(user);
            user = await userService.Update(data.Id, data.FirstName, data.LastName, data.Email, data.Password);
            reply.UserData = _mapper.Map<UserData>(user);
            reply.Type = UpdateUserReplyType.UpdateUserSuccess;
        }
        catch (RpcException e)
        {
            reply.Type = UpdateUserReplyType.UpdateUserFailure;
            var rpcError = RpcError.ParseRpcErrorMessage(e.Message);
            await _notificationClient.SendNotification(rpcError.Detail, command.UserId ?? Guid.Empty, Constants.UpdateUserFailureAction);
        }
        catch (Exception e)
        {
            reply.Type = UpdateUserReplyType.UpdateUserFailure;
            await _notificationClient.SendNotification(e.Message, command.UserId ?? Guid.Empty, Constants.UpdateUserFailureAction);
        }
        return reply;
    }

    private async Task<UpdateUserReply> RollbackUser(UpdateUserCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<UserContext>();
        var userRepository = new Repository<UserEntity>(dbContext);
        var roleRepository = new Repository<Role>(dbContext);
        var reply = new UpdateUserReply(command)
        {
            Type = UpdateUserReplyType.UserRolledBack
        };
        
        var data = command.OldUserData;
        if (data == null)
        {
            return reply;
        }
        var user = await userRepository.FirstOrDefaultAsync(new UserSpecification(data.Id));
        if (user == null)
        {
            throw new UserNotFoundException(data.Id);
        }
        user.Email = data.Email!;
        user.FirstName = data.FirstName;
        user.LastName = data.LastName;
        user.Password = data.Password!;
        var roleIds = data.Roles.Select(role => role.Id);
        user.Roles = await roleRepository.ListAsync(new RoleSpecification(roleIds));
        await userRepository.UpdateAsync(user);
        return reply;
    }

    private async Task<UpdateUserReply> ConcludeSuccessfully(UpdateUserCommand command)
    {
        await _notificationClient.SendNotification("User updated successfully.", command.UserId ?? Guid.Empty, Constants.UpdateUserSuccessAction);
        return new UpdateUserReply(command);
    }

    private Task<UpdateUserReply> ConcludeWithFailure(UpdateUserCommand command)
    {
        return Task.FromResult(new UpdateUserReply(command));
    }
}
