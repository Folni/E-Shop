using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using Microsoft.EntityFrameworkCore; 
using System.Linq; 

namespace WebApplication.Controllers
{
    [Authorize] 
    public class NarudzbeController : Controller
    {
        private readonly EtrgovinaContext _context;

        public NarudzbeController(EtrgovinaContext context) { _context = context; }

        [HttpPost]
        public async Task<IActionResult> Dodaj(int proizvodId)
        {
            var email = User.Identity.Name;
            var korisnik = await _context.Korisniks.FirstOrDefaultAsync(u => u.Email == email);

            var narudzba = new Narudzba
            {
                KorisnikId = korisnik.KorisnikId,
                DatumKreiranja = DateTime.Now,
                UkupnaCijena = 0 
            };

            _context.Narudzbas.Add(narudzba);
            await _context.SaveChangesAsync();

            var proizvod = await _context.Proizvods.FindAsync(proizvodId);
            var stavka = new NarudzbaProizvod
            {
                NarudzbaId = narudzba.NarudzbaId,
                ProizvodId = proizvodId,
                Kolicina = 1
            };

            _context.NarudzbaProizvods.Add(stavka);
            narudzba.UkupnaCijena = proizvod.Cijena;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Narudžba je uspješno kreirana!";
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminPregled()
        {
            var narudzbe = await _context.Narudzbas
                .Include(n => n.Korisnik) 
                .Include(n => n.NarudzbaProizvods) 
                    .ThenInclude(np => np.Proizvod) 
                .OrderByDescending(n => n.DatumKreiranja)
                .ToListAsync();

            return View(narudzbe);
        }
    }
}
