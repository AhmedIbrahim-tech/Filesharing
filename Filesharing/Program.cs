var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation().AddViewLocalization( op =>
{
    op.ResourcesPath = "Helper/Resources";
});

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


builder.Services.AddTransient<IMailServices, MailServices>();

// Add Localization

builder.Services.AddLocalization();


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
var SupportedLecture = new[] { "ar-SA" , "en-US" };
app.UseRequestLocalization( reg =>  // "Ar-SA" , "en-US" , "fr-FR"
{
    reg.AddSupportedUICultures(SupportedLecture);
    reg.AddSupportedCultures(SupportedLecture);
    reg.SetDefaultCulture("en-US");
}
    ); 

// For Account ( Register & Login ) 
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
