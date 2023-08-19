using Grpc.Core;

namespace Common.Gprc.Exceptions;
public class ConnectionException : RpcException
{
    public ConnectionException(string userId) :
        base(new Status(StatusCode.FailedPrecondition, $"Connection is not present for user: {userId}"))
    { }
}
