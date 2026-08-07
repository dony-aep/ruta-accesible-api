using Api.Data;
using Api.Dtos;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TiposBarreraController : ControllerBase
{
    private readonly AppDbContext _contexto;

    public TiposBarreraController(AppDbContext contexto)
    {
        _contexto = contexto;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TipoBarreraDto>>> ObtenerTodos()
    {
        var tipos = await _contexto.TiposBarrera
            .Select(t => MapearATipoBarreraDto(t))
            .ToListAsync();

        return Ok(tipos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TipoBarreraDto>> ObtenerPorId(int id)
    {
        var tipo = await _contexto.TiposBarrera.FindAsync(id);

        if (tipo == null)
            return NotFound(new { mensaje = $"Tipo de barrera con ID {id} no encontrado" });

        return Ok(MapearATipoBarreraDto(tipo));
    }

    [HttpPost]
    public async Task<ActionResult<TipoBarreraDto>> Crear([FromBody] TipoBarreraCrearDto dto)
    {
        // El código tiene índice único en AppDbContext: sin esta comprobación previa
        // el duplicado revienta en SaveChangesAsync y sale como 500.
        var codigoEnUso = await _contexto.TiposBarrera
            .AnyAsync(t => t.Codigo == dto.Codigo);

        if (codigoEnUso)
            return BadRequest(new { mensaje = $"Ya existe un tipo de barrera con el código {dto.Codigo}" });

        var nuevoTipo = new TipoBarrera
        {
            Codigo = dto.Codigo,
            Nombre = dto.Nombre,
            CriterioNorma = dto.CriterioNorma
        };

        _contexto.TiposBarrera.Add(nuevoTipo);
        await _contexto.SaveChangesAsync();

        var resultadoDto = MapearATipoBarreraDto(nuevoTipo);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoTipo.Id }, resultadoDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] TipoBarreraCrearDto dto)
    {
        var tipo = await _contexto.TiposBarrera.FindAsync(id);

        if (tipo == null)
            return NotFound(new { mensaje = $"Tipo de barrera con ID {id} no encontrado" });

        // Mismo índice único que en el POST, excluyendo el propio registro para que
        // se pueda editar el nombre o el criterio sin cambiar el código.
        var codigoEnUso = await _contexto.TiposBarrera
            .AnyAsync(t => t.Codigo == dto.Codigo && t.Id != id);

        if (codigoEnUso)
            return BadRequest(new { mensaje = $"Ya existe otro tipo de barrera con el código {dto.Codigo}" });

        tipo.Codigo = dto.Codigo;
        tipo.Nombre = dto.Nombre;
        tipo.CriterioNorma = dto.CriterioNorma;

        await _contexto.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var tipo = await _contexto.TiposBarrera.FindAsync(id);

        if (tipo == null)
            return NotFound(new { mensaje = $"Tipo de barrera con ID {id} no encontrado" });

        // Borrado protegido: la relación está en Restrict, así que borrar un criterio
        // en uso fallaría en la base. Se responde 400 con el motivo en vez de dejarlo
        // reventar.
        var tieneReportes = await _contexto.Reportes
            .AnyAsync(r => r.TipoBarreraId == id);

        if (tieneReportes)
            return BadRequest(new { mensaje = "No se puede eliminar el tipo de barrera porque tiene reportes asociados" });

        _contexto.TiposBarrera.Remove(tipo);
        await _contexto.SaveChangesAsync();

        return NoContent();
    }

    private static TipoBarreraDto MapearATipoBarreraDto(TipoBarrera t) => new TipoBarreraDto
    {
        Id = t.Id,
        Codigo = t.Codigo,
        Nombre = t.Nombre,
        CriterioNorma = t.CriterioNorma
    };
}
