using FluentValidation;
using Team.API.Domain.Entities;

namespace Team.API.Domain.Validators;

/// <summary>
/// Validator class that contains validation rules for team entity.
/// </summary>
public class TeamValidator : AbstractValidator<TeamEntity>
{
    public TeamValidator()
    {
        RuleFor(team => team.Name.Length).InclusiveBetween(3, 64);
    }
}
