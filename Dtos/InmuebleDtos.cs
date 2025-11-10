using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace InmobiliariaAPI.Dtos;

public class InmuebleCreateDto
 {
    public string Direccion { get; set; } = null!;
    public string? Tipo { get; set; }
    public string? Uso { get; set; }
    public int Ambientes { get; set; }
    public decimal Latitud { get; set; }
    public decimal Longitud { get; set; }
    public IFormFile? Foto { get; set; }
    public bool? Disponible { get; set; } 
}


public record InmuebleListItemDto(
    int Id,
    string? Direccion,
    bool Disponible,
    string? FotoUrl
);

