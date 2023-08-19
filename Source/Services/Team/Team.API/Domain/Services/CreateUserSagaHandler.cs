using Common;
using Common.Gprc;
using Common.Gprc.Exceptions;
using Common.Notification;
using Common.Saga;
using Common.Saga.User;
using Grpc.Core;
using Team.API.Domain.Entities;
using Team.API.Infrastructure.Data;

namespace Team.API.Domain.Services;

public class CreateUserSagaHandler
    : SagaHandler<CreateUserSagaHandler, CreateUserCommand, CreateUserReply, CreateUserCommandType, CreateUserReplyType>
{
    private readonly INotificationClient _notificationClient;
    public CreateUserSagaHandler(IConfiguration configuration, ILogger<CreateUserSagaHandler> logger, IServiceProvider serviceProvider)
        : base(configuration, logger, serviceProvider)
    {
        _notificationClient = _serviceProvider.GetRequiredService<INotificationClient>();
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _notificationClient.Dispose();
        }
        base.Dispose(true);
    }

    protected override async Task<CreateUserReply> HandleCommand(CreateUserCommand command)
    {
        return command.Type switch
        {
            CreateUserCommandType.UpdateTeamService => await UpdateTeamService(command),
            CreateUserCommandType.RollbackTeamService => await RollbackTeamService(command),
            _ => new CreateUserReply(command)
        };
    }
    
    private async Task<CreateUserReply> UpdateTeamService(CreateUserCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<TeamContext>();
        var memberRepository = new Repository<Member>(dbContext);
        var reply = new CreateUserReply(command);
        var data = command.UserData;
        try
        {
            if (data == null)
            {
                throw new InvalidArgumentException(nameof(command.UserData), "null", "User Data Object");
            }
            if (data.Roles.Select(r => r.Name).Contains("Admin"))
            {
                reply.Type = CreateUserReplyType.UpdateTeamServiceSuccess;
                return reply;
            }
            var member = new Member
            {
                Id = data.Id,
                FirstName = data.FirstName,
                LastName = data.LastName,
                TeamId = null,
                Team = null
            };
            await memberRepository.AddAsync(member);
            reply.Type = CreateUserReplyType.UpdateTeamServiceSuccess;
        }
        catch (RpcException e)
        {
            reply.Type = CreateUserReplyType.UpdateTeamServiceFailure;
            var rpcError = RpcError.ParseRpcErrorMessage(e.Message);
            await _notificationClient.SendNotification(rpcError.Detail, command.UserId ?? Guid.Empty, Constants.CreateUserFailureAction);
        }
        catch (Exception e)
        {
            reply.Type = CreateUserReplyType.UpdateTeamServiceFailure;
            await _notificationClient.SendNotification(e.Message, command.UserId ?? Guid.Empty, Constants.CreateUserFailureAction);
        }
        return reply;
    }

    private async Task<CreateUserReply> RollbackTeamService(CreateUserCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<TeamContext>();
        var memberRepository = new Repository<Member>(dbContext);
        var reply = new CreateUserReply(command)
        {
            Type = CreateUserReplyType.TeamServiceRolledBack
        };
        var data = command.UserData;
        if (data == null)
        {
            return reply;
        }
        var member = await memberRepository.GetByIdAsync(data.Id);
        if (member != null)
        {
            await memberRepository.DeleteAsync(member);
        }
        return reply;
    }
}
