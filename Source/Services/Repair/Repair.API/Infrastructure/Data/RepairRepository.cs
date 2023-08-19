using Ardalis.Specification.EntityFrameworkCore;
using Repair.API.Domain.Entities;

namespace Repair.API.Infrastructure.Data;

/// <summary>
/// Repository class used for executing database operations and applying specifications.
/// It's registered as a Scoped service in Program.cs
/// </summary>
public class RepairRepository : RepositoryBase<RepairEntity>
{
    private readonly RepairContext _dbContext;

    public RepairRepository(RepairContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}