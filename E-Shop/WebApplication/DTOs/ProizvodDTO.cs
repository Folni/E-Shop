namespace WebApplication.DTOs
{
    public class ProizvodDTO
    {
        public int ProizvodId { get; set; }
        public string Naziv { get; set; } = null!;
        public string? Opis { get; set; }
        public decimal Cijena { get; set; }
        public string? KategorijaNaziv { get; set; }
        public List<string> DrzaveNazivi { get; set; } = new List<string>();
    }
}