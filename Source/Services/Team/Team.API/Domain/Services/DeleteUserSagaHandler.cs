using AutoMapper;
using Common;
using Common.Gprc;
using Common.Gprc.Exceptions;
using Common.Notification;
using Common.Saga;
using Common.Saga.User;
using Grpc.Core;
using Team.API.Domain.Entities;
using Team.API.Domain.Exceptions;
using Team.API.Infrastructure.Data;

namespace Team.API.Domain.Services;

public class DeleteUserSagaHandler
    : SagaHandler<DeleteUserSagaHandler, DeleteUserCommand, DeleteUserReply, DeleteUserCommandType, DeleteUserReplyType>
{
    private readonly INotificationClient _notificationClient;
    private readonly IMapper _mapper;
    public DeleteUserSagaHandler(IConfiguration configuration, ILogger<DeleteUserSagaHandler> logger, IServiceProvider serviceProvider) 
        : base(configuration, logger, serviceProvider)
    {
        _notificationClient = _serviceProvider.GetRequiredService<INotificationClient>();
        _mapper = _serviceProvider.GetRequiredService<IMapper>();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _notificationClient.Dispose();
        }
        base.Dispose(true);
    }

    protected override async Task<DeleteUserReply> HandleCommand(DeleteUserCommand command)
    {
        return command.Type switch
        {
            DeleteUserCommandType.UpdateTeamService => await UpdateTeamService(command),
            DeleteUserCommandType.RollbackTeamService => await RollbackTeamService(command),
            _ => new DeleteUserReply(command)
        };
    }
    private async Task<DeleteUserReply> UpdateTeamService(DeleteUserCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<TeamContext>();
        var memberRepository = new Repository<Member>(dbContext);
        var reply = new DeleteUserReply(command);
        var data = command.OldUserData;
        try
        {
            if (data == null)
            {
                throw new InvalidArgumentException(nameof(command.OldUserData), "null", "User Data Object");
            }
            if (data.Roles.Select(r => r.Name).Contains("Admin"))
            {
                reply.Type = DeleteUserReplyType.UpdateTeamServiceSuccess;
                return reply;
            }
            var member = await memberRepository.GetByIdAsync(data.Id);
            if (member == null)
            {
                throw new MemberNotFoundException(data.Id);
            }
            await memberRepository.DeleteAsync(member);
            reply.Type = DeleteUserReplyType.UpdateTeamServiceSuccess;
        }
        catch (RpcException e)
        {
            reply.Type = DeleteUserReplyType.UpdateTeamServiceFailure;
            var rpcError = RpcError.ParseRpcErrorMessage(e.Message);
            await _notificationClient.SendNotification(rpcError.Detail, command.UserId ?? Guid.Empty, Constants.DeleteUserFailureAction);
        }
        catch (Exception e)
        {
            reply.Type = DeleteUserReplyType.UpdateTeamServiceFailure;
            await _notificationClient.SendNotification(e.Message, command.UserId ?? Guid.Empty, Constants.DeleteUserFailureAction);
        }
        return reply;
    }

    private async Task<DeleteUserReply> RollbackTeamService(DeleteUserCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<TeamContext>();
        var memberRepository = new Repository<Member>(dbContext);
        var reply = new DeleteUserReply(command)
        {
            Type = DeleteUserReplyType.TeamServiceRolledBack
        };
        var data = command.OldUserData;
        if (data == null)
        {
            return reply;
        }
        var user = await memberRepository.GetByIdAsync(data.Id);
        if (user != null) return reply;

        user = _mapper.Map<Member>(data);
        await memberRepository.AddAsync(user);
        return reply;
    }
}
