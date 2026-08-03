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
    [HttpGet]
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
    [HttpGet("{id}")]
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
    [HttpPost]
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
    [HttpPut("{id}")]
    public async Task<IActionResult> Editar(int id, [FromBody] EditarReporteDto dto)
    {
        var reporte = await _contexto.Reportes.FindAsync(id);

        if (reporte == null)
            return NotFound(new { mensaje = $"Reporte con ID {id} no encontrado" });

        // Validar que el estado proporcionado es un valor del enum
        if (!Enum.TryParse<EstadoReporte>(dto.Estado, ignoreCase: true, out var nuevoEstado))
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
    [HttpDelete("{id}")]
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
    [HttpPost("{id}/analizar")]
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

        // Si el servicio falla, devolver respuesta de degradación sin guardar en la base
        if (respuestaIa == null)
        {
            return Ok(new AnalisisDto
            {
                CodigoCriterio = "Servicio de IA no disponible",
                NombreCriterio = "Servicio de IA no disponible",
                Severidad = "Servicio de IA no disponible",
                AnalisisIa = "Servicio de IA no disponible",
                AjusteRazonable = "Servicio de IA no disponible",
                CertezaIa = 0.0
            });
        }

        try
        {
            // Deserializar la respuesta JSON del modelo
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var analisis = JsonSerializer.Deserialize<AnalisisDto>(respuestaIa, opciones);

            if (analisis == null)
            {
                return Ok(new AnalisisDto
                {
                    CodigoCriterio = "Servicio de IA no disponible",
                    NombreCriterio = "Servicio de IA no disponible",
                    Severidad = "Servicio de IA no disponible",
                    AnalisisIa = "Servicio de IA no disponible",
                    AjusteRazonable = "Servicio de IA no disponible",
                    CertezaIa = 0.0
                });
            }

            // Buscar el tipo de barrera por código para asignarlo al reporte
            var tipoBarrera = tiposBarrera
                .FirstOrDefault(t => t.Codigo.Equals(analisis.CodigoCriterio, StringComparison.OrdinalIgnoreCase));

            // Guardar los resultados del análisis en el reporte
            if (tipoBarrera != null)
            {
                reporte.TipoBarreraId = tipoBarrera.Id;
                reporte.TipoBarrera = tipoBarrera;
                analisis.NombreCriterio = tipoBarrera.Nombre;
            }

            if (Enum.TryParse<NivelSeveridad>(analisis.Severidad, ignoreCase: true, out var severidad))
                reporte.Severidad = severidad;

            reporte.AnalisisIa = analisis.AnalisisIa;
            reporte.AjusteRazonable = analisis.AjusteRazonable;
            reporte.CertezaIa = analisis.CertezaIa;
            reporte.Estado = EstadoReporte.Analizado;

            await _contexto.SaveChangesAsync();

            return Ok(analisis);
        }
        catch (JsonException)
        {
            return Ok(new AnalisisDto
            {
                CodigoCriterio = "Servicio de IA no disponible",
                NombreCriterio = "Servicio de IA no disponible",
                Severidad = "Servicio de IA no disponible",
                AnalisisIa = "Servicio de IA no disponible",
                AjusteRazonable = "Servicio de IA no disponible",
                CertezaIa = 0.0
            });
        }
    }

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
