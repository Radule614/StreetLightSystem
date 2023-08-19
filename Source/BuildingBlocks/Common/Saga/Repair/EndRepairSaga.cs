using Common.Saga.Repair.Dto;

namespace Common.Saga.Repair;
public enum EndRepairCommandType
{
    UnknownCommand,
    EndRepair,
    ValidateUserTeam,
    UpdatePole,
    RollbackPole,
    RollbackRepair,
    ConcludeSuccessfully,
    ConcludeWithFailure,
}

public enum EndRepairReplyType
{
    UnknownReply,
    EndRepairSuccess,
    EndRepairFailure,
    UserTeamValid,
    UserTeamInvalid,
    PoleUpdateSuccess,
    PoleUpdateFailure,
    PoleRolledBack,
    RepairRolledBack
}

public class EndRepairCommand : ICommand<EndRepairCommandType>
{
    public Guid? UserId { get; set; }
    public RepairData? RepairData { get; set; }
    public EndRepairCommandType Type { get; set; }
    public int? OldPoleStatus { get; set; }
    public EndRepairCommandType UnknownType => EndRepairCommandType.UnknownCommand;
    public EndRepairCommand()
    {
        Type = UnknownType;
    }
    public EndRepairCommand(EndRepairReply? reply)
    {
        Type = UnknownType;
        RepairData = reply?.RepairData;
        UserId = reply?.UserId;
        OldPoleStatus = reply?.OldPoleStatus;
    }
}

public class EndRepairReply : IReply<EndRepairReplyType>
{
    public Guid? UserId { get; set; }
    public RepairData? RepairData { get; set; }
    public EndRepairReplyType Type { get; set; }
    public int? OldPoleStatus { get; set; }
    public EndRepairReplyType UnknownType => EndRepairReplyType.UnknownReply;

    public EndRepairReply()
    {
        Type = UnknownType;
    }

    public EndRepairReply(EndRepairCommand? command)
    {
        Type = UnknownType;
        RepairData = command?.RepairData;
        UserId = command?.UserId;
        OldPoleStatus = command?.OldPoleStatus;
    }
}
