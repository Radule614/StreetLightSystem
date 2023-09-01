using Ardalis.Specification;
using Repair.API.Domain.Entities;

namespace Repair.API.Domain.Specifications;

public sealed class PoleNotFinishedSpecification : Specification<RepairEntity>
{
    public PoleNotFinishedSpecification(Guid poleId)
    {
        Query.Where(repair => repair.PoleId.Equals(poleId) && repair.EndDate == null);
    }
}