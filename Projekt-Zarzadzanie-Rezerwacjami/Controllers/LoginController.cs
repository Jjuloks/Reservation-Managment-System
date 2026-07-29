using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Projekt_Zarzadzanie_Rezerwacjami.Data;
using Projekt_Zarzadzanie_Rezerwacjami.Models;
 
namespace Projekt_Zarzadzanie_Rezerwacjami.Controllers
{
    public class LoginController : Controller
    {
        private readonly Projekt_Zarzadzanie_RezerwacjamiContext _context;
        private readonly IPasswordHasher<Uzytkownik> _passwordHasher;

        public LoginController(
            Projekt_Zarzadzanie_RezerwacjamiContext context,
            IPasswordHasher<Uzytkownik> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logowanie(string? login, string? password)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Niepoprawne dane logowania";
                return View("Index");
            }

            var user = await _context.Uzytkownik
                .FirstOrDefaultAsync(x => x.Login == login);

            if (user != null && await VerifyPasswordAsync(user, password))
            {
                HttpContext.Session.SetString("role", user.Role);
                HttpContext.Session.SetString("login", user.Login);

                if (user.Role == "admin")
                    return RedirectToAction("Index", "Rezerwacje");

                if (user.Role == "user")
                    return RedirectToAction("Index", "User");
            }

            ViewBag.Error = "Niepoprawne dane logowania";
            return View("Index");
        }

        private async Task<bool> VerifyPasswordAsync(Uzytkownik user, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);

            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.Password = _passwordHasher.HashPassword(user, password);
                await _context.SaveChangesAsync();
            }

            return result != PasswordVerificationResult.Failed;
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
