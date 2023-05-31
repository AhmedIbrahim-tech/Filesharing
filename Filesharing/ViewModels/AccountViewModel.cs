using System.ComponentModel;

namespace Filesharing.ViewModels
{
    public class LoginViewModel
    {
        [EmailAddress(ErrorMessageResourceName = "EmailAddress", ErrorMessageResourceType = typeof(SharedResource))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(SharedResource))]
        [Display(Name = "EmailLabel" , ResourceType = typeof(SharedResource))]
        public string? Email { get; set; }

		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(SharedResource))]
		[Display(Name = "PasswordLabel", ResourceType = typeof(SharedResource))]
		public string? Password { get; set; }
    }

    public class RegistertViewModel
    {
		[EmailAddress(ErrorMessageResourceName = "EmailAddress", ErrorMessageResourceType = typeof(SharedResource))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(SharedResource))]
		[Display(Name = "EmailLabel", ResourceType = typeof(SharedResource))]
		public string? Email { get; set; }

		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(SharedResource))]
		[Display(Name = "PasswordLabel", ResourceType = typeof(SharedResource))]
		public string? Password { get; set; }

        [Compare("Password")]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(SharedResource))]
		[Display(Name = "ComfirmPasswordLabel", ResourceType = typeof(SharedResource))]
		public string? ComfirmPassword { get; set; }
    }
}
