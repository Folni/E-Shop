using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class Proizvod
{
    public int ProizvodId { get; set; }

    public string Naziv { get; set; } = null!;

    public string? Opis { get; set; }

    public decimal Cijena { get; set; }

    public string? SlikaPath { get; set; }

    public int KategorijaId { get; set; }

    public virtual Kategorija Kategorija { get; set; } = null!;

    public virtual ICollection<NarudzbaProizvod> NarudzbaProizvods { get; set; } = new List<NarudzbaProizvod>();

    public virtual ICollection<Drzava> Drzavas { get; set; } = new List<Drzava>();
}
