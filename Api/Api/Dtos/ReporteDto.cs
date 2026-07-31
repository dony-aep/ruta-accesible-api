namespace Api.Dtos
{
    public class ReporteDto
    {
        public int Id { get; set; }
        public int LugarId { get; set; }
        public string Descripción { get; set; } = string.Empty;
        public string TipoBarrera { get; set; } = string.Empty;
        public DateTime FechaCreación { get; set; }
    }
}