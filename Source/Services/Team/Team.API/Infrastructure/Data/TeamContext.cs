using Microsoft.EntityFrameworkCore;
using Team.API.Domain.Entities;

namespace Team.API.Infrastructure.Data;

public class TeamContext : DbContext
{
    public TeamContext() { }
    public TeamContext(DbContextOptions<TeamContext> options)
        : base(options) { }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        var id1 = Guid.Parse("1af3c2c3-3bf2-47cf-908b-6dc20fb678b2");
        var id2 = Guid.Parse("93ce4690-1c6a-41df-b701-1aa7c6ecaa25");
        TeamEntity team = new()
        {
            Id = Guid.NewGuid(),
            Name = "Team 1"
        };
        builder.Entity<TeamEntity>().HasData(team);
        List<Member> members = new()
        {
            new Member
            {
                Id = id1, FirstName = "Marko", LastName = "Uljarevic", TeamId = team.Id
            },
            new Member
            {
                Id = id2, FirstName = "Darko", LastName = "Selakovic", TeamId = team.Id
            }
        };

        builder.Entity<Member>().HasData(members);
        base.OnModelCreating(builder);
    }
}
