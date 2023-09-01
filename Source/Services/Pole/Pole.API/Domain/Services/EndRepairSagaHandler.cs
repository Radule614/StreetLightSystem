using Common;
using Common.Gprc.Exceptions;
using Common.Gprc;
using Common.Notification;
using Common.Saga.Repair;
using Common.Saga;
using Grpc.Core;
using Pole.API.Domain.Entities;
using Pole.API.Domain.Exceptions;
using Pole.API.Domain.Specifications;
using Pole.API.Infrastructure.Data;

namespace Pole.API.Domain.Services;

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
            EndRepairCommandType.UpdatePole => await UpdatePole(command),
            EndRepairCommandType.RollbackPole => await RollbackPole(command),
            _ => new EndRepairReply(command)
        };
    }

    private async Task<EndRepairReply> UpdatePole(EndRepairCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<PoleContext>();
        var poleRepository = new PoleRepository(dbContext);
        var reply = new EndRepairReply(command);
        var data = command.RepairData;
        var userId = command.UserId ?? Guid.Empty;
        try
        {
            if (data == null)
            {
                throw new InvalidArgumentException(nameof(command.RepairData), "null", "Repair Data Object");
            }
            var pole = await poleRepository.FirstOrDefaultAsync(new PoleSpecification(data.PoleId));
            if (pole == null)
            {
                throw new PoleNotFoundException(data.PoleId);
            }
            if (pole.Status != PoleStatus.BeingRepaired)
            {
                throw new InvalidPoleStatus(PoleStatus.BeingRepaired.ToString(), pole.Status.ToString());
            }
            reply.OldPoleStatus = (int)pole.Status;
            pole.Status = data.IsSuccessful ? PoleStatus.Working : PoleStatus.Broken;
            await poleRepository.UpdateAsync(pole);
            reply.Type = EndRepairReplyType.PoleUpdateSuccess;
        }
        catch (RpcException e)
        {
            reply.Type = EndRepairReplyType.PoleUpdateFailure;
            var rpcError = RpcError.ParseRpcErrorMessage(e.Message);
            await _notificationClient.SendNotification(rpcError.Detail, userId, Constants.EndRepairFailureAction);
        }
        catch (Exception e)
        {
            reply.Type = EndRepairReplyType.PoleUpdateFailure;
            await _notificationClient.SendNotification(e.Message, userId, Constants.EndRepairFailureAction);
        }
        return reply;
    }

    private async Task<EndRepairReply> RollbackPole(EndRepairCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<PoleContext>();
        var poleRepository = new PoleRepository(dbContext);
        var reply = new EndRepairReply(command)
        {
            Type = EndRepairReplyType.PoleRolledBack
        };
        var data = command.RepairData;
        if (data == null || command.OldPoleStatus == null)
        {
            return reply;
        }
        var pole = await poleRepository.FirstOrDefaultAsync(new PoleSpecification(data.PoleId));
        if (pole == null) return reply;
        pole.Status = (PoleStatus)command.OldPoleStatus;
        await poleRepository.UpdateAsync(pole);
        return reply;
    }
}
