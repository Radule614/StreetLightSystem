using Common;
using Common.Gprc;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using NotificationProto;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/notification")]
public class NotificationController : ControllerBase, IDisposable
{
    /// <summary>
    /// Notification service channel used by notification grpc client
    /// </summary>
    private readonly GrpcChannel _notificationServiceChannel;
    /// <summary>
    /// Notification gprc client used to make grpc requests.
    /// </summary>
    private readonly NotificationGrpc.NotificationGrpcClient _notificationClient;
    public NotificationController(IChannelFactory factory, IConfiguration configuration)
    {
        _notificationServiceChannel = factory.GetChannel(configuration[Constants.NotificationServiceAddress]!);
        _notificationClient = new NotificationGrpc.NotificationGrpcClient(_notificationServiceChannel);
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
        _notificationServiceChannel.Dispose();
    }

    /// <summary>
    /// Http endpoint for sending a message to another user.
    /// </summary>
    /// <param name="data">Message data.</param>
    [HttpPost("message")]
    public async Task SendMessage(SendMessageDto data)
    {
        var metadata = Request.HttpContext.Items["Metadata"] as Metadata;
        await _notificationClient.SendMessageAsync(data, metadata);
    }
    /// <summary>
    /// Http endpoint for getting all messages.
    /// </summary>
    /// <returns>List of all messages.</returns>
    [HttpGet("message")]
    public async Task<MessageList> GetAllMessages()
    {
        var metadata = Request.HttpContext.Items["Metadata"] as Metadata;
        return await _notificationClient.GetMessagesAsync(new Empty(), metadata);
    }
    /// <summary>
    /// Http endpoint for checking if user has any unsent messages and then sending them through SignalR.
    /// </summary>
    [HttpGet("unsent")]
    public async Task NotifyUnsentMessages()
    {
        var metadata = Request.HttpContext.Items["Metadata"] as Metadata;
        await _notificationClient.NotifyUnsentMessagesAsync(new Empty(), metadata);
    }
}
