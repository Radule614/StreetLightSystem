using Common.Gprc.Exceptions;

namespace Pole.API.Domain.Exceptions;

/// <summary>
/// PoleNotFoundException used by pole micro service to express that the pole entity has not been found.
/// </summary>
public class PoleNotFoundException : EntityNotFoundException
{
    /// <param name="poleId">Id of the pole entity that has not been found.</param>
    public PoleNotFoundException(Guid poleId) :
        base("Pole", poleId)
    { }
}