using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTO
{
    public class LogDTO
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string? Tip { get; set; }

        [StringLength(1024)]
        public string? Poruka { get; set; }

        public DateTime? Datum { get; set; }

        public int? KorisnikId { get; set; }
    }
}
