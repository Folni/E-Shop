using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using WebAPI.Models;
using WebAPI.Security;
using Microsoft.EntityFrameworkCore;

namespace WebApplication.Controllers
{
    public class AutentifikacijaController : Controller
    {
        private readonly EtrgovinaContext _context;

        public AutentifikacijaController(EtrgovinaContext context)
        {
            _context = context;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string lozinka)
        {
            var korisnik = await _context.Korisniks
                .FirstOrDefaultAsync(k => k.Email == email);

            if (korisnik != null)
            {   
                var hashZaProvjeru = WebAPI.Security.PasswordHashProvider.GetHash(lozinka, korisnik.LozinkaSalt!);

                if (korisnik.LozinkaHash == hashZaProvjeru)
                {
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, korisnik.Email),
                    new Claim("StvarnoIme", korisnik.Ime), 
                    new Claim(ClaimTypes.Email, korisnik.Email),
                    new Claim(ClaimTypes.Role, korisnik.Uloga ?? "Korisnik")
                };

                    var claimsIdentity = new ClaimsIdentity(claims, Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    if (korisnik.Uloga == "Admin")
                    {
                        return RedirectToAction("Index", "Proizvodi");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            ViewBag.Error = "Neispravan email ili lozinka!";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string ime, string prezime, string email, string lozinka)
        {
            if (await _context.Korisniks.AnyAsync(u => u.Email == email))
            {
                ViewBag.Error = "Korisnik s tim emailom već postoji.";
                return View();
            }

            var salt = PasswordHashProvider.GetSalt();
            var hash = PasswordHashProvider.GetHash(lozinka, salt);

            var noviKorisnik = new Korisnik
            {
                Ime = ime,
                Prezime = prezime,
                Email = email,
                LozinkaHash = hash,
                LozinkaSalt = salt,
                Uloga = "Korisnik", 
                Guid = Guid.NewGuid()
            };

            _context.Korisniks.Add(noviKorisnik);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Registracija uspješna! Sada se možete prijaviti.";
            return RedirectToAction("Login");
        }
    }
}