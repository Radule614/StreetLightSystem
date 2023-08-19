using System.ComponentModel.DataAnnotations;

namespace Team.API.Domain.Entities;

public class Member
{
    /// <summary>
    /// Member id used as a primary key in the database.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Firstname of the member.
    /// </summary>
    [MaxLength(64)]
    public string FirstName { get; set; }
    /// <summary>
    /// Lastname of the member.
    /// </summary>
    [MaxLength(64)]
    public string LastName { get; set; }
    public TeamEntity? Team { get; set; }
    public Guid? TeamId { get; set; }
    public Member()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
    }
}
