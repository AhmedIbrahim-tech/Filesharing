namespace Filesharing.ViewModels;

public class changePasswordViewModel
{
    [Required]
    public string CurrentPassword { get; set; }
    [Required]
    public string NewPassword { get; set; }
    [Compare("NewPassword")]
    public string ConfirmPassword { get; set; }

}
