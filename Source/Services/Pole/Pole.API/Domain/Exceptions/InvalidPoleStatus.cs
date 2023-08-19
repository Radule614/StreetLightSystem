using Grpc.Core;

namespace Pole.API.Domain.Exceptions;

public class InvalidPoleStatus : RpcException
{
    public InvalidPoleStatus(string expected, string actual) :
        base(new Status(
            StatusCode.InvalidArgument,
            $"Invalid pole status. Expected: {expected}, Actual: {actual}."))
    { }
}
