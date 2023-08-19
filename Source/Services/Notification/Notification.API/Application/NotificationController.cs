using AutoMapper;
using Common;
using Common.Auth;
using Common.Gprc;
using Common.Gprc.Exceptions;
using Grpc.Core;
using Grpc.Net.Client;
using Notification.API.Domain.Services;
using NotificationProto;
using UserProto;
using Empty = NotificationProto.Empty;

namespace Notification.API.Application;

public class NotificationController : NotificationGrpc.NotificationGrpcBase, IDisposable
{
    /// <summary>
    /// User service channel used by user grpc client
    /// </summary>
    private readonly GrpcChannel _userServiceChannel;
    /// <summary>
    /// User gprc client used to make grpc requests.
    /// </summary>
    private readonly UserGrpc.UserGrpcClient _userClient;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;
    public NotificationController(IConfiguration configuration, IChannelFactory factory, IMapper mapper, INotificationService notificationService)
    {
        _notificationService = notificationService;
        _mapper = mapper;
        _userServiceChannel = factory.GetChannel(configuration[Constants.UserServiceAddress]!);
        _userClient = new UserGrpc.UserGrpcClient(_userServiceChannel);
    }

    ~NotificationController()
    {
        Dispose(false);
    }

    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        _userServiceChannel.Dispose();
    }

    /// <summary>
    /// Rpc endpoint for sending message to another user.
    /// </summary>
    /// <param name="request">Dto object containing all the necessary data.</param>
    /// <param name="context"></param>
    /// <returns>Empty result.</returns>
    [Auth]
    public override async Task<Empty> SendMessage(SendMessageDto request, ServerCallContext context)
    {
        var user = _userClient.GetUserData(new UserProto.Empty(), context.RequestHeaders);
        if (!Guid.TryParse(user.Id, out var userId))
        {
            throw new InvalidArgumentException(nameof(userId), user.Id ?? "", Constants.GuidFormat);
        }
        if (!Guid.TryParse(request.ReceiverId, out var receiverId))
        {
            throw new InvalidArgumentException(nameof(receiverId), request.ReceiverId, Constants.GuidFormat);
        }
        await _notificationService.SendMessage(request.Message, receiverId, userId, _mapper.Map<Sender>(user));
        return new Empty();
    }

    /// <summary>
    /// Rpc endpoint for broadcasting a notification to all currently connected users.
    /// </summary>
    /// <param name="request">Dto object containing all the necessary data.</param>
    /// <param name="context"></param>
    /// <returns>Empty result.</returns>
    public override async Task<Empty> BroadcastNotification(BroadcastNotificationDto request, ServerCallContext context)
    {
        await _notificationService.BroadcastNotification(request);
        return new Empty();
    }

    public override async Task<Empty> SendNotification(NotificationDto request, ServerCallContext context)
    {
        await _notificationService.SendNotification(request);
        return new Empty();
    }

    /// <summary>
    /// Rpc endpoint for fetching all user messages that came from other users.
    /// </summary>
    /// <param name="request">Empty request.</param>
    /// <param name="context"></param>
    /// <returns>List of message dto objects.</returns>
    [Auth]
    public override async Task<MessageList> GetMessages(Empty request, ServerCallContext context)
    {
        var userIdString = context.UserState["UserId"] as string;
        if (!Guid.TryParse(userIdString, out var userId))
        {
            throw new InvalidArgumentException(nameof(userId), userIdString ?? "", Constants.GuidFormat);
        }
        var response = new MessageList();
        response.Messages.AddRange(await _notificationService.GetUserMessages(userId));
        return response;
    }

    /// <summary>
    /// Rpc endpoint for checking unsent messages.
    /// </summary>
    /// <param name="request">Empty request</param>
    /// <param name="context"></param>
    [Auth]
    public override async Task<Empty> NotifyUnsentMessages(Empty request, ServerCallContext context)
    {
        var userIdString = context.UserState["UserId"] as string;
        if (!Guid.TryParse(userIdString, out var userId))
        {
            throw new InvalidArgumentException(nameof(userId), userIdString ?? "", Constants.GuidFormat);
        }
        await _notificationService.NotifyUnsentMessages(userId);
        return new Empty();
    }
}
