namespace Common.Notification;
/// <summary>
/// Used by grpc services to specify their call to notification service.
/// Call to notification service can't be abstracted in common module due to proto files not being generated.
/// </summary>
public interface INotificationClient: IDisposable
{
    /// <summary>
    /// Method for sending notification from a grpc service.
    /// </summary>
    /// <param name="message">Message content.</param>
    /// <param name="userId">Id of the user that's to receive the message.</param>
    /// <param name="action">Notification action that can determine the type of response to notification on the frontend app.</param>
    /// <returns></returns>
    Task SendNotification(string message, Guid userId, string action);
}
