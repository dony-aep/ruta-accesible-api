using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

/// <summary>
/// Lugares públicos sobre los que se reportan barreras de accesibilidad.
/// </summary>
/// <remarks>
/// PLAN-00 deja solo el GET de la lista para probar que la persistencia funciona.
/// El CRUD completo, los DTOs y el endpoint de busqueda son de PLAN-01.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class LugaresController : ControllerBase
{
    private readonly AppDbContext _contexto;

    public LugaresController(AppDbContext contexto)
    {
        _contexto = contexto;
    }

    /// <summary>Devuelve todos los lugares registrados.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Lugar>>> ObtenerTodos()
    {
        var lugares = await _contexto.Lugares.ToListAsync();
        return Ok(lugares);
    }
}
