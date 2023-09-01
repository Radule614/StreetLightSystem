using Grpc.Core;

namespace Repair.API.Domain.Exceptions;

public class RepairAlreadyFinishedException : RpcException
{
    public RepairAlreadyFinishedException(Guid repairId, DateTime? endDate) :
        base(new Status(
            StatusCode.InvalidArgument,
            $"Repair process with id: {repairId} has already been finished on: {endDate}"))
    { }
}
