using FluentValidation;

namespace UserSvc.Model;

/// <summary>
/// Validation rules for User model
/// </summary>
public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.FirstName).NotNull().MaximumLength(128);
        RuleFor(x => x.LastName).MaximumLength(128);
        RuleFor(x => x.DateOfBirth).NotNull();
        RuleFor(x => x.Email).NotNull().EmailAddress();
        RuleFor(x => x.Phone).NotNull().Matches("\\d{10}").WithMessage("'{PropertyName}' must be 10 digits phone number");
        RuleFor(x => x.Age).GreaterThanOrEqualTo(18).WithMessage("User must be 18 years or older");
    }
}
