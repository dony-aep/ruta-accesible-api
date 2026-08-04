using System.ComponentModel.DataAnnotations;

namespace Api.Dtos;

/// <summary>
/// Datos para actualizar el estado de un reporte. Solo permite avanzar el estado
/// (Registrado -> Analizado -> Verificado -> Atendido), nunca retroceder.
/// No permite modificar los campos que asigna la IA.
/// </summary>
public class EditarReporteDto
{
    [Required(ErrorMessage = "El estado es obligatorio")]
    [MaxLength(20, ErrorMessage = "El estado no puede exceder 20 caracteres")]
    public string Estado { get; set; } = string.Empty;
}
