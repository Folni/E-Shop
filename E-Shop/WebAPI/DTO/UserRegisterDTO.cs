using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTO
{
    public class UserRegisterDTO
    {
        [Required(ErrorMessage = "Ime je obavezno")]
        public string Ime { get; set; } = null!;

        [Required(ErrorMessage = "Prezime je obavezno")]
        public string Prezime { get; set; } = null!;

        [Required(ErrorMessage = "Email je obavezan")]
        [EmailAddress(ErrorMessage = "Neispravan format emaila")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Lozinka je obavezna")]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = null!;
    }
}
