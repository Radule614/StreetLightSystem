using Microsoft.EntityFrameworkCore;
using Notification.API.Domain.Entities;

namespace Notification.API.Infrastructure.Data;

/// <summary>
/// DbContext class used for the database configuration.
/// </summary>
public class NotificationContext : DbContext
{
    public NotificationContext() { }
    public NotificationContext(DbContextOptions<NotificationContext> options)
        : base(options) { }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<NotificationEntity>();
        base.OnModelCreating(builder);
    }
}
