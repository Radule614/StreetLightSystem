using Ardalis.Specification;
using User.API.Domain.Entities;

namespace User.API.Domain.Specifications;

/// <summary>
/// Role specification class used for role queries.
/// </summary>
public sealed class RoleSpecification : Specification<Role>
{
    public RoleSpecification(string roleName)
    {
        Query.Where(role => role.Name.Equals(roleName));
    }

    public RoleSpecification(IEnumerable<Guid> ids)
    {
        Query.Where(role => ids.Contains(role.Id));
    }
}
