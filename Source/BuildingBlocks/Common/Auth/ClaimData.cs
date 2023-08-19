namespace Common.Auth;

public class ClaimData
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public IEnumerable<string> Permissions { get; set; } = new List<string>();
}
