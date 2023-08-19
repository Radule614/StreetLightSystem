using Common.Saga.Repair.Dto;

namespace Common.Saga.Repair;

public enum StartRepairCommandType
{
    UnknownCommand,
    StartRepair,
    FindTeamId,
    UpdateRepairTeamId,
    UpdatePole,
    RollbackPole,
    RollbackRepair,
    ConcludeSuccessfully,
    ConcludeWithFailure,
}

public enum StartRepairReplyType
{
    UnknownReply,
    StartRepairSuccess,
    StartRepairFailure,
    TeamIdFound,
    TeamIdNotFound,
    RepairTeamUpdateIdSuccess,
    RepairTeamUpdateIdFailure,
    PoleUpdateSuccess,
    PoleUpdateFailure,
    PoleRolledBack,
    RepairRolledBack
}

public class StartRepairCommand : ICommand<StartRepairCommandType>
{
    public Guid? UserId { get; set; }
    public RepairData? RepairData { get; set; }
    public StartRepairCommandType Type { get; set; }
    public int? OldPoleStatus { get; set; }
    public StartRepairCommandType UnknownType => StartRepairCommandType.UnknownCommand;
    public StartRepairCommand()
    {
        Type = UnknownType;
    }
    public StartRepairCommand(StartRepairReply? reply)
    {
        Type = UnknownType;
        RepairData = reply?.RepairData;
        UserId = reply?.UserId;
        OldPoleStatus = reply?.OldPoleStatus;
    }
}

public class StartRepairReply : IReply<StartRepairReplyType>
{
    public Guid? UserId { get; set; }
    public RepairData? RepairData { get; set; }
    public StartRepairReplyType Type { get; set; }
    public int? OldPoleStatus { get; set; }
    public StartRepairReplyType UnknownType => StartRepairReplyType.UnknownReply;

    public StartRepairReply()
    {
        Type = UnknownType;
    }

    public StartRepairReply(StartRepairCommand? command)
    {
        Type = UnknownType;
        RepairData = command?.RepairData;
        UserId = command?.UserId;
        OldPoleStatus = command?.OldPoleStatus;
    }
}
