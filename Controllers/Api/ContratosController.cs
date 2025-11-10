using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using _Net.Models;
using InmobiliariaAPI.Dtos;
using InmobiliariaAPI.Helpers;

namespace _Net.Controllers;

[ApiController]
[Route("api")]
[Authorize]
[Produces("application/json")]
public class ContratosController : ControllerBase
{
    private readonly IRepositoryContratos _contratos;
    private readonly IRepositoryInmuebles _inmuebles;
    private readonly IRepositoryPagos _pagos;

    public ContratosController(IRepositoryContratos c, IRepositoryInmuebles i, IRepositoryPagos p)
    { _contratos = c; _inmuebles = i; _pagos = p; }

    [HttpGet("inmuebles/{idInmueble:int}/contratos")]
    public ActionResult<IEnumerable<ContratoListItemDto>> GetByInmueble(int idInmueble)
    {
        var pid = User.PropietarioId();
        var inm = _inmuebles.ObtenerPorId(idInmueble);
        if (inm is null || inm.IdPropietario != pid) return NotFound();

        var lista = _contratos.ObtenerPorInmueble(idInmueble)
            .Select(c => new ContratoListItemDto(c.IdContrato, c.FechaInicio, c.FechaFin, c.ValorMensual, c.Vigente))
            .ToList();

        return Ok(lista);
    }

    [HttpGet("contratos/{contratoId:int}/pagos")]
    public ActionResult<IEnumerable<PagoListItemDto>> GetPagos(int contratoId)
    {
        var pid = User.PropietarioId();
        var c = _contratos.ObtenerPorId(contratoId);
        if (c is null) return NotFound();

        var inm = _inmuebles.ObtenerPorId(c.IdInmueble);
        if (inm is null || inm.IdPropietario != pid) return Forbid();

        var pagos = _pagos.ObtenerPorContrato(contratoId)
            .Select(p => new PagoListItemDto(p.IdPago, p.concepto ?? "", p.importe, p.Fecha, p.anulado))
            .ToList();

        return Ok(pagos);
    }
}
