using AutoMapper;
using ETrgovina.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTO;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NarudzbeController : ControllerBase
    {
        private readonly EtrgovinaContext _context;
        private readonly IMapper _mapper;

        public NarudzbeController(EtrgovinaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> KreirajNarudzbu([FromBody] NarudzbaDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1. Identifikacija korisnika
            var email = User.Identity?.Name;
            var korisnik = await _context.Korisniks
                .FirstOrDefaultAsync(k => k.Email == email);

            if (korisnik == null)
                return Unauthorized();

            // 2. Provjera i dohvat proizvoda (radi kalkulacije cijene)
            var proizvodIds = model.Stavke.Select(s => s.ProizvodId).ToList();
            var proizvodi = await _context.Proizvods
                .Where(p => proizvodIds.Contains(p.ProizvodId))
                .ToListAsync();

            if (proizvodi.Count != model.Stavke.Count)
                return BadRequest("Jedan ili više proizvoda ne postoje.");

            // 3. Mapiranje DTO-a u Entity model
            // AutoMapper će automatski kreirati listu NarudzbaProizvods iz model.Stavke
            var narudzba = _mapper.Map<Narudzba>(model);

            // 4. Dodatna logika koja se ne mapira automatski
            narudzba.KorisnikId = korisnik.KorisnikId;
            narudzba.DatumKreiranja = DateTime.Now;

            // Kalkulacija ukupne cijene (sigurnije je računati na serveru nego primati s frontenda)
            narudzba.UkupnaCijena = model.Stavke.Sum(s =>
                proizvodi.First(p => p.ProizvodId == s.ProizvodId).Cijena * s.Kolicina);

            _context.Narudzbas.Add(narudzba);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Narudžba uspješno kreirana",
                NarudzbaId = narudzba.NarudzbaId
            });
        }
    }
}