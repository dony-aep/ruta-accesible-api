namespace Api.Dtos
{
    /// <summary>
    /// Lugar tal como lo expone la API, con sus reportes asociados.
    /// </summary>
    public class LugarDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Zona { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public bool EsServicioAlCiudadano { get; set; }

        // Un lugar es crítico si al menos uno de sus reportes fue clasificado con
        // severidad Alta por el análisis con IA.
        public bool TieneBarrerasCriticas { get; set; }

        public List<ReporteDto> Reportes { get; set; } = new();
    }
}
