using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Filesharing.Models;
using Filesharing.Services;

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
        {
            Response.Cookies.Append("fs_msg", "Welcome back! Login successful.", new CookieOptions { Path = "/" });
            Response.Cookies.Append("fs_type", "success", new CookieOptions { Path = "/" });
            return RedirectToLocal(returnUrl);
        }

        Response.Cookies.Append("fs_msg", "Invalid email or password.", new CookieOptions { Path = "/" });
        Response.Cookies.Append("fs_type", "danger", new CookieOptions { Path = "/" });
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
        {
            Response.Cookies.Append("fs_msg", "Account created successfully! Welcome to the family.", new CookieOptions { Path = "/" });
            Response.Cookies.Append("fs_type", "success", new CookieOptions { Path = "/" });
            return RedirectToLocal(returnUrl);
        }

        Response.Cookies.Append("fs_msg", "Registration failed. Please check the requirements.", new CookieOptions { Path = "/" });
        Response.Cookies.Append("fs_type", "danger", new CookieOptions { Path = "/" });
        AddErrors(result);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> LogOut()
    {
        await accountService.LogoutAsync();
        Response.Cookies.Append("fs_msg", "Successfully logged out. See you soon!", new CookieOptions { Path = "/" });
        Response.Cookies.Append("fs_type", "success", new CookieOptions { Path = "/" });
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
        {
            Response.Cookies.Append("fs_msg", "External login failed.", new CookieOptions { Path = "/" });
            Response.Cookies.Append("fs_type", "danger", new CookieOptions { Path = "/" });
            return RedirectToAction(nameof(Login));
        }

        var result = await accountService.ExternalLoginSignInAsync(info);

        if (result.Succeeded)
        {
            Response.Cookies.Append("fs_msg", $"Logged in with {info.LoginProvider}.", new CookieOptions { Path = "/" });
            Response.Cookies.Append("fs_type", "success", new CookieOptions { Path = "/" });
            return RedirectToAction("Index", "Home");
        }

        var createResult = await accountService.CreateExternalUserAsync(info);
        if (createResult.Result.Succeeded)
        {
            Response.Cookies.Append("fs_msg", "External account linked and created successfully!", new CookieOptions { Path = "/" });
            Response.Cookies.Append("fs_type", "success", new CookieOptions { Path = "/" });
            return RedirectToAction("Index", "Home");
        }

        Response.Cookies.Append("fs_msg", "External login creation failed.", new CookieOptions { Path = "/" });
        Response.Cookies.Append("fs_type", "danger", new CookieOptions { Path = "/" });
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
        {
            Response.Cookies.Append("fs_msg", "Your password has been changed successfully!", new CookieOptions { Path = "/" });
            Response.Cookies.Append("fs_type", "success", new CookieOptions { Path = "/" });
            return RedirectToAction("Index", "Upload");
        }

        Response.Cookies.Append("fs_msg", "Failed to change password.", new CookieOptions { Path = "/" });
        Response.Cookies.Append("fs_type", "danger", new CookieOptions { Path = "/" });
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
