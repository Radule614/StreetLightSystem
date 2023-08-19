using FluentValidation;
using User.API.Domain.Entities;

namespace User.API.Domain.Validators;

/// <summary>
/// Validator class that contains validation rules for user entity.
/// </summary>
public class UserValidator : AbstractValidator<UserEntity>
{
    public UserValidator()
    {
        RuleFor(user => user.Email).EmailAddress();
        RuleFor(user => user.FirstName.Length).InclusiveBetween(3, 64);
        RuleFor(user => user.LastName.Length).InclusiveBetween(3, 64);
    }
}