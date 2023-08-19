using System.ComponentModel.DataAnnotations.Schema;
using Common.Gprc;
using FluentValidation.Results;
using Pole.API.Domain.Validators;

namespace Pole.API.Domain.Entities;

/// <summary>
/// Pole entity used to model pole data in the database through Entity framework.
/// </summary>
[Table("Pole")]
public class PoleEntity
{
    /// <summary>
    /// Pole id used as primary key in a database
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Represents pole current status
    /// </summary>
    public PoleStatus Status { get; set; }

    /// <summary>
    /// Number that represents latitude in world coordinates
    /// </summary>
    public double Latitude { get; set; }
    /// <summary>
    /// Number that represents longitude in world coordinates
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Method for validating entity data.
    /// </summary>
    /// <param name="validationExceptionBuilder">Validation exception builder that will contain error messages.</param>
    public void ValidateData(ValidationExceptionBuilder validationExceptionBuilder)
    {
        PoleValidator validator = new();
        ValidationResult result = validator.Validate(this);
        if (!result.IsValid)
        {
            validationExceptionBuilder.AddFluentErrors(result.Errors);
        }
    }
}