namespace WebAPI.DTOs
{
    public class UserRegisterDTO
    {
        public string Ime { get; set; } = null!;
        public string Prezime { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}