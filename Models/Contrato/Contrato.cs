using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace _Net.Models;

public class Contrato
{
    [Key]
    [Display(Name = "N° de Contrato")]
    public int IdContrato { get; set; }

    [Display(Name = "Inquilino")]
    public int IdInquilino { get; set; }

    [Display(Name = "Inmueble")]
    public int IdInmueble { get; set; }

    [Display(Name = "Fecha de inicio")]
    public DateTime FechaInicio { get; set; }

    [Display(Name = "Fecha de finalización")]
    public DateTime FechaFin { get; set; }

    [Display(Name = "Valor mensual")]
    public double ValorMensual { get; set; }

    [Display(Name = "¿Vigente?")]
    public bool Vigente { get; set; } = true;

    [NotMapped]
    public string? InquilinoNombre { get; set; }
    [NotMapped]
    public string? InquilinoApellido { get; set; }
    [NotMapped]
    public string InquilinoNombreCompleto => $"{InquilinoNombre} {InquilinoApellido}";
    [NotMapped]
    public string? InmuebleDireccion { get; set; }


    public override string ToString()
    {
        return $"Inquilino: {InquilinoNombreCompleto}, Inmueble: {InmuebleDireccion}, " +
               $"Desde: {FechaInicio.ToShortDateString()} Hasta: {FechaFin.ToShortDateString()}, " +
               $"Monto Mensual: {ValorMensual}, Activo: {Vigente}";
    }
}
