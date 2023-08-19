using Common.Saga;
using Common.Saga.User;

namespace User.API.Domain.Services;

public class UpdateUserOrchestrator
    : SagaOrchestrator<UpdateUserOrchestrator, UpdateUserCommand, UpdateUserReply, UpdateUserCommandType, UpdateUserReplyType>
{
    public UpdateUserOrchestrator(IConfiguration configuration, ILogger<UpdateUserOrchestrator> logger)
        : base(configuration, logger)
    { }

    protected override UpdateUserCommand GetNextCommand(UpdateUserReply? reply)
    {
        var nextCommand = new UpdateUserCommand(reply)
        {
            Type = GetNextCommandType(reply)
        };
        return nextCommand;
    }

    private static UpdateUserCommandType GetNextCommandType(UpdateUserReply? reply)
    {
        if (reply == null) return UpdateUserCommandType.UnknownCommand;
        return reply.Type switch
        {
            UpdateUserReplyType.UpdateUserFailure => UpdateUserCommandType.RollbackUser,
            UpdateUserReplyType.UpdateUserSuccess => UpdateUserCommandType.UpdateTeamService,
            UpdateUserReplyType.UpdateTeamServiceFailure => UpdateUserCommandType.RollbackTeamService,
            UpdateUserReplyType.UpdateTeamServiceSuccess => UpdateUserCommandType.ConcludeSuccessfully,
            UpdateUserReplyType.UserRolledBack => UpdateUserCommandType.ConcludeWithFailure,
            UpdateUserReplyType.TeamServiceRolledBack => UpdateUserCommandType.RollbackUser,
            _ => UpdateUserCommandType.UnknownCommand
        };
    }

}
