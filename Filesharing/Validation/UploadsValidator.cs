using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Filesharing.Validation
{
    public class UploadsValidator : AbstractValidator<Upload>
    {
        public UploadsValidator(IStringLocalizer<Upload> localizer)
        {
            
        }
    }
}
