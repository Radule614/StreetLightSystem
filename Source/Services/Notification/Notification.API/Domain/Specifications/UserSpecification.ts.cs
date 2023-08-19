using Ardalis.Specification;
using Notification.API.Domain.Entities;

namespace Notification.API.Domain.Specifications;

public sealed class UserSpecification : Specification<NotificationEntity>
{
    public UserSpecification(Guid userId)
    {
        Query.Where(entity => entity.ReceiverId.Equals(userId));
    }
}
