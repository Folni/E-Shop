namespace WebAPI.View
{
    public class ProizvodView
    {
        public int Id { get; set; }
        public string Naziv { get; set; } = null!;
        public string? Opis { get; set; }
        public decimal Cijena { get; set; }
        public string? SlikaPath { get; set; }

        public int KategorijaId { get; set; }
        public string? KategorijaNaziv { get; set; }
    }
}
