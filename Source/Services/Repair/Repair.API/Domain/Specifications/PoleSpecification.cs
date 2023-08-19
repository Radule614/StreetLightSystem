using Ardalis.Specification;
using Repair.API.Domain.Entities;

namespace Repair.API.Domain.Specifications;

public sealed class PoleSpecification : Specification<RepairEntity>
{
    public PoleSpecification(Guid poleId)
    {
        Query.Where(repair => repair.PoleId.Equals(poleId));
    }
}