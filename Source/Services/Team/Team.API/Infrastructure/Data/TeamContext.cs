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
        var id3 = Guid.Parse("eeca6b6e-f5e8-4ac6-b9a5-7a8b21d0a8a3");
        TeamEntity team1 = new()
        {
            Id = Guid.NewGuid(),
            Name = "Team 1"
        };
        TeamEntity team2 = new()
        {
            Id = Guid.NewGuid(),
            Name = "Team 2"
        };
        builder.Entity<TeamEntity>().HasData(team1, team2);
        List<Member> members = new()
        {
            new Member
            {
                Id = id1, FirstName = "Marko", LastName = "Markovic", TeamId = team1.Id
            },
            new Member
            {
                Id = id2, FirstName = "Ivan", LastName = "Ivanovic", TeamId = team1.Id
            },
            new Member
            {
                Id = id3, FirstName = "Milan", LastName = "Milanovic", TeamId = team2.Id
            }
        };

        builder.Entity<Member>().HasData(members);
        base.OnModelCreating(builder);
    }
}
