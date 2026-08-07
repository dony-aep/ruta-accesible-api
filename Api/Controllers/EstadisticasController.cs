using Api.Data;
using Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadisticasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EstadisticasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/estadisticas/barreras-por-zona
        [HttpGet("barreras-por-zona")]
        public async Task<ActionResult<IEnumerable<EstadisticasDto>>> GetBarrerasPorZona()
        {
            // TipoBarreraId es nulo hasta que el endpoint de análisis clasifica el reporte,
            // así que los reportes en estado Registrado se agrupan bajo una etiqueta propia
            // en vez de aparecer como null en la respuesta.
            var estadisticas = await _context.Reportes
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
}