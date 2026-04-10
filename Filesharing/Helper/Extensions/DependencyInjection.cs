using Filesharing.Services;
using Filesharing.Helper.Mail;
using Filesharing.Validation;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddIdentityConfig(configuration);
        services.AddAuthProviders(configuration);
        services.AddCustomServices();
        services.AddValidation();
        services.AddWebLocalization();

        return services;
    }

    private static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDBContext>(option =>
            option.UseSqlite(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDBContext).Assembly.FullName)));
    }

    private static void AddIdentityConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            configuration.GetSection("IdentityOptions:Password").Bind(options.Password);
            configuration.GetSection("IdentityOptions:Lockout").Bind(options.Lockout);
        }).AddEntityFrameworkStores<ApplicationDBContext>();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromDays(30);
            options.SlidingExpiration = true;
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.Name = ".Filesharing.Auth";
        });
    }

    private static void AddAuthProviders(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.ClientId = configuration["Authentication:Google:ClientId"] ?? "";
                options.ClientSecret = configuration["Authentication:Google:ClientSecret"] ?? "";
            })
            .AddFacebook(options =>
            {
                options.AppId = configuration["Authentication:Facebook:AppId"] ?? "";
                options.AppSecret = configuration["Authentication:Facebook:AppSecret"] ?? "";
            });
    }

    private static void AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped<IMailServices, MailServices>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IUploadService, UploadService>();
    }

    private static void AddValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<LoginValidator>();
    }

    private static void AddWebLocalization(this IServiceCollection services)
    {
        services.AddLocalization();
        services.AddControllersWithViews()
            .AddRazorRuntimeCompilation()
            .AddViewLocalization(op => op.ResourcesPath = "Helper/Resources");
    }
}
