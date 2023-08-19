using FluentValidation;
using Pole.API.Domain.Entities;

namespace Pole.API.Domain.Validators;

/// <summary>
/// Validator class that contains validation rules for pole entity.
/// </summary>
public class PoleValidator : AbstractValidator<PoleEntity>
{
    public PoleValidator()
    {
        RuleFor(pole => pole.Status).IsInEnum();
    }
}
