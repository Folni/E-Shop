using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class Kategorija
{
    public int KategorijaId { get; set; }

    public string Naziv { get; set; } = null!;

    public virtual ICollection<Proizvod> Proizvods { get; set; } = new List<Proizvod>();
}
