using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using _Net.Models;
using InmobiliariaAPI.Dtos;
using InmobiliariaAPI.Helpers;

namespace _Net.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class PropietariosController : ControllerBase
{
    private readonly IRepositoryPropietario _repoProp;
    private readonly UsuariosRepository _users;

    public PropietariosController(IRepositoryPropietario repoProp, IConfiguration cfg)
    {
        _repoProp = repoProp;
        _users = new UsuariosRepository(cfg);
    }

    [HttpGet("me")]
    public ActionResult<PerfilDto> GetMe()
    {
        var pid = User.PropietarioId();
        var p = _repoProp.ObtenerPorId(pid);
        if (p is null) return NotFound();
        return Ok(new PerfilDto { Nombre = p.Nombre ?? "", Apellido = p.Apellido ?? "", Telefono = p.Telefono, Mail = p.Mail ?? "" });
    }

    [HttpPut("me")]
    public IActionResult UpdateMe([FromBody] PerfilDto dto)
    {
        var pid = User.PropietarioId();
        var p = _repoProp.ObtenerPorId(pid);
        if (p is null) return NotFound();

        p.Nombre = dto.Nombre;
        p.Apellido = dto.Apellido;
        p.Telefono = dto.Telefono;
        p.Mail = dto.Mail;
        _repoProp.Modificar(p);

        return NoContent();
    }

    [HttpPut("me/password")]
    public IActionResult ChangePassword([FromBody] CambioClaveDto dto)
    {
        var email = User.FindFirstValue(ClaimTypes.Email)!;
        var u = _users.ObtenerPorEmail(email);
        if (u is null) return NotFound(new { error = "Usuario no encontrado" });

        if (u.Password != dto.Actual)
            return BadRequest(new { error = "La contraseña actual no coincide" }); // o Forbid()

        u.Password = dto.Nueva; // TODO: hashear
        _users.Modificar(u);
        return NoContent();
    }
}
