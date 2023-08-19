using Ardalis.Specification;
using Repair.API.Domain.Entities;

namespace Repair.API.Domain.Specifications;

public sealed class TeamSpecification : Specification<RepairEntity>
{
    public TeamSpecification(Guid teamId)
    {
        Query.Where(repair => repair.TeamId.Equals(teamId));
    }
}
