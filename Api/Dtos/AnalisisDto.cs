namespace Api.Dtos;

/// <summary>
/// Resultado del análisis con IA de un reporte de accesibilidad.
/// Contiene la clasificación contra la NTC 6047, la severidad estimada,
/// el ajuste razonable sugerido y el nivel de certeza del modelo.
/// </summary>
public class AnalisisDto
{
    public string CodigoCriterio { get; set; } = string.Empty;
    public string NombreCriterio { get; set; } = string.Empty;
    public string Severidad { get; set; } = string.Empty;
    public string AnalisisIa { get; set; } = string.Empty;
    public string AjusteRazonable { get; set; } = string.Empty;
    public double CertezaIa { get; set; }
}
