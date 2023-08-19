using Ardalis.Specification.EntityFrameworkCore;
using Pole.API.Domain.Entities;

namespace Pole.API.Infrastructure.Data;

/// <summary>
/// Generic repository class used for executing database operations and applying specifications.
/// It's registered as a Scoped service in Program.cs
/// </summary>
public class PoleRepository : RepositoryBase<PoleEntity>
{
    private readonly PoleContext _dbContext;

    public PoleRepository(PoleContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}