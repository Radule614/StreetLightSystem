using Ardalis.Specification;
using User.API.Domain.Entities;

namespace User.API.Domain.Specifications;

/// <summary>
/// User specification class used for user queries.
/// </summary>
public sealed class UserSpecification : Specification<UserEntity>
{
    public UserSpecification()
    {
        Query.Include(user => user.Roles);
    }

    public UserSpecification(Guid id)
    {
        Query.Where(user => user.Id.Equals(id)).Include(user => user.Roles);
    }

    public UserSpecification(ICollection<string> ids)
    {
        Query.Where(user => ids.Contains(user.Id.ToString()));
    }
}
