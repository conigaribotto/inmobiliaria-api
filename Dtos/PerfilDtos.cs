using System.ComponentModel.DataAnnotations;

namespace InmobiliariaAPI.Dtos;

public class PerfilDto
{
    [Required, StringLength(80)]
    public string Nombre { get; set; } = "";

    [Required, StringLength(80)]
    public string Apellido { get; set; } = "";

    [Phone]
    public string? Telefono { get; set; }

    [Required, EmailAddress]
    public string Mail { get; set; } = "";
}

public class CambioClaveDto
{
    [Required]
    public string Actual { get; set; } = "";

    [Required, MinLength(6)]
    public string Nueva { get; set; } = "";
}
