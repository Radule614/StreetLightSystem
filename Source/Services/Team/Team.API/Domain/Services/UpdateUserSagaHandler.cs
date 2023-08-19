using Common;
using Common.Gprc;
using Common.Gprc.Exceptions;
using Common.Notification;
using Common.Saga;
using Common.Saga.User;
using Grpc.Core;
using Team.API.Domain.Entities;
using Team.API.Domain.Exceptions;
using Team.API.Infrastructure.Data;

namespace Team.API.Domain.Services;

public class UpdateUserSagaHandler
    : SagaHandler<UpdateUserSagaHandler, UpdateUserCommand, UpdateUserReply, UpdateUserCommandType, UpdateUserReplyType>
{
    private readonly INotificationClient _notificationClient;
    public UpdateUserSagaHandler(IConfiguration configuration, ILogger<UpdateUserSagaHandler> logger, IServiceProvider serviceProvider)
        : base(configuration, logger, serviceProvider)
    {
        _notificationClient = _serviceProvider.GetRequiredService<INotificationClient>();
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
            UpdateUserCommandType.UpdateTeamService => await UpdateTeamService(command),
            UpdateUserCommandType.RollbackTeamService => await RollbackTeamService(command),
            _ => new UpdateUserReply(command)
        };
    }
    private async Task<UpdateUserReply> UpdateTeamService(UpdateUserCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<TeamContext>();
        var memberRepository = new Repository<Member>(dbContext);
        var reply = new UpdateUserReply(command);
        var data = command.UserData;
        try
        {
            if (data == null)
            {
                throw new InvalidArgumentException(nameof(command.UserData), "null", "User Data Object");
            }
            if (data.Roles.Select(r => r.Name).Contains("Admin"))
            {
                reply.Type = UpdateUserReplyType.UpdateTeamServiceSuccess;
                return reply;
            }
            var member = await memberRepository.GetByIdAsync(data.Id);
            if (member == null)
            {
                throw new MemberNotFoundException(data.Id);
            }
            member.FirstName = data.FirstName;
            member.LastName = data.LastName;
            await memberRepository.UpdateAsync(member);
            reply.Type = UpdateUserReplyType.UpdateTeamServiceSuccess;
        }
        catch (RpcException e)
        {
            reply.Type = UpdateUserReplyType.UpdateTeamServiceFailure;
            var rpcError = RpcError.ParseRpcErrorMessage(e.Message);
            await _notificationClient.SendNotification(rpcError.Detail, command.UserId ?? Guid.Empty, Constants.UpdateUserFailureAction);
        }
        catch (Exception e)
        {
            reply.Type = UpdateUserReplyType.UpdateTeamServiceFailure;
            await _notificationClient.SendNotification(e.Message, command.UserId ?? Guid.Empty, Constants.UpdateUserFailureAction);
        }
        return reply;
    }

    private async Task<UpdateUserReply> RollbackTeamService(UpdateUserCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<TeamContext>();
        var memberRepository = new Repository<Member>(dbContext);
        var reply = new UpdateUserReply(command)
        {
            Type = UpdateUserReplyType.TeamServiceRolledBack
        };
        var data = command.OldUserData;
        if (data == null)
        {
            return reply;
        }
        var user = await memberRepository.GetByIdAsync(data.Id);
        if (user == null) return reply;
        user.FirstName = data.FirstName;
        user.LastName = data.LastName;
        await memberRepository.UpdateAsync(user);
        return reply;
    }
}
