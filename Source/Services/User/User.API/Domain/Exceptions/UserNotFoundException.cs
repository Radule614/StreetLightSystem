using Common.Gprc.Exceptions;

namespace User.API.Domain.Exceptions;

/// <summary>
/// UserNotFoundException used by user micro service to express that the user entity has not been found.
/// </summary>
public class UserNotFoundException : EntityNotFoundException
{
    /// <param name="userId">Id of the user entity that has not been found.</param>
    public UserNotFoundException(Guid userId) :
        base("User", userId)
    { }
    /// <param name="email">Email of the user entity that has not been found.</param>
    public UserNotFoundException(string email) :
        base("User", nameof(email), email)
    { }
}