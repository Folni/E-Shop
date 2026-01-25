using System;

namespace WebAPI.View
{
    public class KorisnikView
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }

        public string Ime { get; set; } = null!;
        public string Prezime { get; set; } = null!;
        public string Email { get; set; } = null!;

        public string? Uloga { get; set; }
    }
}
