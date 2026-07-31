namespace Api.Dtos
{
    /// <summary>
    /// Reporte tal como lo expone la API dentro de un lugar. Los campos que completa
    /// el análisis con IA llegan nulos mientras el reporte siga en estado Registrado.
    /// </summary>
    public class ReporteDto
    {
        public int Id { get; set; }
        public int LugarId { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string? TipoBarrera { get; set; }
        public string? Severidad { get; set; }
        public DateTime FechaReporte { get; set; }
    }
}
