// Controllers/Api/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using _Net.Models;
using InmobiliariaAPI.Dtos;

namespace _Net.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _cfg;
    private readonly UsuariosRepository _users;
    private readonly PropietariosRepository _props;

    public AuthController(IConfiguration cfg)
    {
        _cfg = cfg;
        _users = new UsuariosRepository(cfg);
        _props = new PropietariosRepository(cfg);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public ActionResult<LoginResponseDto> Login([FromBody] LoginRequestDto dto)
    {
        // 1) Usuario debe existir y estar activo
        var u = _users.ObtenerPorEmail(dto.Email);
        if (u is null || u.Password != dto.Password || u.Activo == false)
            return Unauthorized(new { error = "Credenciales inválidas" });

        // 2) Debe existir un Propietario con ese mismo email
        var p = _props.ObtenerPorEmail(dto.Email);
        if (p is null)
            return Unauthorized(new { error = "El usuario no está asociado a un Propietario" });

        // 3) Generar JWT con el IdPropietario
        var jwt = _cfg.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, p.IdPropietario.ToString()),
            new Claim(ClaimTypes.Email, u.Email),
            new Claim(ClaimTypes.Role, "Propietario")
        };

        var expires = DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpireMinutes"]!));
        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(new LoginResponseDto(tokenStr, expires));
    }
}
