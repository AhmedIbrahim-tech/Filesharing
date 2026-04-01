using Filesharing.ViewModels;
using FluentValidation;

namespace Filesharing.Validation;

public class UploadsValidator : AbstractValidator<InputFile>
{
    private readonly string[] _allowedExtensions = [".jpg", ".jpeg", ".png", ".pdf", ".txt", ".zip", ".rar", ".docx", ".xlsx"];
    private const long _maxFileSize = 5 * 1024 * 1024; // 5MB

    public UploadsValidator()
    {
        RuleFor(x => x.File)
            .NotNull().WithMessage("Please select a file to upload.")
            .Must(x => x.Length > 0).WithMessage("The selected file is empty.")
            .Must(x => x.Length <= _maxFileSize).WithMessage("File size exceeds 5MB limit.")
            .Must(file => 
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                return _allowedExtensions.Contains(extension);
            }).WithMessage("File type is not allowed.");
    }
}
