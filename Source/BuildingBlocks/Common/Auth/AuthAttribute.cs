namespace Common.Auth;

/// <summary>
/// Custom attribute used to simplify permission specification in grpc controllers.
/// </summary>

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class AuthAttribute : Attribute
{
    public readonly IEnumerable<string> Permissions;

    public AuthAttribute(string permissions)
    {
        Permissions = permissions.Split(",");
    }

    public AuthAttribute()
    {
        Permissions = new List<string>();
    }
}
