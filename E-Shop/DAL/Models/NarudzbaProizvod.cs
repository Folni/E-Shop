using System;
using System.Collections.Generic;

namespace ETrgovina.DAL.Models;

public partial class NarudzbaProizvod
{
    public int NarudzbaId { get; set; }

    public int ProizvodId { get; set; }

    public int? Kolicina { get; set; }

    public virtual Narudzba Narudzba { get; set; } = null!;

    public virtual Proizvod Proizvod { get; set; } = null!;
}
