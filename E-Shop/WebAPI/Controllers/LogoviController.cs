using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class LogoviController : ControllerBase
{
    private readonly EtrgovinaContext _context;

    public LogoviController(EtrgovinaContext context)
    {
        _context = context;
    }

    // GET: api/Logovi?n=10&orderBy=id
    // Implementacija dohvata posljednjih n zapisa (Vježba 5.9)
    [HttpGet]
    public async Task<IActionResult> Get(int n = 10, string orderBy = "id")
    {
        var query = _context.Logovis.AsQueryable();

        query = orderBy.ToLower() switch
        {
            "datum" => query.OrderByDescending(l => l.Datum),
            "poruka" => query.OrderBy(l => l.Poruka),
            _ => query.OrderByDescending(l => l.LogId)
        };

        return Ok(await query.Take(n).ToListAsync());
    }

    // POST: api/Logovi
    // Dodavanje loga uz validaciju (Vježba 5.6, 5.9)
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] LogDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var noviLog = new Logovi
        {
            Tip = dto.Tip,
            Poruka = dto.Poruka,
            Datum = DateTime.Now
        };

        _context.Logovis.Add(noviLog);
        await _context.SaveChangesAsync();
        return Ok();
    }

    // POST: api/Logovi/bulk
    // Dodavanje više zapisa odjednom (Vježba 5.9)
    [HttpPost("bulk")]
    public async Task<IActionResult> PostBulk([FromBody] LogDto[] dtos)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var noviLogovi = dtos.Select(dto => new Logovi
        {
            Tip = dto.Tip,
            Poruka = dto.Poruka,
            Datum = DateTime.Now
        });

        _context.Logovis.AddRange(noviLogovi);
        await _context.SaveChangesAsync();
        return Ok();
    }

    // DELETE: api/Logovi/5
    // Briše prvih n zapisa (Vježba 5.9)
    [HttpDelete("{n}")]
    public async Task<IActionResult> Delete(int n)
    {
        var zaBrisanje = await _context.Logovis
            .OrderBy(l => l.LogId)
            .Take(n)
            .ToListAsync();

        _context.Logovis.RemoveRange(zaBrisanje);
        await _context.SaveChangesAsync();
        return Ok($"Obrisano je prvih {zaBrisanje.Count} zapisa.");
    }

    public class LogDto
    {
        [Required(ErrorMessage = "Tip loga je obavezan")] //
        [StringLength(50)]
        public string Tip { get; set; } = null!;

        [Required(ErrorMessage = "Poruka je obavezna")] //
        [StringLength(1024, ErrorMessage = "Poruka ne smije biti duža od 1024 znaka")] //
        public string Poruka { get; set; } = null!;
    }
}