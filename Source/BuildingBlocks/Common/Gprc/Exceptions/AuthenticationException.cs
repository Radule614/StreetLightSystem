using Grpc.Core;

namespace Common.Gprc.Exceptions;
public class AuthenticationException : RpcException
{
    /// <summary>
    /// Constructor for creating detailed AuthenticationException.
    /// </summary>
    /// <param name="reason">Reason for authentication exception.</param>
    public AuthenticationException(string reason) :
        base(new Status(
            StatusCode.Unauthenticated,
            $"The request does not have valid authentication credentials. Reason: {reason}"))
    { }
}
