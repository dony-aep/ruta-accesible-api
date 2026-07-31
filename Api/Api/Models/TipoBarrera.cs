using System.ComponentModel.DataAnnotations;

namespace Api.Models;

/// <summary>
/// Criterio de accesibilidad de la NTC 6047:2013. Es el catálogo cerrado contra el
/// que el modelo de lenguaje clasifica cada reporte.
/// </summary>
public class TipoBarrera
{
    public int Id { get; set; }

    // Identificador de negocio, por ejemplo NTC-RAMPAS
    [Required]
    [MaxLength(40)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [MaxLength(120)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [MaxLength(400)]
    public string CriterioNorma { get; set; } = string.Empty;

    // Propiedad de navegación: un criterio clasifica muchos reportes
    public List<ReporteAccesibilidad> Reportes { get; set; } = new();
}
