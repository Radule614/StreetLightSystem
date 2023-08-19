using Common.Gprc.Exceptions;
using Grpc.Core;
using Microsoft.AspNetCore.SignalR;
using NotificationProto;

namespace ApiGateway.Services;

public class NotificationService : GatewayProto.NotificationService.NotificationServiceBase
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <summary>
    /// Rpc endpoint for pushing a message through SignalR hub to the connected user.
    /// </summary>
    /// <param name="request">Message data.</param>
    /// <param name="context"></param>
    /// <returns>Empty result.</returns>
    /// <exception cref="ConnectionException">Gets thrown if user connection doesn't exist.</exception>
    public override async Task<Empty> SendMessage(MessageDto request, ServerCallContext context)
    {
        if (!NotificationHub.IsConnectionActive(request.ReceiverId))
        {
            throw new ConnectionException(request.ReceiverId);
        }
        await _hubContext.Clients.Group(request.ReceiverId).SendAsync("message", request.ReceiverId, request);
        return new Empty();
    }
    /// <summary>
    /// Rpc endpoint for broadcasting a notification through SignalR hub to all currently connected users.
    /// </summary>
    /// <param name="request">Notification data.</param>
    /// <param name="context"></param>
    /// <returns>Empty result.</returns>
    public override async Task<Empty> BroadcastNotification(BroadcastNotificationDto request, ServerCallContext context)
    {
        await _hubContext.Clients.All.SendAsync("broadcast", null, request);
        return new Empty();
    }

    public override async Task<Empty> SendNotification(NotificationDto request, ServerCallContext context)
    {
        await _hubContext.Clients.Group(request.ReceiverId).SendAsync("notification", request.ReceiverId, request);
        return new Empty();
    }

    /// <summary>
    /// Rpc endpoint for sending multiple messages to the connected user.
    /// </summary>
    /// <param name="request">Message data.</param>
    /// <param name="context"></param>
    /// <returns>Empty result.</returns>
    /// <exception cref="ConnectionException">Gets thrown if user connection doesn't exist.</exception>
    public override async Task<Empty> SendMessages(MessageList request, ServerCallContext context)
    {
        var receiverId = request.Messages.Count > 0 ? request.Messages[0].ReceiverId : null;
        if (receiverId != null && !NotificationHub.IsConnectionActive(receiverId))
        {
            throw new ConnectionException(receiverId);
        }

        foreach (var message in request.Messages)
        {
            await _hubContext.Clients.Group(message.ReceiverId).SendAsync("message", message.ReceiverId, message);
        }
        return new Empty();
    }
}
