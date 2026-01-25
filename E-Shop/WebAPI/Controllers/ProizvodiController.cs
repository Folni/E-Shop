using AutoMapper;
using AutoMapper.QueryableExtensions;
using ETrgovina.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTO;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProizvodiController : ControllerBase
    {
        private readonly EtrgovinaContext _context;
        private readonly IMapper _mapper;

        public ProizvodiController(EtrgovinaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Proizvodi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProizvodDTO>>> GetProizvodi()
        {
            // Umjesto ručnog Select-a, koristimo ProjectTo
            var proizvodi = await _context.Proizvods
                .ProjectTo<ProizvodDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(proizvodi);
        }

        // POST: api/Proizvodi
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ProizvodDTO>> PostProizvod([FromBody] ProizvodDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var proizvod = _mapper.Map<Proizvod>(dto);

            _context.Proizvods.Add(proizvod);

            // Logiranje akcije
            _context.Logovis.Add(new Logovi
            {
                Tip = "INFO",
                Poruka = $"Admin dodao novi proizvod: {proizvod.Naziv}",
                Datum = DateTime.Now
            });

            await _context.SaveChangesAsync();

            // Vraćamo mapirani DTO natrag (sada ima generiran ID)
            var createdDto = _mapper.Map<ProizvodDTO>(proizvod);
            return CreatedAtAction(nameof(GetProizvodi), new { id = createdDto.Id }, createdDto);
        }

        // PUT: api/Proizvodi/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProizvod(int id, [FromBody] ProizvodDTO dto)
        {
            if (id != dto.Id)
                return BadRequest("ID nesklad.");

            var proizvod = await _context.Proizvods.FindAsync(id);
            if (proizvod == null)
                return NotFound();

            // AutoMapper preslikava DTO preko postojećeg entiteta (Update)
            _mapper.Map(dto, proizvod);

            _context.Logovis.Add(new Logovi
            {
                Tip = "UPDATE",
                Poruka = $"Admin ažurirao proizvod ID: {id}",
                Datum = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Proizvodi/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProizvod(int id)
        {
            var proizvod = await _context.Proizvods.FindAsync(id);
            if (proizvod == null)
                return NotFound();

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

        // GET: api/Proizvodi/Filter?naziv=...
        [HttpGet("Filter")]
        public async Task<ActionResult<IEnumerable<ProizvodDTO>>> Filter(string? naziv)
        {
            var query = _context.Proizvods.AsQueryable();

            if (!string.IsNullOrEmpty(naziv))
                query = query.Where(p => p.Naziv.Contains(naziv));

            var rezultati = await query
                .ProjectTo<ProizvodDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(rezultati);
        }

        // GET: api/Proizvodi/Paged?page=1&count=10
        [HttpGet("Paged")]
        public async Task<ActionResult<IEnumerable<ProizvodDTO>>> GetPaged(int page = 1, int count = 10)
        {
            // Kod Stored Procedure, prvo dohvaćamo objekte pa ih mapiramo
            var proizvodi = await _context.Proizvods
                .FromSqlInterpolated($"EXEC GetProizvodi @Page={page}, @Count={count}")
                .ToListAsync();

            var result = _mapper.Map<List<ProizvodDTO>>(proizvodi);
            return Ok(result);
        }
    }
}