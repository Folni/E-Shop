using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using WebAPI.Security; 
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KorisniciController : ControllerBase
    {
        private readonly EtrgovinaContext _context;
        private readonly IConfiguration _configuration;

        public KorisniciController(EtrgovinaContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var korisnik = await _context.Korisniks
                .FirstOrDefaultAsync(k => k.Email == login.Username);

            if (korisnik == null)
                return Unauthorized("Neispravan email ili lozinka.");

            var hashZaProvjeru = PasswordHashProvider.GetHash(login.Password, korisnik.LozinkaSalt!);

            if (korisnik.LozinkaHash != hashZaProvjeru)
                return Unauthorized("Neispravan email ili lozinka.");

            var secureKey = _configuration["JWT:SecureKey"];
            var token = JwtTokenProvider.CreateToken(secureKey!, 120, korisnik.Email, korisnik.Uloga ?? "Korisnik");

            return Ok(new
            {
                Token = token,
                korisnik.Email,
                ImePrezime = $"{korisnik.Ime} {korisnik.Prezime}",
                Uloga = korisnik.Uloga
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (await _context.Korisniks.AnyAsync(k => k.Email == model.Email))
            {
                return BadRequest("Korisnik s tim emailom već postoji.");
            }

            var salt = PasswordHashProvider.GetSalt();
            var hash = PasswordHashProvider.GetHash(model.Password, salt);

            var noviKorisnik = new Korisnik
            {
                Ime = model.Ime,
                Prezime = model.Prezime,
                Email = model.Email,
                LozinkaHash = hash,   
                LozinkaSalt = salt,   
                Uloga = "Korisnik",
                Guid = Guid.NewGuid()
            };

            try
            {
                _context.Korisniks.Add(noviKorisnik);

                _context.Logovis.Add(new Logovi
                {
                    Tip = "INFO",
                    Poruka = $"Novi korisnik registriran s JWT podrškom: {model.Email}",
                    Datum = DateTime.Now
                });

                await _context.SaveChangesAsync();
                return Ok("Registracija uspješna. Sada se možete prijaviti.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Greška pri registraciji: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("profil")]
        public IActionResult GetProfile()
        {
            var email = User.Identity?.Name;
            var uloga = User.FindFirstValue(ClaimTypes.Role);
            return Ok(new { Poruka = "Pristupili ste zaštićenim podacima", Email = email, Uloga = uloga });
        }

        public class RegisterModel
        {
            [Required(ErrorMessage = "Ime je obavezno")]
            public string Ime { get; set; } = null!;

            [Required(ErrorMessage = "Prezime je obavezno")]
            public string Prezime { get; set; } = null!;

            [Required(ErrorMessage = "Email je obavezan")]
            [EmailAddress(ErrorMessage = "Neispravan format emaila")]
            public string Email { get; set; } = null!;

            [Required(ErrorMessage = "Lozinka je obavezna")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Lozinka mora imati barem 6 znakova")]
            public string Password { get; set; } = null!;
        }

        public class LoginModel
        {
            [Required(ErrorMessage = "Korisničko ime (Email) je obavezno")]
            public string Username { get; set; } = null!;

            [Required(ErrorMessage = "Lozinka je obavezna")]
            public string Password { get; set; } = null!;
        }
    }
}