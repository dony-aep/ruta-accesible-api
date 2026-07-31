using System.ComponentModel.DataAnnotations;

namespace Api.Models;

/// <summary>
/// Recurso principal de la API. Lo crea el ciudadano con una descripcion en texto
/// libre y lo enriquece el endpoint de analisis con IA.
/// </summary>
public class ReporteAccesibilidad
{
    public int Id { get; set; }

    public int LugarId { get; set; }

    public Lugar? Lugar { get; set; }

    // Nulo al crear. Lo asigna el endpoint de analisis: exigirle al ciudadano que
    // conozca la taxonomia de una norma tecnica trasladaria el problema.
    public int? TipoBarreraId { get; set; }

    public TipoBarrera? TipoBarrera { get; set; }

    // Seudonimo de quien reporta. Nunca datos personales reales.
    [Required]
    [MaxLength(60)]
    public string Usuario { get; set; } = string.Empty;

    public DateTime FechaReporte { get; set; } = DateTime.UtcNow;

    // Texto libre del ciudadano. Es lo que alimenta el analisis con IA.
    [Required]
    [MaxLength(1000)]
    public string Descripcion { get; set; } = string.Empty;

    public EstadoReporte Estado { get; set; } = EstadoReporte.Registrado;

    // Los cuatro campos siguientes los completa el endpoint de analisis con IA.
    public NivelSeveridad? Severidad { get; set; }

    [MaxLength(1000)]
    public string? AnalisisIa { get; set; }

    [MaxLength(600)]
    public string? AjusteRazonable { get; set; }

    // Certeza declarada por el modelo, entre 0 y 1.
    public double? CertezaIa { get; set; }
}
