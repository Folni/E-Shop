namespace WebAPI.DTO
{
    public class AuthResponseDTO
    {
        public string Token { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string ImePrezime { get; set; } = null!;
        public string Uloga { get; set; } = null!;
    }
}
