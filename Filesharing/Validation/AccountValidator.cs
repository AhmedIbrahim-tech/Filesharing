using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Filesharing.Validation
{
    public class LoginValidator : AbstractValidator<LoginViewModel>
    {
        public LoginValidator(IStringLocalizer<LoginViewModel> localizer)
        {
            RuleFor(L => L.Email).EmailAddress().WithMessage("Invalid email format.").NotEmpty().WithMessage("Email is required");
            RuleFor(L => L.Password).NotEmpty().WithMessage("Password is required");
        }
    }

    public class RegisterValidator : AbstractValidator<RegistertViewModel>
    {
        public RegisterValidator(IStringLocalizer<RegistertViewModel> localizer)
        {
            RuleFor(R => R.Email).EmailAddress().WithMessage("Invalid email format.").NotEmpty().WithMessage("Email is required");
            RuleFor(R => R.Password).NotEmpty().WithMessage("Password is required");
            // RuleFor(R => R.ComfirmPassword);
        }
    }
}
