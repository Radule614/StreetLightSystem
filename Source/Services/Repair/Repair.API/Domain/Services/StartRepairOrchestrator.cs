using Common.Saga.Repair;
using Common.Saga;

namespace Repair.API.Domain.Services;

public class StartRepairOrchestrator
    : SagaOrchestrator<StartRepairOrchestrator, StartRepairCommand, StartRepairReply, StartRepairCommandType, StartRepairReplyType>
{
    public StartRepairOrchestrator(IConfiguration configuration, ILogger<StartRepairOrchestrator> logger)
        : base(configuration, logger)
    { }

    protected override StartRepairCommand GetNextCommand(StartRepairReply? reply)
    {
        var nextCommand = new StartRepairCommand(reply)
        {
            Type = GetNextCommandType(reply)
        };
        return nextCommand;
    }

    private static StartRepairCommandType GetNextCommandType(StartRepairReply? reply)
    {
        if (reply == null) return StartRepairCommandType.UnknownCommand;
        return reply.Type switch
        {
            StartRepairReplyType.StartRepairFailure => StartRepairCommandType.RollbackRepair,
            StartRepairReplyType.StartRepairSuccess => StartRepairCommandType.FindTeamId,
            StartRepairReplyType.TeamIdNotFound => StartRepairCommandType.RollbackRepair,
            StartRepairReplyType.TeamIdFound => StartRepairCommandType.UpdateRepairTeamId,
            StartRepairReplyType.RepairTeamUpdateIdFailure => StartRepairCommandType.RollbackRepair,
            StartRepairReplyType.RepairTeamUpdateIdSuccess => StartRepairCommandType.UpdatePole,
            StartRepairReplyType.PoleUpdateSuccess => StartRepairCommandType.ConcludeSuccessfully,
            StartRepairReplyType.PoleUpdateFailure => StartRepairCommandType.RollbackPole,
            StartRepairReplyType.RepairRolledBack => StartRepairCommandType.ConcludeWithFailure,
            StartRepairReplyType.PoleRolledBack => StartRepairCommandType.RollbackRepair,
            _ => StartRepairCommandType.UnknownCommand
        };
    }
}
