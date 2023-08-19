using Common.Gprc.Exceptions;

namespace Team.API.Domain.Exceptions;

/// <summary>
/// TeamNotFoundException used by team micro service to express that the team entity has not been found.
/// </summary>
public class TeamNotFoundException : EntityNotFoundException
{
    /// <param name="teamId">Id of the team entity that has not been found.</param>
    public TeamNotFoundException(Guid teamId) :
        base("Team", teamId)
    { }
}