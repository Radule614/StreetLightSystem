using AutoMapper;
using Common;
using Common.Gprc;
using Common.Gprc.Exceptions;
using Common.Notification;
using Common.Saga;
using Common.Saga.User;
using Common.Saga.User.Dto;
using Grpc.Core;
using User.API.Domain.Entities;
using User.API.Domain.Exceptions;
using User.API.Domain.Specifications;
using User.API.Infrastructure.Data;

namespace User.API.Domain.Services;

public class DeleteUserSagaHandler
    : SagaHandler<DeleteUserSagaHandler, DeleteUserCommand, DeleteUserReply, DeleteUserCommandType, DeleteUserReplyType>
{
    private readonly INotificationClient _notificationClient;
    private readonly IMapper _mapper;
    public DeleteUserSagaHandler(IConfiguration configuration, ILogger<DeleteUserSagaHandler> logger, IServiceProvider serviceProvider) 
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

    protected override async Task<DeleteUserReply> HandleCommand(DeleteUserCommand command)
    {
        return command.Type switch
        {
            DeleteUserCommandType.DeleteUserStart => await DeleteUser(command),
            DeleteUserCommandType.RollbackUser => await RollbackUser(command),
            DeleteUserCommandType.ConcludeSuccessfully => await ConcludeSuccessfully(command),
            DeleteUserCommandType.ConcludeWithFailure => await ConcludeWithFailure(command),
            _ => new DeleteUserReply(command)
        };
    }

    private async Task<DeleteUserReply> DeleteUser(DeleteUserCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<UserContext>();
        var userRepository = new Repository<UserEntity>(dbContext);
        var reply = new DeleteUserReply(command);
        try
        {
            if (command.UserToDeleteId == null)
            {
                throw new InvalidArgumentException("UserToDeleteId", command.UserToDeleteId.ToString() ?? "null", Constants.GuidFormat);
            }
            if (command.UserId.Equals(command.UserToDeleteId))
            {
                throw new AuthorizationException("User", "Can't delete yourself.");
            }
            var user = await userRepository.FirstOrDefaultAsync(new UserSpecification(command.UserToDeleteId ?? Guid.Empty));
            if (user == null)
            {
                throw new UserNotFoundException(command.UserToDeleteId.ToString() ?? "");
            }
            reply.OldUserData = _mapper.Map<UserData>(user);
            await userRepository.DeleteAsync(user);
            reply.Type = DeleteUserReplyType.DeleteUserSuccess;
        }
        catch (RpcException e)
        {
            reply.Type = DeleteUserReplyType.DeleteUserFailure;
            var rpcError = RpcError.ParseRpcErrorMessage(e.Message);
            await _notificationClient.SendNotification(rpcError.Detail, command.UserId ?? Guid.Empty, Constants.DeleteUserFailureAction);
        }
        catch (Exception e)
        {
            reply.Type = DeleteUserReplyType.DeleteUserFailure;
            await _notificationClient.SendNotification(e.Message, command.UserId ?? Guid.Empty, Constants.DeleteUserFailureAction);
        }
        return reply;
    }

    private async Task<DeleteUserReply> RollbackUser(DeleteUserCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<UserContext>();
        var userRepository = new Repository<UserEntity>(dbContext);
        var roleRepository = new Repository<Role>(dbContext);
        var reply = new DeleteUserReply(command)
        {
            Type = DeleteUserReplyType.UserRolledBack
        };
        var data = command.OldUserData;
        if (data == null)
        {
            return reply;
        }
        var user = await userRepository.FirstOrDefaultAsync(new UserSpecification(data.Id));
        if (user != null) return reply;

        user = _mapper.Map<UserEntity>(data);
        var roleIds = data.Roles.Select(role => role.Id);
        user.Roles = await roleRepository.ListAsync(new RoleSpecification(roleIds));
        await userRepository.AddAsync(user);
        return reply;
    }

    private async Task<DeleteUserReply> ConcludeSuccessfully(DeleteUserCommand command)
    {
        await _notificationClient.SendNotification("User deleted successfully.", command.UserId ?? Guid.Empty, Constants.DeleteUserSuccessAction);
        return new DeleteUserReply(command);
    }

    private Task<DeleteUserReply> ConcludeWithFailure(DeleteUserCommand command)
    {
        return Task.FromResult(new DeleteUserReply(command));
    }
}
