namespace Api.Dtos;

/// <summary>
/// Reporte completo tal como lo expone la API. Incluye los datos del lugar,
/// el tipo de barrera (si ya fue clasificado) y los resultados del análisis con IA.
/// </summary>
public class ReporteDetalleDto
{
    public int Id { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaReporte { get; set; }
    public string Estado { get; set; } = string.Empty;

    // Datos del lugar
    public int LugarId { get; set; }
    public string NombreLugar { get; set; } = string.Empty;
    public string ZonaLugar { get; set; } = string.Empty;

    // Datos del tipo de barrera (nulos si no ha sido analizado)
    public int? TipoBarreraId { get; set; }
    public string? CodigoCriterio { get; set; }
    public string? NombreCriterio { get; set; }

    // Resultados del análisis con IA (nulos si no ha sido analizado)
    public string? Severidad { get; set; }
    public string? AnalisisIa { get; set; }
    public string? AjusteRazonable { get; set; }
    public double? CertezaIa { get; set; }
}
