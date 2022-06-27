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
                var Result = await signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);
                if (Result.Succeeded)
                {
                    return RedirectToAction("Create", "Upload");
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
                    await signInManager.SignInAsync(user, false);
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

    }
}