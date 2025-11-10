using System.ComponentModel.DataAnnotations;

namespace _Net.Models;

public class Pago
{
    [Key]
    [Display(Name = "N° de pago")]
    public int IdPago { get; set; }

    [Display(Name = "N° de Contrato")]
    public int IdContrato { get; set; } 

    [Display(Name = "Concepto")]
    public string? concepto { get; set; }

    [Display(Name = "Importe")]
    public double importe { get; set; }

    [Display(Name = "Fecha de Pago")]
    public DateTime Fecha { get; set; }

    [Display(Name = "¿Pago anulado?")]
    public bool anulado { get; set; }

    public override string ToString()
    {
        var res = $"Contrato ID: {IdContrato}, Concepto: {concepto}, " +
                  $"Importe: {importe}, Fecha: {Fecha.ToShortDateString()}, Anulado: {anulado}";
        return res;
    }
}
