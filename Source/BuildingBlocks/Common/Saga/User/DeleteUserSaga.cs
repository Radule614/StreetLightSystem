using Common.Saga.User.Dto;

namespace Common.Saga.User;

public enum DeleteUserCommandType
{
    UnknownCommand,
    DeleteUserStart,
    UpdateTeamService,
    RollbackUser,
    RollbackTeamService,
    ConcludeSuccessfully,
    ConcludeWithFailure
}

public enum DeleteUserReplyType
{
    UnknownReply,
    DeleteUserFailure,
    DeleteUserSuccess,
    UpdateTeamServiceFailure,
    UpdateTeamServiceSuccess,
    UserRolledBack,
    TeamServiceRolledBack
}

public class DeleteUserCommand : ICommand<DeleteUserCommandType>
{
    public Guid? UserId { get; set; }
    public Guid? UserToDeleteId { get; set; }
    public UserData? OldUserData { get; set; }
    public DeleteUserCommandType Type { get; set; }
    public DeleteUserCommandType UnknownType => DeleteUserCommandType.UnknownCommand;
    public DeleteUserCommand()
    {
        Type = UnknownType;
    }
    public DeleteUserCommand(DeleteUserReply? reply)
    {
        Type = UnknownType;
        OldUserData = reply?.OldUserData;
        UserId = reply?.UserId;
        UserToDeleteId = reply?.UserToDeleteId;
    }
}

public class DeleteUserReply : IReply<DeleteUserReplyType>
{
    public Guid? UserId { get; set; }
    public Guid? UserToDeleteId { get; set; }
    public UserData? OldUserData { get; set; }
    public DeleteUserReplyType Type { get; set; }
    public DeleteUserReplyType UnknownType => DeleteUserReplyType.UnknownReply;
    public DeleteUserReply()
    {
        Type = UnknownType;
    }

    public DeleteUserReply(DeleteUserCommand? command)
    {
        Type = UnknownType;
        OldUserData = command?.OldUserData;
        UserId = command?.UserId;
        UserToDeleteId = command?.UserToDeleteId;
    }
}