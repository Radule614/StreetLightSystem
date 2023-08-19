namespace User.API.Domain.Entities;

/// <summary>
/// Permission entity used to model role permissions in the database through Entity framework.
/// </summary>
public class Permission
{
    /// <summary>
    /// Permission id used as a primary key in the database.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Name of the permission
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// List of Roles that have this permission
    /// </summary>
    public List<Role> Roles { get; } = new();
    public Permission()
    {
        Name = string.Empty;
    }
}
