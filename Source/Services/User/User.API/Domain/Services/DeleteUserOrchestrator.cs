using Common.Saga.User;
using Common.Saga;

namespace User.API.Domain.Services;

public class DeleteUserOrchestrator
    : SagaOrchestrator<DeleteUserOrchestrator, DeleteUserCommand, DeleteUserReply, DeleteUserCommandType, DeleteUserReplyType>
{
    public DeleteUserOrchestrator(IConfiguration configuration, ILogger<DeleteUserOrchestrator> logger)
        : base(configuration, logger)
    { }

    protected override DeleteUserCommand GetNextCommand(DeleteUserReply? reply)
    {
        var nextCommand = new DeleteUserCommand(reply)
        {
            Type = GetNextCommandType(reply)
        };
        return nextCommand;
    }

    private static DeleteUserCommandType GetNextCommandType(DeleteUserReply? reply)
    {
        if (reply == null) return DeleteUserCommandType.UnknownCommand;
        return reply.Type switch
        {
            DeleteUserReplyType.DeleteUserFailure => DeleteUserCommandType.RollbackUser,
            DeleteUserReplyType.DeleteUserSuccess => DeleteUserCommandType.UpdateTeamService,
            DeleteUserReplyType.UpdateTeamServiceFailure => DeleteUserCommandType.RollbackTeamService,
            DeleteUserReplyType.UpdateTeamServiceSuccess => DeleteUserCommandType.ConcludeSuccessfully,
            DeleteUserReplyType.UserRolledBack => DeleteUserCommandType.ConcludeWithFailure,
            DeleteUserReplyType.TeamServiceRolledBack => DeleteUserCommandType.RollbackUser,
            _ => DeleteUserCommandType.UnknownCommand
        };
    }
}
