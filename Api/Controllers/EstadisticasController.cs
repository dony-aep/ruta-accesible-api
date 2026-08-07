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

    [HttpGet("barreras-por-zona")]
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
