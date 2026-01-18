using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebAPI.Models;
using System.Security.Claims; 

namespace WebApplication.Controllers
{
    public class KosaricaController : Controller
    {
        private readonly EtrgovinaContext _context;

        public KosaricaController(EtrgovinaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var kosarica = PreuzmiIzSesije();
            return View(kosarica);
        }

        [HttpPost]
        public async Task<IActionResult> Dodaj(int proizvodId)
        {
            var proizvod = await _context.Proizvods
                .Include(p => p.Kategorija)
                .FirstOrDefaultAsync(p => p.ProizvodId == proizvodId);

            if (proizvod == null) return NotFound();

            var kosarica = PreuzmiIzSesije();
            kosarica.Add(proizvod);
            SpremiUSesiju(kosarica);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Ukloni(int proizvodId)
        {
            var kosarica = PreuzmiIzSesije();
            var stavka = kosarica.FirstOrDefault(p => p.ProizvodId == proizvodId);

            if (stavka != null)
            {
                kosarica.Remove(stavka);
                SpremiUSesiju(kosarica);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PotvrdiNarudzbu()
        {
            var kosarica = PreuzmiIzSesije();
            if (kosarica == null || !kosarica.Any())
            {
                TempData["Error"] = "Vaša košarica je prazna.";
                return RedirectToAction("Index");
            }

            var emailKorisnika = User.FindFirstValue(ClaimTypes.Name) ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(emailKorisnika))
            {
                return RedirectToAction("Login", "Autentifikacija");
            }

            try
            {
                var korisnik = await _context.Korisniks
                    .FirstOrDefaultAsync(k => k.Email == emailKorisnika);

                if (korisnik == null)
                {
                    return Content($"Greška: Korisnik s emailom {emailKorisnika} nije pronađen u bazi.");
                }

                var novaNarudzba = new Narudzba
                {
                    KorisnikId = korisnik.KorisnikId,
                    DatumKreiranja = DateTime.Now,
                    UkupnaCijena = kosarica.Sum(p => p.Cijena)
                };

                _context.Narudzbas.Add(novaNarudzba);
                await _context.SaveChangesAsync();

                foreach (var p in kosarica)
                {
                    _context.NarudzbaProizvods.Add(new NarudzbaProizvod
                    {
                        NarudzbaId = novaNarudzba.NarudzbaId,
                        ProizvodId = p.ProizvodId,
                        Kolicina = 1
                    });
                }

                await _context.SaveChangesAsync();

                HttpContext.Session.Remove("Kosarica");
                return RedirectToAction("Hvala", new { id = novaNarudzba.NarudzbaId });
            }
            catch (Exception ex)
            {
                return Content($"Baza podataka je javila grešku: {ex.Message} | Detalji: {ex.InnerException?.Message}");
            }
        }

        public IActionResult Hvala(int id)
        {
            ViewBag.NarudzbaId = id;
            return View();
        }

        private List<Proizvod> PreuzmiIzSesije()
        {
            var json = HttpContext.Session.GetString("Kosarica");
            if (string.IsNullOrEmpty(json)) return new List<Proizvod>();

            var options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles };
            return JsonSerializer.Deserialize<List<Proizvod>>(json, options) ?? new List<Proizvod>();
        }

        private void SpremiUSesiju(List<Proizvod> list)
        {
            var options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles };
            var json = JsonSerializer.Serialize(list, options);
            HttpContext.Session.SetString("Kosarica", json);
        }
    }
}