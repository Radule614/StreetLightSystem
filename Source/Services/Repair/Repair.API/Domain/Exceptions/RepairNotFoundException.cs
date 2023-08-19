using Common.Gprc.Exceptions;

namespace Repair.API.Domain.Exceptions;

/// <summary>
/// RepairNotFoundException used by repair micro service to express that the repair entity has not been found.
/// </summary>
public class RepairNotFoundException : EntityNotFoundException
{
    /// <param name="repairId">Id of the repair entity that has not been found.</param>
    public RepairNotFoundException(Guid repairId) :
        base("Repair", repairId)
    { }
}