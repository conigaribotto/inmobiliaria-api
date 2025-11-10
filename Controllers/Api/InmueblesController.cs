using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using _Net.Models;
using InmobiliariaAPI.Dtos;
using InmobiliariaAPI.Helpers;

namespace _Net.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class InmueblesController : ControllerBase
{
    private readonly IRepositoryInmuebles _repo;
    private readonly IWebHostEnvironment _env;

    public InmueblesController(IRepositoryInmuebles repo, IWebHostEnvironment env)
    {
        _repo = repo;
        _env = env;
    }

    [HttpGet]
    public ActionResult<IEnumerable<InmuebleListItemDto>> GetAll()
    {
        var pid = User.PropietarioId();
        var items = _repo.ObtenerTodosOPorFiltro(idPropietario: pid, disponible: null)
            .Select(i => new InmuebleListItemDto(i.IdInmueble, i.Direccion, i.Disponible, i.FotoUrl))
            .ToList();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public ActionResult<InmuebleListItemDto> GetOne(int id)
    {
        var pid = User.PropietarioId();
        var i = _repo.ObtenerPorId(id);
        if (i is null || i.IdPropietario != pid) return NotFound();
        return Ok(new InmuebleListItemDto(i.IdInmueble, i.Direccion, i.Disponible, i.FotoUrl));
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10_000_000)]
    public async Task<ActionResult> Create([FromForm] InmuebleCreateDto dto)
    {
        var pid = User.PropietarioId();
        string? fotoUrl = null;

        if (dto.Foto is not null && dto.Foto.Length > 0)
        {
            var ext = Path.GetExtension(dto.Foto.FileName).ToLowerInvariant();
            if (ext is not (".jpg" or ".jpeg" or ".png"))
                return BadRequest("Formato de imagen inválido");

            var fileName = $"{Guid.NewGuid()}{ext}";
            var webroot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var folder = Path.Combine(webroot, "fotos");
            Directory.CreateDirectory(folder);
            var path = Path.Combine(folder, fileName);
            using var fs = System.IO.File.Create(path);
            await dto.Foto.CopyToAsync(fs);

            fotoUrl = "/fotos/" + fileName;
        }

        var entity = new Inmueble
        {
            Direccion = dto.Direccion,
            Tipo = dto.Tipo,
            Uso = dto.Uso,
            Ambientes = dto.Ambientes,
            Latitud = dto.Latitud,
            Longitud = dto.Longitud,
            IdPropietario = pid,
            Disponible = dto.Disponible ?? true,
            FotoUrl = fotoUrl
        };

        var id = _repo.Alta(entity);
        return CreatedAtAction(nameof(GetOne), new { id }, null);
    }

    [HttpPatch("{id:int}/estado")]
    public IActionResult SetEstado(int id, [FromBody] bool habilitar)
    {
        var pid = User.PropietarioId();
        var i = _repo.ObtenerPorId(id);
        if (i is null || i.IdPropietario != pid) return NotFound();

        i.Disponible = habilitar;
        _repo.Modificar(i);
        return NoContent();
    }
}
