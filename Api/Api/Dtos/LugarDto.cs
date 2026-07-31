namespace Api.Dtos
{
    public class LugarDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Zona { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public bool TieneBarrerasCriticas { get; set; }
        public List<ReporteDto> Reportes { get; set; } = new();
    }
}