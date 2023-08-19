using System.ComponentModel.DataAnnotations.Schema;

namespace Notification.API.Domain.Entities;

/// <summary>
/// Notification entity used to model Notification data in the database through Entity framework.
/// </summary>
[Table("Notification")]
public class NotificationEntity
{
    /// <summary>
    /// Notification id used as primary key in a database
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Notification message.
    /// </summary>
    public string Message { get; set; }
    /// <summary>
    /// Id of the user that's supposed to receive the message.
    /// </summary>
    public Guid ReceiverId { get; set; }
    /// <summary>
    /// Boolean value representing whether the user has received the message.
    /// </summary>
    public bool Received { get; set; }
    /// <summary>
    /// Id of the sender.
    /// </summary>
    public Guid SenderId { get; set; }
    /// <summary>
    /// Date when notification was sent.
    /// </summary>
    public DateTime SentDate { get; set; }
    /// <summary>
    /// Date when notification was received by end user.
    /// </summary>
    public DateTime ReceivedDate { get; set; }
    /// <summary>
    /// Indicates if the user has previously fetched notification.
    /// </summary>
    public bool IsNew { get; set; }

    public NotificationEntity()
    {
        Message = string.Empty;
    }
}
