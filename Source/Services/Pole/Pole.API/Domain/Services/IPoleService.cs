using Pole.API.Domain.Entities;

namespace Pole.API.Domain.Services;

public interface IPoleService : IDisposable
{
    /// <summary>
    /// Method for creating a pole entity in the database. It has all the necessary validation.
    /// </summary>
    /// <param name="latitude">Latitude of pole's world position</param>
    /// <param name="longitude">Longitude of pole's world position</param>
    /// <returns>Created pole entity</returns>
    Task<PoleEntity> Create(double latitude, double longitude);
    /// <summary>
    /// Method for updating existing pole entity.
    /// </summary>
    /// <param name="poleId">Id of the pole that's to be updated</param>
    /// <param name="status">New pole status</param>
    /// <param name="latitude">New latitude of pole's world position</param>
    /// <param name="longitude">New longitude of pole's world position</param>
    /// <returns>Updated pole entity</returns>
    Task<PoleEntity> Update(Guid poleId, PoleStatus status, double latitude, double longitude);

    /// <summary>
    /// Method for updating pole status
    /// </summary>
    /// <param name="poleId">Id of the pole that's to be updated</param>
    /// <param name="status">New pole status</param>
    /// <returns>Updated pole entity</returns>
    Task<PoleEntity> UpdateStatus(Guid poleId, PoleStatus status);
}
