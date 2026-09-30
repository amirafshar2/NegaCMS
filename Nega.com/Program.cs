using BE;
using Microsoft.AspNetCore.Identity;
using BLL;
using DAL.Context;
using DAL.Seed;
using System.Globalization;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Negacom.Infrastructure;

// German UI texts; parsing of numbers/dates in forms stays culture-independent
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("de-DE");

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// ---------- Database: SQLite file inside the project (App_Data/nega.db) ----------
var cs = new SqliteConnectionStringBuilder(builder.Configuration.GetConnectionString("Default") ?? "Data Source=App_Data/nega.db");
if (!Path.IsPathRooted(cs.DataSource)) cs.DataSource = Path.Combine(builder.Environment.ContentRootPath, cs.DataSource);
Directory.CreateDirectory(Path.GetDirectoryName(cs.DataSource)!);
services.AddDbContext<DB>(o => o.UseSqlite(cs.ToString()));

// ---------- Identity ----------
services.AddIdentity<User, UserRole>(o =>
    {
        o.Password.RequireUppercase = false;
        o.Password.RequireNonAlphanumeric = false;
        o.Password.RequiredLength = 6;
        o.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<DB>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<GermanIdentityErrorDescriber>();

services.ConfigureApplicationCookie(o =>
{
    o.Cookie.Name = "nega_auth";
    o.LoginPath = "/admin/account/login";
    o.AccessDeniedPath = "/admin/account/denied";
    o.ExpireTimeSpan = TimeSpan.FromHours(2);
    o.SlidingExpiration = true;
});

// ---------- Layers (DAL + BLL) and UI services ----------
services.AddNegaLayers();
services.Configure<DemoOptions>(builder.Configuration.GetSection("Demo"));
services.AddScoped<IFileStorage, FileStorage>();
services.AddHostedService<DemoResetService>();
services.AddRouting(o => o.LowercaseUrls = true);
services.AddControllersWithViews(o =>
{
    // German texts for model binding errors
    var m = o.ModelBindingMessageProvider;
    m.SetValueMustNotBeNullAccessor(_ => "Bitte einen Wert auswählen bzw. eingeben.");
    m.SetAttemptedValueIsInvalidAccessor((v, f) => $"Der Wert „{v}“ ist ungültig.");
    m.SetValueIsInvalidAccessor(v => $"Der Wert „{v}“ ist ungültig.");
    m.SetMissingBindRequiredValueAccessor(f => $"Das Feld „{f}“ fehlt.");
    m.SetValueMustBeANumberAccessor(f => $"Das Feld „{f}“ muss eine Zahl sein.");
    m.SetNonPropertyAttemptedValueIsInvalidAccessor(v => $"Der Wert „{v}“ ist ungültig.");
    m.SetNonPropertyValueMustBeANumberAccessor(() => "Der Wert muss eine Zahl sein.");
    m.SetUnknownValueIsInvalidAccessor(f => $"Der eingegebene Wert für „{f}“ ist ungültig.");
});

var app = builder.Build();

// ---------- Create + seed the database on startup ----------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DB>();
    if (builder.Configuration.GetValue("Demo:Enabled", true)) DbSeeder.Reset(db, recreateSchema: true); // demo: always start with fresh data
    else DbSeeder.Seed(db);                                                       // otherwise: create + seed only if empty
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/fehler");
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/status/{0}");
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseMiddleware<DemoAutoLoginMiddleware>();
app.UseAuthorization();

app.MapControllerRoute("admin", "admin/{controller=Dashboard}/{action=Index}/{id:int?}", new { area = "Admin" }, new { area = "Admin" });
app.MapControllerRoute("blogDetail", "blog/{id:int}/{slug?}", new { controller = "Blog", action = "Detail" });
app.MapControllerRoute("privacy", "datenschutz", new { controller = "Home", action = "Privacy" });
app.MapControllerRoute("imprint", "impressum", new { controller = "Home", action = "Imprint" });
app.MapControllerRoute("status", "status/{code:int}", new { controller = "Home", action = "Status" });
app.MapControllerRoute("error", "fehler", new { controller = "Home", action = "Error" });
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id:int?}");
app.MapGet("/health", () => Results.Ok("ok")); // for the hosting health check (does not count as a visit)

app.Run();
