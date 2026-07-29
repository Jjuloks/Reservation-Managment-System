using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Projekt_Zarzadzanie_Rezerwacjami.Data;
using Projekt_Zarzadzanie_Rezerwacjami.Models;

namespace Projekt_Zarzadzanie_Rezerwacjami.Controllers
{
    public class UzytkownicyController : Controller
    {
        private readonly Projekt_Zarzadzanie_RezerwacjamiContext _context;
        private readonly IPasswordHasher<Uzytkownik> _passwordHasher;

        public UzytkownicyController(
            Projekt_Zarzadzanie_RezerwacjamiContext context,
            IPasswordHasher<Uzytkownik> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("role");
            return role == "admin";
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
                return RedirectToAction("Index", "Login");

            return View(await _context.Uzytkownik.ToListAsync());
        }

        public IActionResult Create()
        {
            if (!IsAdmin())
                return RedirectToAction("Index", "Login");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Uzytkownik user)
        {
            if (!IsAdmin())
                return RedirectToAction("Index", "Login");

            if (ModelState.IsValid)
            {
                user.Password = _passwordHasher.HashPassword(user, user.Password);
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Index", "Login");

            var user = await _context.Uzytkownik.FindAsync(id);

            if (user != null)
            {
                _context.Uzytkownik.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
