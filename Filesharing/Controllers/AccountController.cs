namespace Filesharing.Controllers;

public class AccountController(IAccountService accountService, SignInManager<IdentityUser> signInManager) : Controller
{
    #region Authentication Actions

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        returnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid)
            return View(model);

        var result = await accountService.LoginAsync(model);

        if (result.Succeeded)
            return RedirectToLocal(returnUrl);

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    [HttpGet]
    public IActionResult Register(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        returnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid)
            return View(model);

        var (result, user) = await accountService.RegisterAsync(model);

        if (result.Succeeded)
            return RedirectToLocal(returnUrl);

        AddErrors(result);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> LogOut()
    {
        await accountService.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }

    #endregion

    #region External Authentication

    public IActionResult ExternalLogin(string provider)
    {
        var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, "/Account/ExternalResponse");
        return Challenge(properties, provider);
    }

    public async Task<IActionResult> ExternalResponse()
    {
        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return RedirectToAction(nameof(Login));

        var result = await accountService.ExternalLoginSignInAsync(info);

        if (result.Succeeded)
            return RedirectToAction("Index", "Home");

        var createResult = await accountService.CreateExternalUserAsync(info);
        if (createResult.Result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }

        AddErrors(createResult.Result);
        return RedirectToAction(nameof(Login));
    }

    #endregion

    #region Account Management

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await accountService.ChangePasswordAsync(User, model.CurrentPassword, model.NewPassword);

        if (result.Succeeded)
            return RedirectToAction("Index", "Upload");

        AddErrors(result);
        return View(model);
    }

    #endregion

    #region Helper Methods

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);
    }

    private IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
        
        return RedirectToAction("Index", "Upload");
    }

    #endregion
}
