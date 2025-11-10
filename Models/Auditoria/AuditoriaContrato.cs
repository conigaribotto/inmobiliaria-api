
using System.ComponentModel.DataAnnotations.Schema;

namespace _Net.Models

{
    public class AuditoriaContrato
    {
        public int IdRegistro { get; set; }
        public int IdContrato { get; set; }

        public int IdUsuarioCreador { get; set; }
        public DateTime FechaCreacion { get; set; }

        public int? IdUsuarioFinalizador { get; set; }
        public DateTime? FechaFinalizacion { get; set; }

        [NotMapped]
        public string? NombreUsuarioCreador { get; set; }
        [NotMapped] public string? NombreUsuarioFinalizador { get; set; }
    }
}
