using Common.Saga.User.Dto;

namespace Common.Saga.User;

public enum UpdateUserCommandType
{
    UnknownCommand,
    UpdateUserStart,
    UpdateTeamService,
    RollbackUser,
    RollbackTeamService,
    ConcludeSuccessfully,
    ConcludeWithFailure
}

public enum UpdateUserReplyType
{
    UnknownReply,
    UpdateUserFailure,
    UpdateUserSuccess,
    UpdateTeamServiceFailure,
    UpdateTeamServiceSuccess,
    UserRolledBack,
    TeamServiceRolledBack
}

public class UpdateUserCommand : ICommand<UpdateUserCommandType>
{
    public Guid? UserId { get; set; }
    public UserData? UserData { get; set; }
    public UserData? OldUserData { get; set; }
    public UpdateUserCommandType Type { get; set; }
    public UpdateUserCommandType UnknownType => UpdateUserCommandType.UnknownCommand;

    public UpdateUserCommand()
    {
        Type = UnknownType;
    }
    public UpdateUserCommand(UpdateUserReply? reply)
    {
        Type = UnknownType;
        UserData = reply?.UserData;
        UserId = reply?.UserId;
        OldUserData = reply?.OldUserData;
    }
}

public class UpdateUserReply : IReply<UpdateUserReplyType>
{
    public Guid? UserId { get; set; }
    public UserData? UserData { get; set; }
    public UserData? OldUserData { get; set; }
    public UpdateUserReplyType Type { get; set; }
    public UpdateUserReplyType UnknownType => UpdateUserReplyType.UnknownReply;

    public UpdateUserReply()
    {
        Type = UnknownType;
    }

    public UpdateUserReply(UpdateUserCommand? command)
    {
        Type = UnknownType;
        UserData = command?.UserData;
        UserId = command?.UserId;
        OldUserData = command?.OldUserData;
    }
}
