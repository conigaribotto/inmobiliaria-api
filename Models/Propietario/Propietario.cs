using System.ComponentModel.DataAnnotations;

namespace _Net.Models;

public class Propietario
{
    [Key]
    [Display(Name = "Num. de Código")]
    public int IdPropietario { get; set; }
    public int Documento { get; set; }
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }

    [Display(Name = "Teléfono")]
    public string? Telefono { get; set; }
    public string? Mail { get; set; }

    public override string ToString()
    {
        var res = $"Nombre: {Nombre}, Apellido: {Apellido}";
        return res;
    }

}
