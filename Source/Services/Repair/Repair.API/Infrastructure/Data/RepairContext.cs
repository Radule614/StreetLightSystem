using Microsoft.EntityFrameworkCore;
using Repair.API.Domain.Entities;

namespace Repair.API.Infrastructure.Data;

/// <summary>
/// DbContext class used for the database configuration.
/// </summary>
public class RepairContext : DbContext
{
    public RepairContext() { }
    public RepairContext(DbContextOptions<RepairContext> options)
        : base(options) { }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<RepairEntity>();
        base.OnModelCreating(builder);
    }
}
