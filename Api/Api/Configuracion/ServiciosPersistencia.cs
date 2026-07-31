using Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Configuracion;

/// <summary>
/// Registro de la persistencia. Pertenece a PLAN-00 (Backend / TL).
/// </summary>
public static class ServiciosPersistencia
{
    public static IServiceCollection AgregarPersistencia(
        this IServiceCollection servicios,
        IConfiguration configuracion)
    {
        servicios.AddDbContext<AppDbContext>(opciones =>
            opciones.UseSqlite(configuracion.GetConnectionString("DefaultConnection")));

        return servicios;
    }
}
