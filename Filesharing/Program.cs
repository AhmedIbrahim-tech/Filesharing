using Filesharing.Services.Interface;
using Filesharing.Services.Repository;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation().AddViewLocalization(op =>
{
	op.ResourcesPath = "Helper/Resources";
});


#region Connection String

builder.Services.AddDbContext<ApplicationDBContext>(option =>
	option.UseSqlServer(
		builder.Configuration.GetConnectionString("DefaultConnection"),
			b => b.MigrationsAssembly(typeof(ApplicationDBContext).Assembly.FullName)));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
	options.Password.RequiredLength = 6;
	options.Password.RequireLowercase = true;
	options.Password.RequireUppercase = true;

}).AddEntityFrameworkStores<ApplicationDBContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = "/Account/Login";
	options.LogoutPath = "/Account/Logout";
	options.AccessDeniedPath = "/Account/AccessDenied";
	options.ExpireTimeSpan = TimeSpan.FromDays(30); // Long duration
	options.SlidingExpiration = true; // Refresh expiration on use
	options.Cookie.HttpOnly = true;
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Recommended
	options.Cookie.SameSite = SameSiteMode.Lax;
	options.Cookie.Name = ".Filesharing.Auth";
});

#endregion


#region Dependency injection

builder.Services.AddScoped<IMailServices, MailServices>();

builder.Services.AddScoped<IUploadService, UploadService>();

#endregion

// Add Localization
#region Localization

builder.Services.AddLocalization();

#endregion




// For Auth by Google And FaceBook
#region Auth by Google And FaceBook

builder.Services.AddAuthentication()
	.AddGoogle(options =>
	{
		options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
		options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
	})
	.AddFacebook(options =>
	{
		options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
		options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];

	});

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");


// Http >> Htpps
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// For Localization
#region Localization

var SupportedLecture = new[] { "ar-SA", "en-US" };
app.UseRequestLocalization(reg =>  // "Ar-SA" , "en-US" , "fr-FR"
{
	reg.AddSupportedUICultures(SupportedLecture);
	reg.AddSupportedCultures(SupportedLecture);
	reg.SetDefaultCulture("en-US");
});

#endregion

// For Account ( Register & Login ) 
app.UseAuthentication();
app.UseAuthorization();

#region Database.Migrate

using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
	db.Database.Migrate();
}

#endregion

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
