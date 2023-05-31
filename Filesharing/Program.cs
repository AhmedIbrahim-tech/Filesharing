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

#endregion

#region Dependency injection

builder.Services.AddTransient<IMailServices, MailServices>();

builder.Services.AddTransient<IUploadService, UploadService>();

#endregion

// Add Localization
builder.Services.AddLocalization();

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// For Auth by Google And FaceBook
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}


// Http >> Htpps
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// For Localization
var SupportedLecture = new[] { "ar-SA", "en-US" };
app.UseRequestLocalization(reg =>  // "Ar-SA" , "en-US" , "fr-FR"
{
	reg.AddSupportedUICultures(SupportedLecture);
	reg.AddSupportedCultures(SupportedLecture);
	reg.SetDefaultCulture("en-US");
});


// For Account ( Register & Login ) 
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
	db.Database.Migrate();
}

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
