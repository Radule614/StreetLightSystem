using Common;
using Common.Notification;
using Common.Saga.Repair;
using Common.Saga;
using Team.API.Domain.Entities;
using Team.API.Domain.Specifications;
using Team.API.Infrastructure.Data;
using Common.Gprc.Exceptions;
using Common.Gprc;
using Grpc.Core;
using Team.API.Domain.Exceptions;

namespace Team.API.Domain.Services;

public class StartRepairSagaHandler
: SagaHandler<StartRepairSagaHandler, StartRepairCommand, StartRepairReply, StartRepairCommandType, StartRepairReplyType>
{
    private readonly INotificationClient _notificationClient;
    public StartRepairSagaHandler(IConfiguration configuration, ILogger<StartRepairSagaHandler> logger, IServiceProvider serviceProvider)
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

    protected override async Task<StartRepairReply> HandleCommand(StartRepairCommand command)
    {
        return command.Type switch
        {
            StartRepairCommandType.FindTeamId => await FindTeamId(command),
            _ => new StartRepairReply(command)
        };
    }

    private async Task<StartRepairReply> FindTeamId(StartRepairCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<TeamContext>();
        var memberRepository = new Repository<Member>(dbContext);
        var reply = new StartRepairReply(command);
        var data = command.RepairData;
        var userId = command.UserId ?? Guid.Empty;
        try
        {
            if (data == null)
            {
                throw new InvalidArgumentException(nameof(command.RepairData), "null", "Repair Data Object");
            }
            var member = await memberRepository.FirstOrDefaultAsync(new MemberSpecification(userId));
            if (member == null)
            {
                throw new MemberNotFoundException(userId);
            }
            if (member.TeamId == null)
            {
                throw new MemberHasNoTeamException(member.Id);
            }
            reply.RepairData!.TeamId = member.TeamId.Value;
            reply.Type = StartRepairReplyType.TeamIdFound;
        }
        catch (RpcException e)
        {
            reply.Type = StartRepairReplyType.TeamIdNotFound;
            var rpcError = RpcError.ParseRpcErrorMessage(e.Message);
            await _notificationClient.SendNotification(rpcError.Detail, userId, Constants.StartRepairFailureAction);
        }
        catch (Exception e)
        {
            reply.Type = StartRepairReplyType.TeamIdNotFound;
            await _notificationClient.SendNotification(e.Message, userId, Constants.StartRepairFailureAction);
        }
        return reply;
    }
}
