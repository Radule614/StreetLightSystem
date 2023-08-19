using Ardalis.Specification;
using Team.API.Domain.Entities;

namespace Team.API.Domain.Specifications;

/// <summary>
/// Team specification class used for team queries.
/// </summary>
public sealed class TeamSpecification : Specification<TeamEntity>
{
    public TeamSpecification()
    {
        Query.Include(team => team.Members);
    }

    public TeamSpecification(Guid teamId)
    {
        Query
            .Where(team => team.Id.Equals(teamId))
            .Include(team => team.Members);
    }

    public TeamSpecification(ICollection<string> ids)
    {
        Query
            .Where(team => ids.Contains(team.Id.ToString()))
            .Include(team => team.Members);
    }
}
