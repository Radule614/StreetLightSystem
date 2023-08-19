namespace Common.Saga.User.Dto;
public class UserData
{
    public Guid Id { get; set; }
    public string? Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Password { get; set; } = string.Empty;
    public List<RoleData> Roles { get; set; } = new();
}
