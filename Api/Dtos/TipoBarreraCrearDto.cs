using System.ComponentModel.DataAnnotations;

namespace Api.Dtos
{
    public class TipoBarreraCrearDto
    {
        [Required(ErrorMessage = "El código es obligatorio.")]
        [StringLength(40, ErrorMessage = "El código no puede exceder los 40 caracteres.")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(120, ErrorMessage = "El nombre no puede exceder los 120 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El criterio de la norma es obligatorio.")]
        [StringLength(400, ErrorMessage = "El criterio de la norma no puede exceder los 400 caracteres.")]
        public string CriterioNorma { get; set; } = string.Empty;
    }
}