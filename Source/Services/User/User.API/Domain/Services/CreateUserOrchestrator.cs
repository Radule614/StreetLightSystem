using Common.Saga;
using Common.Saga.User;

namespace User.API.Domain.Services;

public class CreateUserOrchestrator
    : SagaOrchestrator<CreateUserOrchestrator, CreateUserCommand, CreateUserReply, CreateUserCommandType, CreateUserReplyType>
{
    public CreateUserOrchestrator(IConfiguration configuration, ILogger<CreateUserOrchestrator> logger)
        : base(configuration, logger)
    { }

    protected override CreateUserCommand GetNextCommand(CreateUserReply? reply)
    {
        var nextCommand = new CreateUserCommand(reply)
        {
            Type = GetNextCommandType(reply)
        };
        return nextCommand;
    }

    private static CreateUserCommandType GetNextCommandType(CreateUserReply? reply)
    {
        if (reply == null) return CreateUserCommandType.UnknownCommand;
        return reply.Type switch
        {
            CreateUserReplyType.CreateUserFailure => CreateUserCommandType.RollbackUser,
            CreateUserReplyType.CreateUserSuccess => CreateUserCommandType.UpdateTeamService,
            CreateUserReplyType.UpdateTeamServiceFailure => CreateUserCommandType.RollbackTeamService,
            CreateUserReplyType.UpdateTeamServiceSuccess => CreateUserCommandType.ConcludeSuccessfully,
            CreateUserReplyType.UserRolledBack => CreateUserCommandType.ConcludeWithFailure,
            CreateUserReplyType.TeamServiceRolledBack => CreateUserCommandType.RollbackUser,
            _ => CreateUserCommandType.UnknownCommand
        };
    }
}
