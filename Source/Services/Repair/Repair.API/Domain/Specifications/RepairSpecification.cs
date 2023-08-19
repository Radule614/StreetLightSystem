using Ardalis.Specification;
using Repair.API.Domain.Entities;

namespace Repair.API.Domain.Specifications;

public sealed class RepairSpecification : Specification<RepairEntity>
{
    public RepairSpecification() { }

    public RepairSpecification(Guid id)
    {
        Query.Where(repair => repair.Id.Equals(id));
    }
}
