using System.ComponentModel.DataAnnotations;

namespace Api.Dtos
{
    public class LugarActualizarDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Tipo { get; set; } = string.Empty;

        [Required]
        public string Zona { get; set; } = string.Empty;

        [Required]
        public string Direccion { get; set; } = string.Empty;

        [Required]
        [Range(-90.0, 90.0)]
        public double Latitud { get; set; }

        [Required]
        [Range(-180.0, 180.0)]
        public double Longitud { get; set; }
    }
}