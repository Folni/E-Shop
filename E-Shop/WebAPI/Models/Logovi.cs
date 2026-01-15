using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class Logovi
{
    public int LogId { get; set; }

    public string? Tip { get; set; }

    public string? Poruka { get; set; }

    public DateTime? Datum { get; set; }

    public int? KorisnikId { get; set; }

    public virtual Korisnik? Korisnik { get; set; }
}
