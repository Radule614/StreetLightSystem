using Ardalis.Specification;
using Notification.API.Domain.Entities;

namespace Notification.API.Domain.Specifications;

public sealed class UserUnsentSpecification : Specification<NotificationEntity>
{
    public UserUnsentSpecification(Guid userId)
    {
        Query.Where(entity => entity.ReceiverId.Equals(userId) && !entity.Received);
    }
}
