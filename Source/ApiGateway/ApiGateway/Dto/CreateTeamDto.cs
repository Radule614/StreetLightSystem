namespace ApiGateway.Dto;

public class CreateTeamDto
{
    public string Name { get; set; } = string.Empty;
    public IEnumerable<string> MemberIds { get; set; } = new List<string>();
}