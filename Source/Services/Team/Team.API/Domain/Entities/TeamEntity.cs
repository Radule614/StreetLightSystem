using Common.Gprc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Team.API.Domain.Validators;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Team.API.Domain.Entities;

[Table("Team")]
public class TeamEntity
{
    /// <summary>
    /// Team id used as a primary key in the database.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Name of the team.
    /// </summary>
    [MaxLength(128)]
    public string Name { get; set; }
    /// <summary>
    /// List of team members.
    /// </summary>
    public List<Member> Members { get; set; } = new();

    public TeamEntity()
    {
        Name = string.Empty;
    }

    /// <summary>
    /// Method for validating entity data.
    /// </summary>
    /// <param name="validationExceptionBuilder">Validation exception builder that will contain error messages.</param>
    public void ValidateData(ValidationExceptionBuilder validationExceptionBuilder)
    {
        TeamValidator validator = new();
        ValidationResult result = validator.Validate(this);
        if (!result.IsValid)
        {
            validationExceptionBuilder.AddFluentErrors(result.Errors);
        }
    }
}
