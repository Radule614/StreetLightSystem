using Ardalis.Specification.EntityFrameworkCore;

namespace User.API.Infrastructure.Data;

/// <summary>
/// Repository class used for executing database operations and applying specifications.
/// It's registered as a Scoped service in Program.cs
/// </summary>
public class Repository<T> : RepositoryBase<T> where T : class
{
    public Repository(UserContext dbContext) : base(dbContext) { }
}
