using Api.Data;
using Api.Dtos;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LugaresController : ControllerBase
{
    private readonly AppDbContext _contexto;

    public LugaresController(AppDbContext contexto)
    {
        _contexto = contexto;
    }

    /// <summary>
    /// Obtiene todos los lugares con sus reportes de accesibilidad.
    /// </summary>
    /// <response code="200">Lista de lugares obtenida correctamente.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LugarDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LugarDto>>> ObtenerTodos()
    {
        var lugares = await _contexto.Lugares
            .Include(l => l.Reportes)
                .ThenInclude(r => r.TipoBarrera)
            .Select(l => MapearALugarDto(l))
            .ToListAsync();

        return Ok(lugares);
    }

    /// <summary>
    /// Obtiene un lugar por su identificador.
    /// </summary>
    /// <response code="200">Lugar encontrado.</response>
    /// <response code="404">No existe un lugar con ese identificador.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(LugarDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LugarDto>> ObtenerPorId(int id)
    {
        var lugar = await _contexto.Lugares
            .Include(l => l.Reportes)
                .ThenInclude(r => r.TipoBarrera)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (lugar == null)
            return NotFound(new { mensaje = $"Lugar con ID {id} no encontrado" });

        return Ok(MapearALugarDto(lugar));
    }

    /// <summary>
    /// Filtra lugares por tipo, zona, si prestan servicio al ciudadano y si están
    /// libres de barreras críticas. Los cuatro filtros son opcionales y se combinan.
    /// </summary>
    /// <param name="tipo">Tipo de lugar, por ejemplo Parque o Hospital.</param>
    /// <param name="zona">Zona de Barranquilla donde está el lugar.</param>
    /// <param name="soloServicioCiudadano">Solo lugares que prestan servicio al ciudadano.</param>
    /// <param name="sinBarrerasCriticas">Solo lugares sin reportes de severidad Alta.</param>
    /// <response code="200">Lista de lugares que cumplen los filtros.</response>
    [HttpGet("buscar")]
    [ProducesResponseType(typeof(IEnumerable<LugarDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LugarDto>>> Buscar(
        [FromQuery] string? tipo,
        [FromQuery] string? zona,
        [FromQuery] bool? soloServicioCiudadano,
        [FromQuery] bool? sinBarrerasCriticas)
    {
        var query = _contexto.Lugares
            .Include(l => l.Reportes)
                .ThenInclude(r => r.TipoBarrera)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(tipo))
            query = query.Where(l => l.Tipo.ToLower() == tipo.ToLower());

        if (!string.IsNullOrWhiteSpace(zona))
            query = query.Where(l => l.Zona.ToLower() == zona.ToLower());

        if (soloServicioCiudadano == true)
            query = query.Where(l => l.EsServicioAlCiudadano);

        // Sin barreras críticas significa sin reportes de severidad Alta, no sin
        // reportes: un lugar con una barrera leve sigue siendo transitable.
        if (sinBarrerasCriticas == true)
            query = query.Where(l => !l.Reportes.Any(r => r.Severidad == NivelSeveridad.Alta));

        var resultados = await query.Select(l => MapearALugarDto(l)).ToListAsync();

        return Ok(resultados);
    }

    /// <summary>
    /// Registra un nuevo lugar.
    /// </summary>
    /// <response code="201">Lugar creado. Devuelve el recurso y su ubicación.</response>
    /// <response code="400">Los datos enviados no cumplen las validaciones.</response>
    [HttpPost]
    [ProducesResponseType(typeof(LugarDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LugarDto>> Crear([FromBody] LugarCrearDto dto)
    {
        var nuevoLugar = new Lugar
        {
            Nombre = dto.Nombre,
            Tipo = dto.Tipo,
            Zona = dto.Zona,
            Direccion = dto.Direccion,
            Latitud = dto.Latitud,
            Longitud = dto.Longitud,
            EsServicioAlCiudadano = dto.EsServicioAlCiudadano
        };

        _contexto.Lugares.Add(nuevoLugar);
        await _contexto.SaveChangesAsync();

        var resultadoDto = MapearALugarDto(nuevoLugar);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoLugar.Id }, resultadoDto);
    }

    /// <summary>
    /// Actualiza los datos de un lugar existente.
    /// </summary>
    /// <response code="204">Lugar actualizado.</response>
    /// <response code="400">Los datos enviados no cumplen las validaciones.</response>
    /// <response code="404">No existe un lugar con ese identificador.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Actualizar(int id, [FromBody] LugarActualizarDto dto)
    {
        var lugar = await _contexto.Lugares.FindAsync(id);

        if (lugar == null)
            return NotFound(new { mensaje = $"Lugar con ID {id} no encontrado" });

        lugar.Nombre = dto.Nombre;
        lugar.Tipo = dto.Tipo;
        lugar.Zona = dto.Zona;
        lugar.Direccion = dto.Direccion;
        lugar.Latitud = dto.Latitud;
        lugar.Longitud = dto.Longitud;
        lugar.EsServicioAlCiudadano = dto.EsServicioAlCiudadano;

        await _contexto.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Elimina un lugar y, en cascada, sus reportes asociados.
    /// </summary>
    /// <response code="204">Lugar eliminado.</response>
    /// <response code="404">No existe un lugar con ese identificador.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int id)
    {
        var lugar = await _contexto.Lugares.FindAsync(id);

        if (lugar == null)
            return NotFound(new { mensaje = $"Lugar con ID {id} no encontrado" });

        _contexto.Lugares.Remove(lugar);
        await _contexto.SaveChangesAsync();

        return NoContent();
    }

    private static LugarDto MapearALugarDto(Lugar l) => new LugarDto
    {
        Id = l.Id,
        Nombre = l.Nombre,
        Tipo = l.Tipo,
        Zona = l.Zona,
        Direccion = l.Direccion,
        Latitud = l.Latitud,
        Longitud = l.Longitud,
        EsServicioAlCiudadano = l.EsServicioAlCiudadano,
        TieneBarrerasCriticas = l.Reportes.Any(r => r.Severidad == NivelSeveridad.Alta),
        Reportes = l.Reportes.Select(r => new ReporteDto
        {
            Id = r.Id,
            LugarId = r.LugarId,
            Usuario = r.Usuario,
            Descripcion = r.Descripcion,
            Estado = r.Estado.ToString(),
            TipoBarrera = r.TipoBarrera?.Nombre,
            Severidad = r.Severidad?.ToString(),
            FechaReporte = r.FechaReporte
        }).ToList()
    };
}
