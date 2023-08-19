using Common;
using Common.Gprc;
using Common.Notification;
using Grpc.Net.Client;
using NotificationProto;

namespace Pole.API.Infrastructure;

/// <inheritdoc />
public class NotificationClient : INotificationClient
{
    /// <summary>
    /// Notification service channel used by notification grpc client
    /// </summary>
    private readonly GrpcChannel _notificationServiceChannel;
    /// <summary>
    /// Notification gprc client used to make grpc requests.
    /// </summary>
    private readonly NotificationGrpc.NotificationGrpcClient _notificationClient;
    private readonly ILogger<NotificationClient> _logger;

    public NotificationClient(IChannelFactory channelFactory, IConfiguration configuration, ILogger<NotificationClient> logger)
    {
        _notificationServiceChannel = channelFactory.GetChannel(configuration[Constants.NotificationServiceAddress]!);
        _notificationClient = new NotificationGrpc.NotificationGrpcClient(_notificationServiceChannel);
        _logger = logger;
    }

    ~NotificationClient()
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

    public async Task SendNotification(string message, Guid userId, string action)
    {
        _logger.LogInformation($"Notification sent from notification client: {message}");
        var data = new NotificationDto
        {
            Message = message,
            ReceiverId = userId.ToString(),
            Action = action
        };
        await _notificationClient.SendNotificationAsync(data);
    }
}
