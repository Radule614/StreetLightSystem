using Ardalis.Specification.EntityFrameworkCore;
using Notification.API.Domain.Entities;

namespace Notification.API.Infrastructure.Data;

/// <summary>
/// Generic repository class used for executing database operations and applying specifications.
/// It's registered as a Scoped service in Program.cs
/// </summary>
public class NotificationRepository : RepositoryBase<NotificationEntity>
{
    private readonly NotificationContext _dbContext;

    public NotificationRepository(NotificationContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}

