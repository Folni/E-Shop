using AutoMapper;
using AutoMapper.QueryableExtensions; // Potrebno za ProjectTo
using ETrgovina.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTO;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LogoviController : ControllerBase
    {
        private readonly EtrgovinaContext _context;
        private readonly IMapper _mapper;

        public LogoviController(EtrgovinaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Logovi?n=10&orderBy=id
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

            var result = await query
                .Take(n)
                .ProjectTo<LogDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(result);
        }

        // POST: api/Logovi
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LogDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var log = _mapper.Map<Logovi>(dto);

            log.Datum ??= DateTime.Now;

            _context.Logovis.Add(log);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST: api/Logovi/bulk
        [HttpPost("bulk")]
        public async Task<IActionResult> PostBulk([FromBody] List<LogDTO> dtos)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var logovi = _mapper.Map<List<Logovi>>(dtos);

            foreach (var l in logovi) l.Datum ??= DateTime.Now;

            _context.Logovis.AddRange(logovi);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Logovi/5
        [HttpDelete("{n}")]
        public async Task<IActionResult> Delete(int n)
        {
            var zaBrisanje = await _context.Logovis
                .OrderBy(l => l.LogId)
                .Take(n)
                .ToListAsync();

            _context.Logovis.RemoveRange(zaBrisanje);
            await _context.SaveChangesAsync();

            return Ok($"Obrisano je {zaBrisanje.Count} zapisa.");
        }
    }
}