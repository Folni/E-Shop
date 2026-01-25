namespace WebAPI.View
{
    public class NarudzbaProizvodView
    {
        public int ProizvodId { get; set; }
        public string NazivProizvoda { get; set; } = null!;
        public int Kolicina { get; set; }
        public decimal Cijena { get; set; }
        public decimal Ukupno { get; set; }
    }
}
