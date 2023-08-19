using AutoMapper;
using Common;
using Common.Gprc.Exceptions;
using Common.Gprc;
using Common.Notification;
using Common.Saga.Repair.Dto;
using Common.Saga.Repair;
using Common.Saga;
using Grpc.Core;
using Repair.API.Domain.Specifications;
using Repair.API.Infrastructure.Data;

namespace Repair.API.Domain.Services;

public class EndRepairSagaHandler
: SagaHandler<EndRepairSagaHandler, EndRepairCommand, EndRepairReply, EndRepairCommandType, EndRepairReplyType>
{
    private readonly INotificationClient _notificationClient;
    private readonly IMapper _mapper;
    public EndRepairSagaHandler(IConfiguration configuration, ILogger<EndRepairSagaHandler> logger, IServiceProvider serviceProvider)
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

    protected override async Task<EndRepairReply> HandleCommand(EndRepairCommand command)
    {
        return command.Type switch
        {
            EndRepairCommandType.EndRepair => await EndRepair(command),
            EndRepairCommandType.RollbackRepair => await RollbackRepair(command),
            EndRepairCommandType.ConcludeSuccessfully => await ConcludeSuccessfully(command),
            EndRepairCommandType.ConcludeWithFailure => await ConcludeWithFailure(command),
            _ => new EndRepairReply(command)
        };
    }

    private async Task<EndRepairReply> EndRepair(EndRepairCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        using var repairService = scope.ServiceProvider.GetRequiredService<IRepairService>();
        var reply = new EndRepairReply(command);
        var data = command.RepairData;
        var userId = command.UserId ?? Guid.Empty;
        try
        {
            if (data == null)
            {
                throw new InvalidArgumentException(nameof(command.RepairData), "null", "Repair Data Object");
            }
            var repair = await repairService.EndRepair(data.Id, data.IsSuccessful);
            reply.RepairData = _mapper.Map<RepairData>(repair);
            reply.Type = EndRepairReplyType.EndRepairSuccess;
        }
        catch (RpcException e)
        {
            reply.Type = EndRepairReplyType.EndRepairFailure;
            var rpcError = RpcError.ParseRpcErrorMessage(e.Message);
            await _notificationClient.SendNotification(rpcError.Detail, userId, Constants.EndRepairFailureAction);
        }
        catch (Exception e)
        {
            reply.Type = EndRepairReplyType.EndRepairFailure;
            await _notificationClient.SendNotification(e.Message, userId, Constants.EndRepairFailureAction);
        }
        return reply;
    }

    private async Task<EndRepairReply> RollbackRepair(EndRepairCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<RepairContext>();
        var repairRepository = new RepairRepository(dbContext);
        var reply = new EndRepairReply(command)
        {
            Type = EndRepairReplyType.RepairRolledBack
        };
        var data = command.RepairData;
        if (data == null)
        {
            return reply;
        }
        var repair = await repairRepository.FirstOrDefaultAsync(new RepairSpecification(data.Id));
        if (repair != null)
        {
            repair.IsSuccessful = false;
            repair.EndDate = null;
            await repairRepository.UpdateAsync(repair);
        }
        return reply;
    }

    private async Task<EndRepairReply> ConcludeSuccessfully(EndRepairCommand command)
    {
        await _notificationClient.SendNotification("Repair process ended successfully.", command.UserId ?? Guid.Empty, Constants.EndRepairSuccessAction);
        return new EndRepairReply(command);
    }

    private Task<EndRepairReply> ConcludeWithFailure(EndRepairCommand command)
    {
        return Task.FromResult(new EndRepairReply(command));
    }
}
