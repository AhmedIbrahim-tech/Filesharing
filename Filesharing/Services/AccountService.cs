using System.Security.Claims;
using Filesharing.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Filesharing.Services;

public interface IAccountService
{
    Task<Microsoft.AspNetCore.Identity.SignInResult> LoginAsync(LoginViewModel model);
    Task<(IdentityResult Result, IdentityUser? User)> RegisterAsync(RegisterViewModel model);
    Task LogoutAsync();
    Task<IdentityResult> ChangePasswordAsync(ClaimsPrincipal userPrincipal, string currentPassword, string newPassword);
    Task<Microsoft.AspNetCore.Identity.SignInResult> ExternalLoginSignInAsync(ExternalLoginInfo info);
    Task<(IdentityResult Result, IdentityUser? User)> CreateExternalUserAsync(ExternalLoginInfo info);
}

public class AccountService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) : IAccountService
{
    public async Task<Microsoft.AspNetCore.Identity.SignInResult> LoginAsync(LoginViewModel model)
    {
        return await signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, lockoutOnFailure: false);
    }

    public async Task<(IdentityResult Result, IdentityUser? User)> RegisterAsync(RegisterViewModel model)
    {
        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        var result = await userManager.CreateAsync(user, model.Password);
        
        if (result.Succeeded)
            await signInManager.SignInAsync(user, isPersistent: true);

        return (result, user);
    }

    public async Task LogoutAsync()
    {
        await signInManager.SignOutAsync();
    }

    public async Task<IdentityResult> ChangePasswordAsync(ClaimsPrincipal userPrincipal, string currentPassword, string newPassword)
    {
        var user = await userManager.GetUserAsync(userPrincipal);
        if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });

        return await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
    }

    public async Task<Microsoft.AspNetCore.Identity.SignInResult> ExternalLoginSignInAsync(ExternalLoginInfo info)
    {
        return await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true);
    }

    public async Task<(IdentityResult Result, IdentityUser? User)> CreateExternalUserAsync(ExternalLoginInfo info)
    {
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
            return (IdentityResult.Failed(new IdentityError { Description = "Email is missing from the login provider." }), null);

        var user = new IdentityUser { Email = email, UserName = email };
        var result = await userManager.CreateAsync(user);
        
        if (result.Succeeded)
        {
            var addLoginResult = await userManager.AddLoginAsync(user, info);
            if (!addLoginResult.Succeeded) return (addLoginResult, user);

            await signInManager.SignInAsync(user, isPersistent: true, info.LoginProvider);
        }

        return (result, user);
    }
}
