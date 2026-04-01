using Filesharing.Helper.Resources;
using FluentValidation;

namespace Filesharing.Validation;

public class LoginValidator : AbstractValidator<LoginViewModel>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(x => string.Format(SharedResource.Required, nameof(x.Email)))
            .EmailAddress().WithMessage(SharedResource.EmailAddress);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(x => string.Format(SharedResource.Required, nameof(x.Password)));
    }
}

public class RegisterValidator : AbstractValidator<RegisterViewModel>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(x => string.Format(SharedResource.Required, nameof(x.Email)))
            .EmailAddress().WithMessage(SharedResource.EmailAddress);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(x => string.Format(SharedResource.Required, nameof(x.Password)))
            .MinimumLength(6);

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage(x => string.Format(SharedResource.Required, nameof(x.ConfirmPassword)))
            .Equal(x => x.Password).WithMessage("Passwords do not match");
    }
}

public class ChangePasswordValidator : AbstractValidator<ChangePasswordViewModel>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required")
            .MinimumLength(6);

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required")
            .Equal(x => x.NewPassword).WithMessage("Passwords do not match");
    }
}
