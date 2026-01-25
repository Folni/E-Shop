using System;
using System.Collections.Generic;

namespace ETrgovina.DAL.Models;

public partial class Drzava
{
    public int DrzavaId { get; set; }

    public string Naziv { get; set; } = null!;

    public virtual ICollection<Proizvod> Proizvods { get; set; } = new List<Proizvod>();
}
