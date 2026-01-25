using System;
using System.Collections.Generic;

namespace WebAPI.View
{
    public class NarudzbaView
    {
        public int Id { get; set; }
        public DateTime DatumKreiranja { get; set; }
        public decimal UkupnaCijena { get; set; }

        public int KorisnikId { get; set; }
        public string KorisnikIme { get; set; } = null!;

        public List<NarudzbaProizvodView> Proizvodi { get; set; } = new();
    }
}
