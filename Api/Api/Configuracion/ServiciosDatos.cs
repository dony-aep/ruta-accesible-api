using Api.Data;

namespace Api.Configuracion
{
    public static class ServiciosDatos
    {
        public static async Task CargarSeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await SeedData.InicializarAsync(context);
        }
    }
}