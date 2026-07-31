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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LugarDto>>> ObtenerTodos()
    {
        var lugares = await _contexto.Lugares
            .Include(l => l.Reportes)
            .Select(l => MapearALugarDto(l))
            .ToListAsync();

        return Ok(lugares);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LugarDto>> ObtenerPorId(int id)
    {
        var lugar = await _contexto.Lugares
            .Include(l => l.Reportes)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (lugar == null)
            return NotFound(new { mensaje = $"Lugar con ID {id} no encontrado" });

        return Ok(MapearALugarDto(lugar));
    }

    [HttpGet("buscar")]
    public async Task<ActionResult<IEnumerable<LugarDto>>> Buscar(
        [FromQuery] string? tipo,
        [FromQuery] string? zona,
        [FromQuery] bool? sinBarrerasCriticas)
    {
        var query = _contexto.Lugares.Include(l => l.Reportes).AsQueryable();

        if (!string.IsNullOrWhiteSpace(tipo))
            query = query.Where(l => l.Tipo.ToLower() == tipo.ToLower());

        if (!string.IsNullOrWhiteSpace(zona))
            query = query.Where(l => l.Zona.ToLower() == zona.ToLower());

        if (sinBarrerasCriticas.HasValue && sinBarrerasCriticas.Value)
            query = query.Where(l => !l.Reportes.Any());

        var resultados = await query.Select(l => MapearALugarDto(l)).ToListAsync();

        return Ok(resultados);
    }

    [HttpPost]
    public async Task<ActionResult<LugarDto>> Crear([FromBody] LugarCrearDto dto)
    {
        var nuevoLugar = new Lugar
        {
            Nombre = dto.Nombre,
            Tipo = dto.Tipo,
            Zona = dto.Zona,
            Direccion = dto.Direccion,
            Latitud = dto.Latitud,
            Longitud = dto.Longitud
        };

        _contexto.Lugares.Add(nuevoLugar);
        await _contexto.SaveChangesAsync();

        var resultadoDto = MapearALugarDto(nuevoLugar);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoLugar.Id }, resultadoDto);
    }

    [HttpPut("{id}")]
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

        await _contexto.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
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
        Nombre = l.Nombre ?? string.Empty,
        Tipo = l.Tipo ?? string.Empty,
        Zona = l.Zona ?? string.Empty,
        Direccion = l.Direccion ?? string.Empty,
        Latitud = (decimal)l.Latitud,
        Longitud = (decimal)l.Longitud,
        TieneBarrerasCriticas = l.Reportes != null && l.Reportes.Any(),
        Reportes = l.Reportes?.Select(r => new ReporteDto
        {
            Id = r.Id,
            LugarId = r.LugarId,
            Descripción = r.Descripcion ?? string.Empty,
            TipoBarrera = r.TipoBarrera?.Nombre ?? "General",
            FechaCreación = r.FechaReporte
        }).ToList() ?? new List<ReporteDto>()
    };
}













