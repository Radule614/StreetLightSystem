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
            StartRepairCommandType.UpdatePole => await UpdatePole(command),
            StartRepairCommandType.RollbackPole => await RollbackPole(command),
            _ => new StartRepairReply(command)
        };
    }

    private async Task<StartRepairReply> UpdatePole(StartRepairCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<PoleContext>();
        var poleRepository = new PoleRepository(dbContext);
        var reply = new StartRepairReply(command);
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
            if (pole.Status != PoleStatus.Broken)
            {
                throw new InvalidPoleStatus(PoleStatus.Broken.ToString(), pole.Status.ToString());
            }
            reply.OldPoleStatus = (int)pole.Status;
            pole.Status = PoleStatus.BeingRepaired;
            await poleRepository.UpdateAsync(pole);
            reply.Type = StartRepairReplyType.PoleUpdateSuccess;
        }
        catch (RpcException e)
        {
            reply.Type = StartRepairReplyType.PoleUpdateFailure;
            var rpcError = RpcError.ParseRpcErrorMessage(e.Message);
            await _notificationClient.SendNotification(rpcError.Detail, userId, Constants.StartRepairFailureAction);
        }
        catch (Exception e)
        {
            reply.Type = StartRepairReplyType.PoleUpdateFailure;
            await _notificationClient.SendNotification(e.Message, userId, Constants.StartRepairFailureAction);
        }
        return reply;
    }

    private async Task<StartRepairReply> RollbackPole(StartRepairCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<PoleContext>();
        var poleRepository = new PoleRepository(dbContext);
        var reply = new StartRepairReply(command)
        {
            Type = StartRepairReplyType.PoleRolledBack
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
