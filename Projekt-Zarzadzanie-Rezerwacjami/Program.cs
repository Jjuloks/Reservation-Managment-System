using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Projekt_Zarzadzanie_Rezerwacjami.Data;
using Projekt_Zarzadzanie_Rezerwacjami.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Projekt_Zarzadzanie_RezerwacjamiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Projekt_Zarzadzanie_RezerwacjamiContext")
        ?? throw new InvalidOperationException("Connection string not found")));

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IPasswordHasher<Uzytkownik>, PasswordHasher<Uzytkownik>>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

app.UseStaticFiles();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
    await UserPasswordDataMigration.InitializeAsync(services, builder.Configuration);
}
app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
