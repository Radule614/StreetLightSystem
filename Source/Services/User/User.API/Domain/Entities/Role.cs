namespace User.API.Domain.Entities;

/// <summary>
/// Role entity used to model role data in the database through Entity framework.
/// </summary>
public class Role
{
    /// <summary>
    /// Role id used as a primary key in the database.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Name of the role
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// List of permissions
    /// </summary>
    public List<Permission> Permissions { get; } = new();
    /// <summary>
    /// List of users that have this role. Many to many relationship.
    /// </summary>
    public List<UserEntity> Users { get; } = new();

    public Role()
    {
        Name = string.Empty;
    }
}
