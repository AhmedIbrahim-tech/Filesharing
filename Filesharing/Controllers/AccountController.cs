namespace Filesharing.Controllers
{
	public class AccountController : Controller
	{
		private readonly SignInManager<IdentityUser> signInManager;
		private readonly UserManager<IdentityUser> userManager;

		public AccountController(
			SignInManager<IdentityUser> signInManager,  // We Use with Login
			UserManager<IdentityUser> userManager       // We Use with Register
			)
		{
			this.signInManager = signInManager;
			this.userManager = userManager;
		}

		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				var Result = await signInManager.PasswordSignInAsync(model.Email, model.Password, true, false); //True = Remember me , False = Lock the Account after 5 Times.
				if (Result.Succeeded)
				{
					return RedirectToAction("Create", "Upload");
				}
				else
				{
					return View(model);
				}
			}

			return View(model);
		}

		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegistertViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new IdentityUser()
				{
					UserName = model.Email,
					Email = model.Email
				};
				var Result = await userManager.CreateAsync(user, model.Password);

				// We should Write This Code Because Don't Need Login Again
				if (Result.Succeeded)
				{
					await signInManager.SignInAsync(user, false); //false = Remember me 
					return RedirectToAction("Create", "Upload");
				}

				foreach (var error in Result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}
			return View(model);
		}


		[HttpGet]
		public async Task<IActionResult> LogOut()
		{
			await signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}


		#region External Login
		public IActionResult ExternalLogin(string Provider) // Provider = "Facebook" , "Google"
		{
			var Properties = signInManager.ConfigureExternalAuthenticationProperties(Provider, "/Account/ExternalResponse");

			return Challenge(Properties, Provider);
		}


		public async Task<IActionResult> ExternalResponse() // Provider = "Facebook" , "Google"
		{
			var logininfo = await signInManager.GetExternalLoginInfoAsync();
			if (logininfo == null)
			{
				return RedirectToAction("Login");
			}

			var loginResult = await signInManager.ExternalLoginSignInAsync(logininfo.LoginProvider, logininfo.ProviderKey, false);
			if (!loginResult.Succeeded)
			{
				// Create Local Account
				var email = logininfo.Principal.FindFirstValue(ClaimTypes.Email);
				var firstName = logininfo.Principal.FindFirstValue(ClaimTypes.GivenName);
				var lastName = logininfo.Principal.FindFirstValue(ClaimTypes.Surname);

				var UserToCreate = new IdentityUser
				{
					Email = email,
					UserName = email
				};

				var CreateResult = await userManager.CreateAsync(UserToCreate);
				if (CreateResult.Succeeded)
				{
					var ExLoginResult = await userManager.AddLoginAsync(UserToCreate, logininfo);
					if (ExLoginResult.Succeeded)
					{
						await signInManager.SignInAsync(UserToCreate, false, logininfo.LoginProvider);
						return RedirectToAction("Index", "Home");
					}
				}
				return RedirectToAction("Login");
			}

			return RedirectToAction("Index", "Home");
		}
		#endregion

	}
}