using Common.Gprc.Exceptions;

namespace User.API.Domain.Exceptions;

/// <summary>
/// RoleNotFoundException used by user micro service to express that the role entity has not been found.
/// </summary>
public class RoleNotFoundException : EntityNotFoundException
{
    /// <param name="roleId">Id of the role entity that has not been found.</param>
    public RoleNotFoundException(Guid roleId) :
        base("Role", roleId)
    { }

    /// <param name="propertyName">Property name of the role entity that has not been found.</param>
    /// <param name="propertyValue">Property value.</param>
    public RoleNotFoundException(string propertyName, string propertyValue) :
        base("Role", propertyName, propertyValue)
    { }
}