namespace Api.Dtos
{
    public class TipoBarreraDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string CriterioNorma { get; set; } = string.Empty;
    }
}