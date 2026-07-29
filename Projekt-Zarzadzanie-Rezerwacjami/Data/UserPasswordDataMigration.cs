using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Projekt_Zarzadzanie_Rezerwacjami.Models;

namespace Projekt_Zarzadzanie_Rezerwacjami.Data;

public static class UserPasswordDataMigration
{
    public static async Task InitializeAsync(
        IServiceProvider services,
        IConfiguration configuration)
    {
        var context = services.GetRequiredService<Projekt_Zarzadzanie_RezerwacjamiContext>();
        var passwordHasher = services.GetRequiredService<IPasswordHasher<Uzytkownik>>();
        var users = await context.Uzytkownik.ToListAsync();

        foreach (var user in users.Where(user => !IsIdentityPasswordHash(user.Password)))
        {
            user.Password = passwordHasher.HashPassword(user, user.Password);
        }

        if (users.Count == 0)
        {
            AddInitialAdmin(context, passwordHasher, configuration);
        }

        if (context.ChangeTracker.HasChanges())
        {
            await context.SaveChangesAsync();
        }
    }

    private static void AddInitialAdmin(
        Projekt_Zarzadzanie_RezerwacjamiContext context,
        IPasswordHasher<Uzytkownik> passwordHasher,
        IConfiguration configuration)
    {
        var login = configuration["InitialAdmin:Login"];
        var password = configuration["InitialAdmin:Password"];

        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        if (password.Length < 6)
        {
            throw new InvalidOperationException(
                "InitialAdmin:Password must contain at least 6 characters.");
        }

        var admin = new Uzytkownik
        {
            Login = login,
            Role = "admin"
        };

        admin.Password = passwordHasher.HashPassword(admin, password);
        context.Uzytkownik.Add(admin);
    }

    private static bool IsIdentityPasswordHash(string value)
    {
        try
        {
            var decoded = Convert.FromBase64String(value);
            return decoded.Length > 0 && decoded[0] is 0x00 or 0x01;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
