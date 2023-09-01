using Common;
using Common.Gprc.Exceptions;
using Common.Gprc;
using Common.Notification;
using Common.Saga.Repair;
using Common.Saga;
using Grpc.Core;
using Team.API.Domain.Entities;
using Team.API.Domain.Exceptions;
using Team.API.Domain.Specifications;
using Team.API.Infrastructure.Data;

namespace Team.API.Domain.Services;

public class EndRepairSagaHandler
    : SagaHandler<EndRepairSagaHandler, EndRepairCommand, EndRepairReply, EndRepairCommandType, EndRepairReplyType>
{
    private readonly INotificationClient _notificationClient;
    public EndRepairSagaHandler(IConfiguration configuration, ILogger<EndRepairSagaHandler> logger, IServiceProvider serviceProvider)
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

    protected override async Task<EndRepairReply> HandleCommand(EndRepairCommand command)
    {
        return command.Type switch
        {
            EndRepairCommandType.ValidateUserTeam => await ValidateUserTeam(command),
            _ => new EndRepairReply(command)
        };
    }

    private async Task<EndRepairReply> ValidateUserTeam(EndRepairCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<TeamContext>();
        var teamRepository = new Repository<TeamEntity>(dbContext);
        var reply = new EndRepairReply(command);
        var data = command.RepairData;
        var userId = command.UserId ?? Guid.Empty;
        try
        {
            if (data == null)
            {
                throw new InvalidArgumentException(nameof(command.RepairData), "null", "Repair Data Object");
            }
            var teamId = data.TeamId ?? Guid.Empty;
            var team = await teamRepository.FirstOrDefaultAsync(new TeamSpecification(teamId));
            if (team == null)
            {
                throw new MemberNotFoundException(teamId);
            }
            if (!team.Members.Select(m => m.Id).Contains(userId))
            {
                throw new TeamInvalidMemberException(teamId, userId);
            }
            reply.Type = EndRepairReplyType.UserTeamValid;
        }
        catch (RpcException e)
        {
            reply.Type = EndRepairReplyType.UserTeamInvalid;
            var rpcError = RpcError.ParseRpcErrorMessage(e.Message);
            await _notificationClient.SendNotification(rpcError.Detail, userId, Constants.EndRepairFailureAction);
        }
        catch (Exception e)
        {
            reply.Type = EndRepairReplyType.UserTeamInvalid;
            await _notificationClient.SendNotification(e.Message, userId, Constants.EndRepairFailureAction);
        }
        return reply;
    }
}
