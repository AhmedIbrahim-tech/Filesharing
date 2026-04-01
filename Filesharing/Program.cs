var builder = WebApplication.CreateBuilder(args);

// Add infrastructure services (DB, Auth, Identity, Services, etc.)
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Localization Middleware
var supportedCultures = new[] { "ar-SA", "en-US" };
app.UseRequestLocalization(options =>
{
    options.AddSupportedUICultures(supportedCultures)
           .AddSupportedCultures(supportedCultures)
           .SetDefaultCulture("en-US");
});

app.UseAuthentication();
app.UseAuthorization();

// Auto-run migrations and seeds on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
    db.Database.Migrate();

    // Call Seeder
    await Filesharing.Helper.DataSeeder.SeedAsync(scope.ServiceProvider);
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
