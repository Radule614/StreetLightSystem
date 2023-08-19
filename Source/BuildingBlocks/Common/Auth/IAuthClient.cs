using Grpc.Core;

namespace Common.Auth;
/// <summary>
/// Used by grpc services to specify their call to auth service.
/// Call to auth service can't be abstracted in common module due to proto files not being generated.
/// </summary>
public interface IAuthClient: IDisposable
{
    /// <summary>
    /// Method is supposed to make a grpc call to auth service in order to authorize request.
    /// </summary>
    /// <param name="permissions">List of permissions that can authorize request. User must have at least one of them.</param>
    /// <param name="context"></param>
    Task<ClaimData> ValidateSession(IEnumerable<string> permissions, ServerCallContext context);
}
