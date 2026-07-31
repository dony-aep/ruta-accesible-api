using System.ComponentModel.DataAnnotations;

namespace Api.Models;

/// <summary>
/// Espacio público de Barranquilla sobre el que los ciudadanos reportan barreras.
/// </summary>
public class Lugar
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Direccion { get; set; } = string.Empty;

    // Biblioteca publica, terminal, estacion de transporte, plaza, sede administrativa...
    [Required]
    [MaxLength(80)]
    public string Tipo { get; set; } = string.Empty;

    [Required]
    [MaxLength(80)]
    public string Zona { get; set; } = string.Empty;

    public double Latitud { get; set; }

    public double Longitud { get; set; }

    // La NTC 6047 aplica a los puntos de servicio al ciudadano. Marcarlo permite
    // distinguir donde el incumplimiento tiene consecuencia jurídica y no solo técnica.
    public bool EsServicioAlCiudadano { get; set; }

    // Propiedad de navegación: un lugar acumula muchos reportes
    public List<ReporteAccesibilidad> Reportes { get; set; } = new();
}
