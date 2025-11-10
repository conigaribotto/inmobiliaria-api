using System.ComponentModel.DataAnnotations.Schema;

namespace _Net.Models
{
    public class AuditoriaPago
    {
        public int IdRegistro { get; set; }
        public int IdPago { get; set; }
        public int IdUsuarioCreador { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int? IdUsuarioAnulador { get; set; }
        public DateTime? FechaAnulacion { get; set; }

        [NotMapped]
        public string? NombreUsuarioCreador { get; set; }
        [NotMapped]
        public string? NombreUsuarioAnulador { get; set; }
    }
}
