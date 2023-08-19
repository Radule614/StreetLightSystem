using Ardalis.Specification.EntityFrameworkCore;

namespace Team.API.Infrastructure.Data;

public class Repository<T> : RepositoryBase<T> where T : class
{
    private readonly TeamContext _dbContext;

    public Repository(TeamContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
