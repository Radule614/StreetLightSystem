using Team.API.Domain.Entities;

namespace Team.API.Domain.Services;

public interface ITeamService
{
    /// <summary>
    /// Method for creating an team entity in the database. It has all the necessary validation.
    /// </summary>
    /// <param name="name">Team's name.</param>
    /// <param name="memberIds">List of team member ids.</param>
    /// <returns>Created team entity.</returns>
    Task<TeamEntity> Create(string name, IEnumerable<string> memberIds);
    /// <summary>
    /// Method for updating existing team entity.
    /// </summary>
    /// <param name="teamId">Id of the team that's to be updated.</param>
    /// <param name="name">New name.</param>
    /// <param name="memberIds">List of team member ids.</param>
    /// <returns>Updated team entity.</returns>
    Task<TeamEntity> Update(Guid teamId, string name, IEnumerable<string> memberIds);
}
