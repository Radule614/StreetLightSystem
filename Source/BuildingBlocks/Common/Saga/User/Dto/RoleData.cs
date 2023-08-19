namespace Common.Saga.User.Dto;
public class RoleData
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public RoleData()
    {
        Name = string.Empty;
    }
}
