using Common.Saga;
using Common.Saga.Repair;

namespace Repair.API.Domain.Services;

public class EndRepairOrchestrator
    : SagaOrchestrator<EndRepairOrchestrator, EndRepairCommand, EndRepairReply, EndRepairCommandType, EndRepairReplyType>
{
    public EndRepairOrchestrator(IConfiguration configuration, ILogger<EndRepairOrchestrator> logger)
        : base(configuration, logger)
    { }

    protected override EndRepairCommand GetNextCommand(EndRepairReply? reply)
    {
        var nextCommand = new EndRepairCommand(reply)
        {
            Type = GetNextCommandType(reply)
        };
        return nextCommand;
    }

    private static EndRepairCommandType GetNextCommandType(EndRepairReply? reply)
    {
        if (reply == null) return EndRepairCommandType.UnknownCommand;
        return reply.Type switch
        {
            EndRepairReplyType.EndRepairFailure => EndRepairCommandType.RollbackRepair,
            EndRepairReplyType.EndRepairSuccess => EndRepairCommandType.ValidateUserTeam,
            EndRepairReplyType.UserTeamInvalid=> EndRepairCommandType.RollbackRepair,
            EndRepairReplyType.UserTeamValid => EndRepairCommandType.UpdatePole,
            EndRepairReplyType.PoleUpdateSuccess => EndRepairCommandType.ConcludeSuccessfully,
            EndRepairReplyType.PoleUpdateFailure => EndRepairCommandType.RollbackPole,
            EndRepairReplyType.RepairRolledBack => EndRepairCommandType.ConcludeWithFailure,
            EndRepairReplyType.PoleRolledBack => EndRepairCommandType.RollbackRepair,
            _ => EndRepairCommandType.UnknownCommand
        };
    }
}
