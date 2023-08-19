using Grpc.Core;

namespace Common.Gprc.Exceptions;
public class AuthorizationException : RpcException
{
    /// <summary>
    /// Constructor for creating detailed AuthorizationException.
    /// </summary>
    /// <param name="userRoles">User roles.</param>
    /// <param name="requiredPermissions">Required permissions</param>
    public AuthorizationException(IEnumerable<string>? userRoles, IEnumerable<string>? requiredPermissions) :
        base(new Status(
            StatusCode.PermissionDenied,
            $"The user does not have access. Required permissions: " +
            $"{string.Join(",", requiredPermissions ?? new List<string>())}. " +
            $"User permissions: {string.Join(",", userRoles ?? new List<string>())}"))
    { }

    /// <summary>
    /// Constructor for creating unauthorized resource access exception.
    /// </summary>
    /// <param name="resourceName">Name of the requested resource.</param>
    /// <param name="details">Details.</param>
    public AuthorizationException(string resourceName, string details) :
        base(new Status(
            StatusCode.PermissionDenied,
            $"User does not have access to requested resource: {resourceName}. Details: {details}"))
    { }
}
