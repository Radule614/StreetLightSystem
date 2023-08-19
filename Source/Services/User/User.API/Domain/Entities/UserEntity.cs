using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Gprc;
using User.API.Domain.Validators;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace User.API.Domain.Entities;

/// <summary>
/// User entity used to model user data in the database through Entity framework.
/// </summary>
[Table("User")]
public class UserEntity
{
    /// <summary>
    /// User id used as a primary key in the database.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Email of the user.
    /// </summary>
    [MaxLength(128)]
    public string Email { get; set; }
    /// <summary>
    /// Firstname of the user.
    /// </summary>
    [MaxLength(64)]
    public string FirstName { get; set; }
    /// <summary>
    /// Lastname of the user.
    /// </summary>
    [MaxLength(64)]
    public string LastName { get; set; }
    /// <summary>
    /// Password of the user. It represents hashed value by PBKDF2 method.
    /// </summary>
    [MaxLength(128)]
    public string Password { get; set; }
    /// <summary>
    /// List of user's roles. Many to many relationship.
    /// </summary>
    public List<Role> Roles { get; set; } = new();

    public UserEntity()
    {
        Email = string.Empty;
        FirstName = string.Empty;
        LastName = string.Empty;
        Password = string.Empty;
    }
    /// <summary>
    /// Method for validating entity data.
    /// </summary>
    /// <param name="validationExceptionBuilder">Validation exception builder that will contain error messages.</param>
    public void ValidateData(ValidationExceptionBuilder validationExceptionBuilder)
    {
        UserValidator validator = new();
        ValidationResult result = validator.Validate(this);
        if (!result.IsValid)
        {
            validationExceptionBuilder.AddFluentErrors(result.Errors);
        }
    }
}
