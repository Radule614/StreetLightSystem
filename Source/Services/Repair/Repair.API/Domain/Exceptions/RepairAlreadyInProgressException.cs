using Grpc.Core;

namespace Repair.API.Domain.Exceptions;

public class RepairAlreadyInProgressException : RpcException
{
    public RepairAlreadyInProgressException(Guid poleId) :
        base(new Status(
            StatusCode.InvalidArgument,
            $"Repair process for the pole with id: {poleId} has already been started."))
    { }
}
