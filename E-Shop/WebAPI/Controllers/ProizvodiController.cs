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

        // GET: api/Proizvodi/Paged?page=1&pageSize=10&naziv=mobitel
        [HttpGet("Paged")]
        public async Task<IActionResult> GetPagedProizvodi(int page = 1, int pageSize = 10, string? naziv = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _context.Proizvods.AsQueryable();

            if (!string.IsNullOrEmpty(naziv))
            {
                query = query.Where(p => p.Naziv.Contains(naziv));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var stavke = await query
                .OrderBy(p => p.ProizvodId) 
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<ProizvodDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize,
                Items = stavke
            });
        }

        // GET: api/Proizvodi 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProizvodDTO>>> GetProizvodi()
        {
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

            try
            {
                var proizvod = _mapper.Map<Proizvod>(dto);
                proizvod.Kategorija = null;

                _context.Proizvods.Add(proizvod);

                _context.Logovis.Add(new Logovi
                {
                    Tip = "INFO",
                    Poruka = $"Admin dodao novi proizvod: {proizvod.Naziv}",
                    Datum = DateTime.Now
                });

                await _context.SaveChangesAsync();

                var createdProizvod = await _context.Proizvods
                    .Include(p => p.Kategorija)
                    .FirstOrDefaultAsync(p => p.ProizvodId == proizvod.ProizvodId);

                var resultDto = _mapper.Map<ProizvodDTO>(createdProizvod);
                return CreatedAtAction(nameof(GetProizvodi), new { id = resultDto.Id }, resultDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Greška pri spremanju: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        // PUT: api/Proizvodi/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProizvod(int id, [FromBody] ProizvodDTO dto)
        {
            if (dto.Id != 0 && id != dto.Id)
                return BadRequest("ID nesklad.");

            var proizvod = await _context.Proizvods.FindAsync(id);
            if (proizvod == null)
                return NotFound();

            try
            {
                _mapper.Map(dto, proizvod);
                proizvod.Kategorija = null;
                proizvod.ProizvodId = id;

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
                return StatusCode(500, $"Greška pri ažuriranju: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        // DELETE: api/Proizvodi/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProizvod(int id)
        {
            var proizvod = await _context.Proizvods.FindAsync(id);
            if (proizvod == null)
                return NotFound();

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
                return Ok(new { message = $"Proizvod {proizvod.Naziv} obrisan." });
            }
            catch (Exception)
            {
                return StatusCode(500, "Ne možete obrisati proizvod koji je povezan s narudžbama.");
            }
        }

        [HttpGet("Filter")]
        public async Task<ActionResult<IEnumerable<ProizvodDTO>>> Filter(string? naziv)
        {
            var query = _context.Proizvods.AsQueryable();
            if (!string.IsNullOrEmpty(naziv))
                query = query.Where(p => p.Naziv.Contains(naziv));

            return Ok(await query.ProjectTo<ProizvodDTO>(_mapper.ConfigurationProvider).ToListAsync());
        }
    }
}