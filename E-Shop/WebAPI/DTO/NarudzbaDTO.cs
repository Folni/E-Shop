using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTO
{
    public class NarudzbaDTO
    {
        [Required]
        public List<StavkaDTO> Stavke { get; set; } = new();
    }

    public class StavkaDTO
    {
        [Required]
        public int ProizvodId { get; set; }

        [Range(1, int.MaxValue)]
        public int Kolicina { get; set; }
    }
}
