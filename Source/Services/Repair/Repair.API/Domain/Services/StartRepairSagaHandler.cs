using AutoMapper;
using Common;
using Common.Gprc.Exceptions;
using Common.Gprc;
using Common.Notification;
using Common.Saga;
using Common.Saga.Repair;
using Common.Saga.Repair.Dto;
using Grpc.Core;
using Repair.API.Domain.Exceptions;
using Repair.API.Domain.Specifications;
using Repair.API.Infrastructure.Data;

namespace Repair.API.Domain.Services;

public class StartRepairSagaHandler
    : SagaHandler<StartRepairSagaHandler, StartRepairCommand, StartRepairReply, StartRepairCommandType, StartRepairReplyType>
{
    private readonly INotificationClient _notificationClient;
    private readonly IMapper _mapper;
    public StartRepairSagaHandler(IConfiguration configuration, ILogger<StartRepairSagaHandler> logger, IServiceProvider serviceProvider)
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

    protected override async Task<StartRepairReply> HandleCommand(StartRepairCommand command)
    {
        return command.Type switch
        {
            StartRepairCommandType.StartRepair => await StartRepair(command),
            StartRepairCommandType.RollbackRepair => await RollbackRepair(command),
            StartRepairCommandType.UpdateRepairTeamId => await UpdateTeamId(command),
            StartRepairCommandType.ConcludeSuccessfully => await ConcludeSuccessfully(command),
            StartRepairCommandType.ConcludeWithFailure => await ConcludeWithFailure(command),
            _ => new StartRepairReply(command)
        };
    }

    private async Task<StartRepairReply> StartRepair(StartRepairCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<RepairContext>();
        var repairRepository = new RepairRepository(dbContext);
        using var repairService = scope.ServiceProvider.GetRequiredService<IRepairService>();
        var reply = new StartRepairReply(command);
        var data = command.RepairData;
        var userId = command.UserId ?? Guid.Empty;
        try
        {
            if (data == null)
            {
                throw new InvalidArgumentException(nameof(command.RepairData), "null", "Repair Data Object");
            }
            if (await repairRepository.FirstOrDefaultAsync(new PoleNotFinishedSpecification(data.PoleId)) != null)
            {
                throw new RepairAlreadyInProgressException(data.PoleId);
            }
            var repair = await repairService.Create(data.PoleId);
            reply.RepairData = _mapper.Map<RepairData>(repair);
            reply.Type = StartRepairReplyType.StartRepairSuccess;
        }
        catch (RpcException e)
        {
            reply.Type = StartRepairReplyType.StartRepairFailure;
            var rpcError = RpcError.ParseRpcErrorMessage(e.Message);
            await _notificationClient.SendNotification(rpcError.Detail, userId, Constants.StartRepairFailureAction);
        }
        catch (Exception e)
        {
            reply.Type = StartRepairReplyType.StartRepairFailure;
            await _notificationClient.SendNotification(e.Message, userId, Constants.StartRepairFailureAction);
        }
        return reply;
    }

    private async Task<StartRepairReply> UpdateTeamId(StartRepairCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<RepairContext>();
        var repairRepository = new RepairRepository(dbContext);
        var reply = new StartRepairReply(command);
        var data = command.RepairData;
        var userId = command.UserId ?? Guid.Empty;
        try
        {
            if (data == null)
            {
                throw new InvalidArgumentException(nameof(command.RepairData), "null", "Repair Data Object");
            }
            if (data.TeamId == null)
            {
                throw new InvalidArgumentException(nameof(data.TeamId), "null", Constants.GuidFormat);
            }
            var repair = await repairRepository.FirstOrDefaultAsync(new RepairSpecification(data.Id));
            if (repair == null)
            {
                throw new RepairNotFoundException(data.Id);
            }
            repair.TeamId = data.TeamId;
            await repairRepository.UpdateAsync(repair);
            reply.RepairData = _mapper.Map<RepairData>(repair);
            reply.Type = StartRepairReplyType.RepairTeamUpdateIdSuccess;
        }
        catch (RpcException e)
        {
            reply.Type = StartRepairReplyType.RepairTeamUpdateIdFailure;
            var rpcError = RpcError.ParseRpcErrorMessage(e.Message);
            await _notificationClient.SendNotification(rpcError.Detail, userId, Constants.StartRepairFailureAction);
        }
        catch (Exception e)
        {
            reply.Type = StartRepairReplyType.RepairTeamUpdateIdFailure;
            await _notificationClient.SendNotification(e.Message, userId, Constants.StartRepairFailureAction);
        }
        return reply;
    }

    private async Task<StartRepairReply> RollbackRepair(StartRepairCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<RepairContext>();
        var repairRepository = new RepairRepository(dbContext);
        var reply = new StartRepairReply(command)
        {
            Type = StartRepairReplyType.RepairRolledBack
        };
        var data = command.RepairData;
        if (data == null)
        {
            return reply;
        }
        var repair = await repairRepository.FirstOrDefaultAsync(new RepairSpecification(data.Id));
        if (repair != null)
        {
            await repairRepository.DeleteAsync(repair);
        }
        return reply;
    }

    private async Task<StartRepairReply> ConcludeSuccessfully(StartRepairCommand command)
    {
        await _notificationClient.SendNotification("Repair process begun successfully.", command.UserId ?? Guid.Empty, Constants.StartRepairSuccessAction);
        return new StartRepairReply(command);
    }

    private Task<StartRepairReply> ConcludeWithFailure(StartRepairCommand command)
    {
        return Task.FromResult(new StartRepairReply(command));
    }
}
