using Ardalis.Specification;
using Team.API.Domain.Entities;

namespace Team.API.Domain.Specifications;

/// <summary>
/// Member specification class used for team queries.
/// </summary>
public sealed class MemberSpecification : Specification<Member>
{
    public MemberSpecification()
    {
        Query.Include(member => member.Team);
    }

    public MemberSpecification(Guid memberId)
    {
        Query
            .Where(member => member.Id.Equals(memberId))
            .Include(member => member.Team);
    }
    public MemberSpecification(IEnumerable<string> ids)
    {
        Query.Where(member => ids.Contains(member.Id.ToString()));
    }
}
