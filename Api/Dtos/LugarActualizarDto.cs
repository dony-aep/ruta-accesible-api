using System.ComponentModel.DataAnnotations;

namespace Api.Dtos
{
    /// <summary>
    /// Datos que el cliente envía para actualizar un lugar. El PUT reemplaza el
    /// recurso completo, así que exige los mismos campos obligatorios que el POST.
    /// </summary>
    public class LugarActualizarDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo es obligatorio")]
        [MaxLength(80, ErrorMessage = "El tipo no puede exceder 80 caracteres")]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La zona es obligatoria")]
        [MaxLength(80, ErrorMessage = "La zona no puede exceder 80 caracteres")]
        public string Zona { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [MaxLength(200, ErrorMessage = "La dirección no puede exceder 200 caracteres")]
        public string Direccion { get; set; } = string.Empty;

        [Range(-90.0, 90.0, ErrorMessage = "La latitud debe estar entre -90 y 90")]
        public double Latitud { get; set; }

        [Range(-180.0, 180.0, ErrorMessage = "La longitud debe estar entre -180 y 180")]
        public double Longitud { get; set; }

        // Marca si la NTC 6047 aplica al lugar como punto de servicio al ciudadano.
        public bool EsServicioAlCiudadano { get; set; }
    }
}
