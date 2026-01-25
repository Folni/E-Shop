using System;
using System.Collections.Generic;

namespace ETrgovina.DAL.Models;

public partial class Korisnik
{
    public int KorisnikId { get; set; }

    public Guid Guid { get; set; }

    public string Ime { get; set; } = null!;

    public string Prezime { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string LozinkaHash { get; set; } = null!;

    public string? LozinkaSalt { get; set; }

    public string? Uloga { get; set; }

    public virtual ICollection<Logovi> Logovis { get; set; } = new List<Logovi>();

    public virtual ICollection<Narudzba> Narudzbas { get; set; } = new List<Narudzba>();
}