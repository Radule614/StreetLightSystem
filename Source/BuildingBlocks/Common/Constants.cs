namespace Common;

/// <summary>
/// Common constants used in the project.
/// </summary>
public class Constants
{
    #region service addresses
    public static readonly string ApiGatewayAddress = "API_GATEWAY_ADDRESS";
    public static readonly string AuthServiceAddress = "AUTH_SERVICE_ADDRESS";
    public static readonly string PoleServiceAddress = "POLE_SERVICE_ADDRESS";
    public static readonly string UserServiceAddress = "USER_SERVICE_ADDRESS";
    public static readonly string TeamServiceAddress = "TEAM_SERVICE_ADDRESS";
    public static readonly string NotificationServiceAddress = "NOTIFICATION_SERVICE_ADDRESS";
    public static readonly string RepairServiceAddress = "REPAIR_SERVICE_ADDRESS";
    public static readonly string MqttBrokerAddress = "MQTT_BROKER_ADDRESS";
    public static readonly string EventQueueAddress = "EVENT_QUEUE_ADDRESS";
    #endregion

    /// <summary>
    /// Represents the database address specified in the docker compose file.
    /// </summary>
    public static readonly string DatabaseAddress = "DATABASE_ADDRESS";
    /// <summary>
    /// Represents the database user specified in the docker compose file.
    /// </summary>
    public static readonly string DatabaseUser = "DATABASE_USER";
    /// <summary>
    /// Represents the database password specified in the docker compose file.
    /// </summary>
    public static readonly string DatabasePassword = "DATABASE_PASSWORD";
    /// <summary>
    /// Guid string format used by exceptions.
    /// </summary>
    public static readonly string GuidFormat = "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX";

    #region actions
    public static readonly string CreateUserSuccessAction = "CreateUserSuccess";
    public static readonly string CreateUserFailureAction = "CreateUserFailure";
    public static readonly string DeleteUserSuccessAction = "DeleteUserSuccess";
    public static readonly string DeleteUserFailureAction = "DeleteUserFailure";
    public static readonly string UpdateUserSuccessAction = "UpdateUserSuccess";
    public static readonly string UpdateUserFailureAction = "UpdateUserFailure";

    public static readonly string StartRepairSuccessAction = "StartRepairSuccess";
    public static readonly string StartRepairFailureAction = "StartRepairFailure";
    public static readonly string EndRepairSuccessAction = "EndRepairSuccess";
    public static readonly string EndRepairFailureAction = "EndRepairFailure";
    #endregion
}
