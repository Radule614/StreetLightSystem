using Ardalis.Specification;
using User.API.Domain.Entities;

namespace User.API.Domain.Specifications;

/// <summary>
/// User specification class used for email user queries.
/// </summary>
public sealed class EmailSpecification : Specification<UserEntity>
{
    public EmailSpecification(string email)
    {
        Query
            .Where(user => user.Email.Equals(email))
            .Include(user => user.Roles)
            .ThenInclude(role => role.Permissions);
    }
}
