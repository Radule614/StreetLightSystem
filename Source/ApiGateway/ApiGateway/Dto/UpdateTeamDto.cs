namespace ApiGateway.Dto;

public class UpdateTeamDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public IEnumerable<string> MemberIds { get; set; } = new List<string>();
}
