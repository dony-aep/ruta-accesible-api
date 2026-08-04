using Api.Services;

namespace Api.Configuracion;

/// <summary>
/// Registro del servicio de IA y del HttpClient nombrado para Gemini.
/// Pertenece a PLAN-02 (API / IA). La línea de llamada en Program.cs la agrega el TL.
/// </summary>
public static class ServiciosIa
{
    public static IServiceCollection AgregarServiciosIa(
        this IServiceCollection servicios,
        IConfiguration configuracion)
    {
        // Cliente nombrado para Gemini con el endpoint compatible con OpenAI.
        // La URL base y la clave se leen de configuración y user-secrets.
        servicios.AddHttpClient("Gemini", cliente =>
        {
            var urlBase = configuracion["Ia:UrlBase"]
                ?? "https://generativelanguage.googleapis.com/v1beta/openai/";

            cliente.BaseAddress = new Uri(urlBase);

            // Timeout corto: si el proveedor se cuelga, es preferible degradar rápido
            // a dejar la petición esperando los 100 segundos que trae por defecto.
            cliente.Timeout = TimeSpan.FromSeconds(30);

            var apiKey = configuracion["Ia:ApiKey"] ?? string.Empty;
            cliente.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        });

        // Registrar el servicio de IA como transient: cada petición crea su instancia
        // y el HttpClient lo gestiona el factory.
        servicios.AddTransient<ServicioIa>();

        return servicios;
    }
}
