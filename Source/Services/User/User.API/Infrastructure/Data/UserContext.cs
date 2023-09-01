using Common;
using Microsoft.EntityFrameworkCore;
using User.API.Domain.Entities;

namespace User.API.Infrastructure.Data;

/// <summary>
/// DbContext class used for the database configuration.
/// </summary>
public class UserContext : DbContext
{
    public UserContext() { }
    public UserContext(DbContextOptions<UserContext> options)
        : base(options) { }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        var hashedPassword = PasswordValidator.HashPassword("12341234abc");
        var id1 = Guid.Parse("4e7e4d2a-bdad-4e2c-9103-fb89d08c70f2");
        var id2 = Guid.Parse("1af3c2c3-3bf2-47cf-908b-6dc20fb678b2");
        var id3 = Guid.Parse("93ce4690-1c6a-41df-b701-1aa7c6ecaa25");
        var id4 = Guid.Parse("eeca6b6e-f5e8-4ac6-b9a5-7a8b21d0a8a3");
        List<UserEntity> users = new()
        {
            new UserEntity
            {
                Id = id1, Email = "stojanovicrade614@gmail.com", FirstName = "Rade", LastName = "Stojanovic", Password = hashedPassword
            },
            new UserEntity
            {
                Id = id2, Email = "marko@gmail.com", FirstName = "Marko", LastName = "Markovic", Password = hashedPassword
            },
            new UserEntity
            {
                Id = id3, Email = "ivan@gmail.com", FirstName = "Ivan", LastName = "Ivanovic", Password = hashedPassword
            },
            new UserEntity
            {
                Id = id4, Email = "milan@gmail.com", FirstName = "Milan", LastName = "Milanovic", Password = hashedPassword
            }
        };
        Dictionary<string, Role> roles = InitRoles(builder);

        builder
            .Entity<UserEntity>()
            .HasData(
                users
            );
        builder.Entity<UserEntity>()
            .HasMany(user => user.Roles)
            .WithMany(role => role.Users)
            .UsingEntity<Dictionary<string, object>>(
                "UserRole",
                right => right.HasOne<Role>().WithMany().HasForeignKey("RoleId"),
                left => left.HasOne<UserEntity>().WithMany().HasForeignKey("UserId"),
                joinEntity =>
                {
                    joinEntity.HasKey("UserId", "RoleId");
                    joinEntity.HasData(
                        new { UserId = users[0].Id, RoleId = roles["User"].Id },
                        new { UserId = users[0].Id, RoleId = roles["Admin"].Id },
                        new { UserId = users[1].Id, RoleId = roles["User"].Id },
                        new { UserId = users[2].Id, RoleId = roles["User"].Id },
                        new { UserId = users[3].Id, RoleId = roles["User"].Id });
                });

        base.OnModelCreating(builder);
    }

    private Dictionary<string, Role> InitRoles(ModelBuilder builder)
    {
        var roles = new Dictionary<string, Role>
        {
            { "Admin", new Role { Id = Guid.NewGuid(), Name = "Admin" } },
            { "User", new Role { Id = Guid.NewGuid(), Name = "User" } }
        };

        var permissions = new List<Permission>
        {
            new() { Id = Guid.NewGuid(), Name = "ReadUsers" },
            new() { Id = Guid.NewGuid(), Name = "ModifyUsers" },
            new() { Id = Guid.NewGuid(), Name = "ReadPoles" },
            new() { Id = Guid.NewGuid(), Name = "ModifyPoles" },
            new() { Id = Guid.NewGuid(), Name = "ReadTeams" },
            new() { Id = Guid.NewGuid(), Name = "ModifyTeams" },
            new() { Id = Guid.NewGuid(), Name = "ReadRepairHistory" },
            new() { Id = Guid.NewGuid(), Name = "PoleRepair" }
        };

        builder
            .Entity<Role>()
            .HasData(roles["Admin"], roles["User"]);
        builder
            .Entity<Permission>()
            .HasData(permissions);

        builder.Entity<Role>()
            .HasMany(role => role.Permissions)
            .WithMany(permission => permission.Roles)
            .UsingEntity<Dictionary<string, object>>(
                "RolePermission",
                right => right.HasOne<Permission>().WithMany().HasForeignKey("PermissionId"),
                left => left.HasOne<Role>().WithMany().HasForeignKey("RoleId"),
                joinEntity =>
                {
                    joinEntity.HasKey("RoleId", "PermissionId");
                    foreach (var permission in permissions)
                    {
                        joinEntity.HasData(
                            new { RoleId = roles["Admin"].Id, PermissionId = permission.Id });
                    }

                    joinEntity.HasData(
                        new { RoleId = roles["User"].Id, PermissionId = permissions[2].Id },
                        new { RoleId = roles["User"].Id, PermissionId = permissions[6].Id },
                        new { RoleId = roles["User"].Id, PermissionId = permissions[7].Id });
                });
        return roles;
    }
}
