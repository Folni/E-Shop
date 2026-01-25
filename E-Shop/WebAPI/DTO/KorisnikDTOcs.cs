namespace WebAPI.DTO
{
    public class KorisnikDTO
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Ime { get; set; } = null!;
        public string Prezime { get; set; } = null!;
        public string Uloga { get; set; } = null!;
    }
}
