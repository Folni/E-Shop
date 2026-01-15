using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization; // Potrebno za [Authorize]

namespace WebAPI.Controllers
{
    [Authorize] // Vježba 6.7 - Samo prijavljeni korisnici s JWT tokenom mogu pristupiti
    [Route("api/[controller]")]
    [ApiController]
    public class NarudzbeController : ControllerBase
    {
        private readonly EtrgovinaContext _context;

        public NarudzbeController(EtrgovinaContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> KreirajNarudzbu([FromBody] NarudzbaDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // 1. Kreiramo glavni zapis narudžbe
                var novaNarudzba = new Narudzba
                {
                    KorisnikId = model.KorisnikId,
                    DatumKreiranja = DateTime.Now,
                    UkupnaCijena = model.Stavke.Sum(s => s.Cijena * s.Kolicina)
                };

                _context.Narudzbas.Add(novaNarudzba);
                await _context.SaveChangesAsync();

                // 2. Kreiramo stavke (veza n-to-n)
                foreach (var stavka in model.Stavke)
                {
                    var detalji = new NarudzbaProizvod
                    {
                        NarudzbaId = novaNarudzba.NarudzbaId,
                        ProizvodId = stavka.ProizvodId,
                        Kolicina = stavka.Kolicina
                    };
                    _context.NarudzbaProizvods.Add(detalji);
                }

                await _context.SaveChangesAsync();

                return Ok(new { Message = "Narudžba uspješna", OrderId = novaNarudzba.NarudzbaId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

    public class NarudzbaDTO
    {
        [Required(ErrorMessage = "KorisnikId je obavezan")]
        public int KorisnikId { get; set; }

        [Required(ErrorMessage = "Narudžba mora imati barem jednu stavku")]
        public List<StavkaDTO> Stavke { get; set; } = new();
    }

    public class StavkaDTO
    {
        [Required]
        public int ProizvodId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Količina mora biti barem 1")]
        public int Kolicina { get; set; }

        [Required]
        public decimal Cijena { get; set; }
    }
}