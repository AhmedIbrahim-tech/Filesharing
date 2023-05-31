using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Filesharing.Validation
{
    public class ContactValidator : AbstractValidator<Contact>
    {
        public ContactValidator(IStringLocalizer<Contact> localizer)
        {
            
        }
    }
}
