using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization; 

namespace WebAPI.DTO
{
    public class ProizvodDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan")]
        [StringLength(100, ErrorMessage = "Naziv ne smije biti duži od 100 znakova")]
        public string Naziv { get; set; } = null!;

        [StringLength(2048, ErrorMessage = "Opis ne smije biti duži od 2048 znakova")]
        public string? Opis { get; set; }

        [Range(0.01, 999999.99, ErrorMessage = "Cijena mora biti veća od 0")]
        public decimal Cijena { get; set; }

        [StringLength(255)]
        public string? SlikaPath { get; set; }

        [Required(ErrorMessage = "KategorijaId je obavezan")]
        public int KategorijaId { get; set; }
        public string? KategorijaNaziv { get; set; }
    }
}