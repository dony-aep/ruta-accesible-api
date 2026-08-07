namespace Api.Dtos
{
    public class EstadisticasDto
    {
        public string Zona { get; set; } = string.Empty;
        public string TipoBarrera { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }
}