using Ardalis.Specification;
using Team.API.Domain.Entities;

namespace Team.API.Domain.Specifications;

public sealed class TeamMemberSpecification : Specification<TeamEntity>
{
    public TeamMemberSpecification(Guid memberId)
    {
        Query
            .Include(team => team.Members)
            .Where(team => team.Members.Select(member => member.Id).Contains(memberId));
    }
}
