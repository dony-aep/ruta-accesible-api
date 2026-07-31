using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public static class SeedData
    {
        public static async Task InicializarAsync(AppDbContext context)
        {
            if (await context.Lugares.AnyAsync()) return;

            var lugares = new List<Lugar>
            {
                new Lugar
                {
                    Nombre = "Centro Administrativo Distrital (Alcaldía)",
                    Tipo = "Sede administrativa",
                    Zona = "Centro",
                    Direccion = "Calle 34 # 43-31",
                    Latitud = 10.9789,
                    Longitud = -74.7780,
                    Reportes = new List<ReporteAccesibilidad>
                    {
                        new ReporteAccesibilidad 
                        { 
                            Descripcion = "Rampa de acceso principal con pendiente superior al 12% sin pasamanos.", 
                            FechaReporte = DateTime.UtcNow 
                        }
                    }
                },
                new Lugar
                {
                    Nombre = "Biblioteca Departamental Meira Delmar",
                    Tipo = "Biblioteca pública",
                    Zona = "Centro",
                    Direccion = "Calle 38 # 38-86",
                    Latitud = 10.9812,
                    Longitud = -74.7815,
                    Reportes = new List<ReporteAccesibilidad>
                    {
                        new ReporteAccesibilidad 
                        { 
                            Descripcion = "Falta de señalización en sistema Braille en la entrada del salón principal.", 
                            FechaReporte = DateTime.UtcNow 
                        }
                    }
                },
                new Lugar
                {
                    Nombre = "Gran Malecón del Río",
                    Tipo = "Espacio público",
                    Zona = "Norte",
                    Direccion = "Sector Puerta de Oro",
                    Latitud = 11.0185,
                    Longitud = -74.8010,
                    Reportes = new List<ReporteAccesibilidad>()
                },
                new Lugar
                {
                    Nombre = "Estadio Metropolitano Roberto Meléndez",
                    Tipo = "Escenario deportivo",
                    Zona = "Sur",
                    Direccion = "Calle 45 con Av. Murillo",
                    Latitud = 10.9258,
                    Longitud = -74.8008,
                    Reportes = new List<ReporteAccesibilidad>
                    {
                        new ReporteAccesibilidad 
                        { 
                            Descripcion = "Baños adaptados fuera de servicio durante eventos masivos.", 
                            FechaReporte = DateTime.UtcNow 
                        }
                    }
                },
                new Lugar
                {
                    Nombre = "Universidad del Atlántico, sede norte",
                    Tipo = "Universidad pública",
                    Zona = "Norte",
                    Direccion = "Km 7 Vía a Puerto Colombia",
                    Latitud = 11.0198,
                    Longitud = -74.8722,
                    Reportes = new List<ReporteAccesibilidad>()
                }
            };

            await context.Lugares.AddRangeAsync(lugares);
            await context.SaveChangesAsync();
        }
    }
}