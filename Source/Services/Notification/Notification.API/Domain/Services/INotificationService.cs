using NotificationProto;

namespace Notification.API.Domain.Services;

public interface INotificationService
{
    /// <summary>
    /// Method for sending a message to another user.
    /// </summary>
    /// <param name="message">Content of the message.</param>
    /// <param name="receiverId">Id of the user that's supposed to receive the message.</param>
    /// <param name="senderId">Id of the sender.</param>
    /// <param name="senderData">Additional sender data.</param>
    Task SendMessage(string message, Guid receiverId, Guid senderId, Sender senderData);
    /// <summary>
    /// Method for broadcasting a notification to all currently connected users.
    /// </summary>
    /// <param name="notification">Notification data.</param>
    Task BroadcastNotification(BroadcastNotificationDto notification);
    Task SendNotification(NotificationDto notification);
    /// <summary>
    /// Method for fetching all user messages.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<ICollection<MessageDto>> GetUserMessages(Guid userId);
    /// <summary>
    /// Method for sending unsent notifications to the currently connected user.
    /// </summary>
    /// <param name="userId">Id of the user that will receive the notifications.</param>
    Task NotifyUnsentMessages(Guid userId);
}
