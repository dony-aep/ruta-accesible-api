using System.Text.Json;
using Api.Data;
using Api.Dtos;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportesController : ControllerBase
{
    // La clasificación del modelo se presenta como sugerencia, nunca como dictamen
    // normativo: una salida mal clasificada podría orientar una decisión pública errada.
    private const string AdvertenciaIa =
        "Clasificación sugerida por un modelo de lenguaje. No constituye dictamen normativo " +
        "y requiere verificación técnica.";

    private readonly AppDbContext _contexto;
    private readonly ServicioIa _servicioIa;

    public ReportesController(AppDbContext contexto, ServicioIa servicioIa)
    {
        _contexto = contexto;
        _servicioIa = servicioIa;
    }

    /// <summary>
    /// Obtiene todos los reportes de accesibilidad.
    /// </summary>
    /// <response code="200">Lista de reportes obtenida correctamente.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ReporteDetalleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ReporteDetalleDto>>> ObtenerTodos()
    {
        var reportes = await _contexto.Reportes
            .Include(r => r.Lugar)
            .Include(r => r.TipoBarrera)
            .Select(r => MapearADto(r))
            .ToListAsync();

        return Ok(reportes);
    }

    /// <summary>
    /// Obtiene un reporte por su identificador.
    /// </summary>
    /// <response code="200">Reporte encontrado.</response>
    /// <response code="404">No existe un reporte con ese identificador.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ReporteDetalleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReporteDetalleDto>> ObtenerPorId(int id)
    {
        var reporte = await _contexto.Reportes
            .Include(r => r.Lugar)
            .Include(r => r.TipoBarrera)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reporte == null)
            return NotFound(new { mensaje = $"Reporte con ID {id} no encontrado" });

        return Ok(MapearADto(reporte));
    }

    /// <summary>
    /// Crea un nuevo reporte de accesibilidad.
    /// </summary>
    /// <response code="201">Reporte creado. Queda en estado Registrado.</response>
    /// <response code="400">Datos inválidos o el lugar indicado no existe.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ReporteDetalleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReporteDetalleDto>> Crear([FromBody] CrearReporteDto dto)
    {
        // Verificar que el lugar existe
        var lugar = await _contexto.Lugares.FindAsync(dto.LugarId);
        if (lugar == null)
            return BadRequest(new { mensaje = $"Lugar con ID {dto.LugarId} no encontrado" });

        var reporte = new ReporteAccesibilidad
        {
            Usuario = dto.Usuario,
            Descripcion = dto.Descripcion,
            LugarId = dto.LugarId,
            FechaReporte = DateTime.UtcNow,
            Estado = EstadoReporte.Registrado
        };

        _contexto.Reportes.Add(reporte);
        await _contexto.SaveChangesAsync();

        // Recargar con las relaciones para mapear el DTO completo
        await _contexto.Entry(reporte).Reference(r => r.Lugar).LoadAsync();

        return CreatedAtAction(nameof(ObtenerPorId), new { id = reporte.Id }, MapearADto(reporte));
    }

    /// <summary>
    /// Actualiza el estado de un reporte. Solo permite avanzar el estado
    /// (Registrado -> Analizado -> Verificado -> Atendido), nunca retroceder.
    /// No permite modificar los campos que asigna la IA.
    /// </summary>
    /// <response code="204">Estado actualizado.</response>
    /// <response code="400">Estado no válido, retroceso de estado o salto de etapas.</response>
    /// <response code="404">No existe un reporte con ese identificador.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Editar(int id, [FromBody] EditarReporteDto dto)
    {
        var reporte = await _contexto.Reportes.FindAsync(id);

        if (reporte == null)
            return NotFound(new { mensaje = $"Reporte con ID {id} no encontrado" });

        // Validar que el estado proporcionado es un valor del enum. TryParse por sí solo
        // acepta cualquier número, así que IsDefined descarta los que no son estados reales.
        if (!Enum.TryParse<EstadoReporte>(dto.Estado, ignoreCase: true, out var nuevoEstado)
            || !Enum.IsDefined(nuevoEstado))
            return BadRequest(new { mensaje = $"Estado no valido. Los estados permitidos son: Registrado, Analizado, Verificado, Atendido" });

        // Validar que no se retrocede ni se salta etapas
        if (nuevoEstado <= reporte.Estado)
            return BadRequest(new { mensaje = $"No se puede retroceder el estado. Estado actual: {reporte.Estado}, estado solicitado: {nuevoEstado}" });

        if ((int)nuevoEstado - (int)reporte.Estado > 1)
            return BadRequest(new { mensaje = $"No se puede saltar etapas. Estado actual: {reporte.Estado}, siguiente estado esperado: {(EstadoReporte)((int)reporte.Estado + 1)}" });

        reporte.Estado = nuevoEstado;
        await _contexto.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Elimina un reporte de accesibilidad.
    /// </summary>
    /// <response code="204">Reporte eliminado.</response>
    /// <response code="404">No existe un reporte con ese identificador.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int id)
    {
        var reporte = await _contexto.Reportes.FindAsync(id);

        if (reporte == null)
            return NotFound(new { mensaje = $"Reporte con ID {id} no encontrado" });

        _contexto.Reportes.Remove(reporte);
        await _contexto.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Clasifica un reporte contra la NTC 6047 usando el modelo de lenguaje.
    /// Carga el reporte con su lugar y tipo de barrera, arma el prompt con la
    /// taxonomía cerrada de la NTC 6047 y devuelve la clasificación.
    /// Si el servicio de IA falla, responde con un mensaje de servicio no disponible.
    /// </summary>
    /// <response code="200">
    /// Clasificación obtenida. Si el servicio de IA no respondió, devolvió un formato
    /// inesperado o propuso un criterio fuera de la norma, responde igualmente 200 con
    /// el motivo: el reporte queda registrado y puede analizarse más tarde.
    /// </response>
    /// <response code="404">No existe un reporte con ese identificador.</response>
    [HttpPost("{id}/analizar")]
    [ProducesResponseType(typeof(AnalisisDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AnalisisDto>> Analizar(int id)
    {
        var reporte = await _contexto.Reportes
            .Include(r => r.Lugar)
            .Include(r => r.TipoBarrera)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reporte == null)
            return NotFound(new { mensaje = $"Reporte con ID {id} no encontrado" });

        // Cargar los tipos de barrera para incluirlos en el prompt
        var tiposBarrera = await _contexto.TiposBarrera.ToListAsync();

        var prompt = ConstruirPrompt(reporte, tiposBarrera);

        var respuestaIa = await _servicioIa.AnalizarAsync(prompt);

        // Si el servicio falla, degradar sin guardar nada: el reporte del ciudadano
        // no se pierde y puede analizarse más tarde.
        if (respuestaIa == null)
            return SinAnalisis(reporte.Id, "Servicio de IA no disponible");

        AnalisisDto? analisis;

        try
        {
            // Deserializar la respuesta JSON del modelo
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            analisis = JsonSerializer.Deserialize<AnalisisDto>(respuestaIa, opciones);
        }
        catch (JsonException)
        {
            return SinAnalisis(reporte.Id, "El servicio de IA respondió en un formato inesperado");
        }

        if (analisis == null)
            return SinAnalisis(reporte.Id, "El servicio de IA respondió en un formato inesperado");

        // Buscar el tipo de barrera por código. Si el modelo devolvió un criterio que no
        // está en el catálogo de la NTC 6047, se lo inventó: no se guarda nada y se avisa.
        // Es la mitigación del riesgo de alucinación normativa que exige el PLAN-02.
        var tipoBarrera = tiposBarrera
            .FirstOrDefault(t => t.Codigo.Equals(analisis.CodigoCriterio, StringComparison.OrdinalIgnoreCase));

        if (tipoBarrera == null)
            return SinAnalisis(reporte.Id, "El servicio de IA propuso un criterio que no existe en la NTC 6047");

        // Guardar los resultados del análisis en el reporte
        reporte.TipoBarreraId = tipoBarrera.Id;
        reporte.TipoBarrera = tipoBarrera;
        analisis.NombreCriterio = tipoBarrera.Nombre;

        if (Enum.TryParse<NivelSeveridad>(analisis.Severidad, ignoreCase: true, out var severidad))
            reporte.Severidad = severidad;

        reporte.AnalisisIa = analisis.AnalisisIa;
        reporte.AjusteRazonable = analisis.AjusteRazonable;
        reporte.CertezaIa = analisis.CertezaIa;

        // Solo avanza desde Registrado. Reanalizar un reporte ya verificado o atendido
        // no puede retroceder su estado: es la misma regla que aplica el PUT.
        if (reporte.Estado == EstadoReporte.Registrado)
            reporte.Estado = EstadoReporte.Analizado;

        await _contexto.SaveChangesAsync();

        analisis.ReporteId = reporte.Id;
        analisis.Advertencia = AdvertenciaIa;

        return Ok(analisis);
    }

    /// <summary>
    /// Respuesta cuando no hubo clasificación válida: el servicio no respondió, devolvió
    /// un formato inesperado o propuso un criterio fuera de la norma. Responde 200 porque
    /// el reporte sigue registrado; solo el análisis queda pendiente.
    /// </summary>
    private ObjectResult SinAnalisis(int reporteId, string analisis) => Ok(new
    {
        reporteId,
        analisis,
        mensaje = "El reporte quedó registrado y puede analizarse más tarde."
    });

    /// <summary>
    /// Construye el prompt para el modelo de lenguaje con la taxonomía cerrada
    /// de la NTC 6047 y los datos del reporte. El modelo debe elegir de la lista,
    /// nunca inventar criterios.
    /// </summary>
    private static string ConstruirPrompt(
        ReporteAccesibilidad reporte,
        List<TipoBarrera> tiposBarrera)
    {
        var catalogoCriterios = string.Join("\n", tiposBarrera.Select(t =>
            $"- Codigo: {t.Codigo}, Nombre: {t.Nombre}, Criterio: {t.CriterioNorma}"));

        var nombreLugar = reporte.Lugar?.Nombre ?? "No especificado";
        var direccionLugar = reporte.Lugar?.Direccion ?? "Sin direccion";
        var zonaLugar = reporte.Lugar?.Zona ?? "desconocida";

        return "Eres un experto en accesibilidad urbana y en la Norma Tecnica Colombiana NTC 6047:2013. " +
            "Tu tarea es clasificar un reporte ciudadano de barrera de accesibilidad.\n\n" +
            "REPORTE:\n" +
            $"- Descripcion: {reporte.Descripcion}\n" +
            $"- Lugar: {nombreLugar} ({direccionLugar}, zona {zonaLugar})\n\n" +
            "CATALOGO CERRADO DE CRITERIOS DE LA NTC 6047 (debes elegir EXCLUSIVAMENTE de esta lista):\n" +
            $"{catalogoCriterios}\n\n" +
            "REGLAS:\n" +
            "1. Elige el criterio mas cercano de la lista anterior. NO inventes criterios nuevos.\n" +
            "2. Asigna una severidad: Alta (impide el acceso), Media (dificulta pero no impide), Baja (inconveniente menor).\n" +
            "3. Sugiere un ajuste razonable concreto y realizable. Usa lenguaje de sugerencia, nunca afirmes medidas numericas como si fueran de la norma.\n" +
            "4. Justifica tu clasificacion en maximo 3 oraciones.\n" +
            "5. Declara tu nivel de certeza entre 0.0 y 1.0. Si el reporte no encaja claramente, usa un valor menor a 0.5.\n\n" +
            "Responde UNICAMENTE con un JSON valido con esta estructura exacta:\n" +
            "{\n" +
            "  \"codigoCriterio\": \"NTC-CODIGO\",\n" +
            "  \"nombreCriterio\": \"Nombre del criterio\",\n" +
            "  \"severidad\": \"Alta|Media|Baja\",\n" +
            "  \"analisisIa\": \"Justificacion breve\",\n" +
            "  \"ajusteRazonable\": \"Sugerencia concreta\",\n" +
            "  \"certezaIa\": 0.0\n" +
            "}";
    }

    /// <summary>
    /// Mapea una entidad ReporteAccesibilidad a su DTO de respuesta.
    /// </summary>
    private static ReporteDetalleDto MapearADto(ReporteAccesibilidad r) => new()
    {
        Id = r.Id,
        Usuario = r.Usuario,
        Descripcion = r.Descripcion,
        FechaReporte = r.FechaReporte,
        Estado = r.Estado.ToString(),
        LugarId = r.LugarId,
        NombreLugar = r.Lugar?.Nombre ?? string.Empty,
        ZonaLugar = r.Lugar?.Zona ?? string.Empty,
        TipoBarreraId = r.TipoBarreraId,
        CodigoCriterio = r.TipoBarrera?.Codigo,
        NombreCriterio = r.TipoBarrera?.Nombre,
        Severidad = r.Severidad?.ToString(),
        AnalisisIa = r.AnalisisIa,
        AjusteRazonable = r.AjusteRazonable,
        CertezaIa = r.CertezaIa
    };
}
