using AutoMapper;
using ETrgovina.DAL.Models;
using ETrgovina.DAL.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebAPI.DTO;


namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KorisniciController : ControllerBase
    {
        private readonly EtrgovinaContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper; 

        public KorisniciController(EtrgovinaContext context, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper; 
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var korisnik = await _context.Korisniks
                .FirstOrDefaultAsync(k => k.Email == login.Username);

            if (korisnik == null)
                return Unauthorized("Neispravan email ili lozinka.");

            var hash = PasswordHashProvider.GetHash(login.Password, korisnik.LozinkaSalt!);

            if (korisnik.LozinkaHash != hash)
                return Unauthorized("Neispravan email ili lozinka.");

            var secureKey = _configuration["JWT:SecureKey"];
            var token = JwtTokenProvider.CreateToken(
                secureKey!,
                120,
                korisnik.Email,
                korisnik.Uloga ?? "Korisnik"
            );

            var response = _mapper.Map<AuthResponseDTO>(korisnik);
            response.Token = token;

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _context.Korisniks.AnyAsync(k => k.Email == model.Email))
                return BadRequest("Korisnik s tim emailom već postoji.");

            var korisnik = _mapper.Map<Korisnik>(model);

            var salt = PasswordHashProvider.GetSalt();
            korisnik.LozinkaSalt = salt;
            korisnik.LozinkaHash = PasswordHashProvider.GetHash(model.Password, salt);
            korisnik.Uloga = "Korisnik";
            korisnik.Guid = Guid.NewGuid();

            _context.Korisniks.Add(korisnik);

            _context.Logovis.Add(new Logovi
            {
                Tip = "INFO",
                Poruka = $"Registriran novi korisnik: {korisnik.Email}",
                Datum = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return Ok("Registracija uspješna.");
        }

        [Authorize]
        [HttpGet("profil")]
        public async Task<IActionResult> Profil()
        {
            var email = User.Identity?.Name;
            var korisnik = await _context.Korisniks.FirstOrDefaultAsync(k => k.Email == email);

            if (korisnik == null) return NotFound();

            return Ok(_mapper.Map<KorisnikDTO>(korisnik));
        }
    }
}