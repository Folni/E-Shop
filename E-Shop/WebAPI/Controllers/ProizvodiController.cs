using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using WebAPI.DTOs;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProizvodiController : ControllerBase
    {
        private readonly EtrgovinaContext _context;

        public ProizvodiController(EtrgovinaContext context)
        {
            _context = context;
        }

        // GET: api/Proizvodi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProizvodDTO>>> GetProizvodi()
        {
            try
            {
                var proizvodi = await _context.Proizvods
                    .Include(p => p.Kategorija)
                    .Select(p => new ProizvodDTO
                    {
                        Id = p.ProizvodId,
                        Naziv = p.Naziv,
                        Opis = p.Opis,
                        Cijena = p.Cijena,
                        SlikaPath = p.SlikaPath,
                        KategorijaId = p.KategorijaId, // Dodano mapiranje ID-a
                        KategorijaNaziv = p.Kategorija != null ? p.Kategorija.Naziv : "Nema kategorije"
                    })
                    .ToListAsync();

                return Ok(proizvodi);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Greška pri dohvaćanju podataka: {ex.Message}");
            }
        }

        // POST: api/Proizvodi
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ProizvodDTO>> PostProizvod([FromBody] ProizvodDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var proizvod = new Proizvod
                {
                    Naziv = dto.Naziv,
                    Opis = dto.Opis,
                    Cijena = dto.Cijena,
                    SlikaPath = dto.SlikaPath,
                    KategorijaId = dto.KategorijaId // Koristi ID iz DTO-a
                };

                _context.Proizvods.Add(proizvod);
                await _context.SaveChangesAsync();

                // Logiranje akcije
                _context.Logovis.Add(new Logovi
                {
                    Tip = "INFO",
                    Poruka = $"Admin dodao novi proizvod: {proizvod.Naziv} (Kat ID: {proizvod.KategorijaId})",
                    Datum = DateTime.Now
                });
                await _context.SaveChangesAsync();

                dto.Id = proizvod.ProizvodId;
                return CreatedAtAction(nameof(GetProizvodi), new { id = dto.Id }, dto);
            }
            catch (Exception ex)
            {
                // Prikaz detaljnije greške ako postoji (npr. strani ključ ne postoji)
                var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest($"Greška pri spremanju: {msg}");
            }
        }

        // PUT: api/Proizvodi/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProizvod(int id, [FromBody] ProizvodDTO dto)
        {
            if (id != dto.Id) return BadRequest("ID nesklad.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var proizvod = await _context.Proizvods.FindAsync(id);
            if (proizvod == null) return NotFound();

            proizvod.Naziv = dto.Naziv;
            proizvod.Opis = dto.Opis;
            proizvod.Cijena = dto.Cijena;
            proizvod.SlikaPath = dto.SlikaPath;
            proizvod.KategorijaId = dto.KategorijaId; // Ažuriranje kategorije preko ID-a

            try
            {
                await _context.SaveChangesAsync();

                _context.Logovis.Add(new Logovi
                {
                    Tip = "UPDATE",
                    Poruka = $"Admin ažurirao proizvod ID: {id}",
                    Datum = DateTime.Now
                });
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest($"Greška pri ažuriranju: {msg}");
            }
        }

        // DELETE: api/Proizvodi/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProizvod(int id)
        {
            var proizvod = await _context.Proizvods.FindAsync(id);
            if (proizvod == null) return NotFound();

            try
            {
                _context.Proizvods.Remove(proizvod);
                _context.Logovis.Add(new Logovi
                {
                    Tip = "DELETE",
                    Poruka = $"Admin obrisao proizvod: {proizvod.Naziv}",
                    Datum = DateTime.Now
                });
                await _context.SaveChangesAsync();

                return Ok($"Proizvod {proizvod.Naziv} obrisan.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Greška pri brisanju: {ex.Message}");
            }
        }

        [HttpGet("Filter")]
        public async Task<ActionResult<IEnumerable<ProizvodDTO>>> Filter(string? naziv)
        {
            var query = _context.Proizvods.Include(p => p.Kategorija).AsQueryable();
            if (!string.IsNullOrEmpty(naziv)) query = query.Where(p => p.Naziv.Contains(naziv));

            var rezultati = await query.Select(p => new ProizvodDTO
            {
                Id = p.ProizvodId,
                Naziv = p.Naziv,
                Cijena = p.Cijena,
                KategorijaId = p.KategorijaId, // Mapiranje ID-a
                KategorijaNaziv = p.Kategorija != null ? p.Kategorija.Naziv : "Nema kategorije",
                SlikaPath = p.SlikaPath
            }).ToListAsync();

            return Ok(rezultati);
        }

        [HttpGet("Paged")]
        public async Task<IActionResult> GetPaged(int page = 1, int count = 10)
        {
            // Poziv Stored Procedure (Vježba 5.7)
            var proizvodi = await _context.Proizvods
                .FromSqlInterpolated($"EXEC GetProizvodi @Page={page}, @Count={count}")
                .ToListAsync();
            return Ok(proizvodi);
        }
    }
}