using Api.Data;
using Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstadisticasController : ControllerBase
{
    private readonly AppDbContext _contexto;

    public EstadisticasController(AppDbContext contexto)
    {
        _contexto = contexto;
    }

    /// <summary>
    /// Cuenta los reportes agrupados por zona y tipo de barrera. Los reportes que
    /// todavía no se han analizado aparecen bajo la etiqueta Sin clasificar.
    /// </summary>
    /// <response code="200">Conteo por zona y tipo de barrera.</response>
    [HttpGet("barreras-por-zona")]
    [ProducesResponseType(typeof(IEnumerable<EstadisticasDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EstadisticasDto>>> ObtenerBarrerasPorZona()
    {
        // TipoBarreraId es nulo hasta que el endpoint de análisis clasifica el reporte,
        // así que los reportes en estado Registrado se agrupan bajo una etiqueta propia
        // en vez de aparecer como null en la respuesta.
        var estadisticas = await _contexto.Reportes
            .GroupBy(r => new
            {
                Zona = r.Lugar!.Zona,
                Tipo = r.TipoBarrera == null ? "Sin clasificar" : r.TipoBarrera.Nombre
            })
            .Select(g => new EstadisticasDto
            {
                Zona = g.Key.Zona,
                TipoBarrera = g.Key.Tipo,
                Cantidad = g.Count()
            })
            .ToListAsync();

        return Ok(estadisticas);
    }
}
