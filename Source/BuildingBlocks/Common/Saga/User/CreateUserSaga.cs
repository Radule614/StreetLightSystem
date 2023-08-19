using Common.Saga.User.Dto;

namespace Common.Saga.User;

public enum CreateUserCommandType
{
    UnknownCommand,
    CreateUserStart,
    UpdateTeamService,
    RollbackUser,
    RollbackTeamService,
    ConcludeSuccessfully,
    ConcludeWithFailure
}

public enum CreateUserReplyType
{
    UnknownReply,
    CreateUserFailure,
    CreateUserSuccess,
    UpdateTeamServiceFailure,
    UpdateTeamServiceSuccess,
    UserRolledBack,
    TeamServiceRolledBack
}

public class CreateUserCommand : ICommand<CreateUserCommandType>
{
    public Guid? UserId { get; set; }
    public UserData? UserData { get; set; }
    public CreateUserCommandType Type { get; set; }
    public CreateUserCommandType UnknownType => CreateUserCommandType.UnknownCommand;

    public CreateUserCommand()
    {
        Type = UnknownType;
    }
    public CreateUserCommand(CreateUserReply? reply)
    {
        Type = UnknownType;
        UserData = reply?.UserData;
        UserId = reply?.UserId;
    }
}

public class CreateUserReply : IReply<CreateUserReplyType>
{
    public Guid? UserId { get; set; }
    public UserData? UserData { get; set; }
    public CreateUserReplyType Type { get; set; }
    public CreateUserReplyType UnknownType => CreateUserReplyType.UnknownReply;

    public CreateUserReply()
    {
        Type = UnknownType;
    }

    public CreateUserReply(CreateUserCommand? command)
    {
        Type = UnknownType;
        UserData = command?.UserData;
        UserId = command?.UserId;
    }
}
