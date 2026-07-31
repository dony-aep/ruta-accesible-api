using System.ComponentModel.DataAnnotations;

namespace Api.Dtos
{
    public class LugarCrearDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo es obligatorio")]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La zona es obligatoria")]
        public string Zona { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es obligatoria")]
        public string Direccion { get; set; } = string.Empty;

        [Required]
        [Range(-90.0, 90.0, ErrorMessage = "La latitud debe estar entre -90 y 90")]
        public double Latitud { get; set; }

        [Required]
        [Range(-180.0, 180.0, ErrorMessage = "La longitud debe estar entre -180 y 180")]
        public double Longitud { get; set; }
    }
}