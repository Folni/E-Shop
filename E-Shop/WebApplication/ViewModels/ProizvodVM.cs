using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; 

namespace WebApplication.ViewModels
{
    public class ProizvodVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan!")]
        [Display(Name = "Naziv proizvoda")]
        public string Naziv { get; set; } = null!;

        [Required(ErrorMessage = "Cijena je obavezna!")]
        [Range(0.01, 99999.99, ErrorMessage = "Cijena mora biti veća od 0!")]
        public decimal Cijena { get; set; }

        [Display(Name = "Opis proizvoda")]
        public string? Opis { get; set; }

        [Required(ErrorMessage = "Odaberite kategoriju!")]
        [Display(Name = "Kategorija")]
        public int KategorijaId { get; set; }

        [Display(Name = "Dostupno u državama")]
        public List<int> OdabraneDrzaveIds { get; set; } = new List<int>();
    }
}