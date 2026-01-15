using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class Narudzba
{
    public int NarudzbaId { get; set; }

    public DateTime? DatumKreiranja { get; set; }

    public int KorisnikId { get; set; }

    public decimal? UkupnaCijena { get; set; }

    public virtual Korisnik Korisnik { get; set; } = null!;

    public virtual ICollection<NarudzbaProizvod> NarudzbaProizvods { get; set; } = new List<NarudzbaProizvod>();
}
