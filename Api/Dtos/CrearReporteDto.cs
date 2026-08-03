using System.ComponentModel.DataAnnotations;

namespace Api.Dtos;

/// <summary>
/// Datos que envía el ciudadano para crear un reporte de barrera.
/// No incluye TipoBarreraId porque lo asigna el endpoint de análisis con IA,
/// no el ciudadano.
/// </summary>
public class CrearReporteDto
{
    [Required(ErrorMessage = "El usuario es obligatorio")]
    [MaxLength(60, ErrorMessage = "El usuario no puede exceder 60 caracteres")]
    public string Usuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es obligatoria")]
    [MaxLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El lugar es obligatorio")]
    public int LugarId { get; set; }
}
