using Common.Gprc.Exceptions;

namespace Team.API.Domain.Exceptions;

/// <summary>
/// MemberNotFoundException used by team micro service to express that the member entity has not been found.
/// </summary>
public class MemberNotFoundException : EntityNotFoundException
{
    /// <param name="memberId">Id of the member entity that has not been found.</param>
    public MemberNotFoundException(Guid memberId) :
        base("Member", memberId)
    { }
}