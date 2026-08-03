using System.Text;
using System.Text.Json;

namespace Api.Services;

/// <summary>
/// Servicio que conecta con la API de Gemini (endpoint compatible con OpenAI)
/// para analizar reportes de accesibilidad contra la NTC 6047.
/// Usa IHttpClientFactory con cliente nombrado, nunca new HttpClient().
/// </summary>
public class ServicioIa
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuracion;
    private readonly ILogger<ServicioIa> _logger;

    public ServicioIa(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuracion,
        ILogger<ServicioIa> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuracion = configuracion;
        _logger = logger;
    }

    /// <summary>
    /// Envía un prompt al modelo de lenguaje y devuelve la respuesta como texto.
    /// La llamada va en try/catch: si falla, devuelve null en vez de propagar la excepción.
    /// </summary>
    public async Task<string?> AnalizarAsync(string prompt)
    {
        try
        {
            var cliente = _httpClientFactory.CreateClient("Gemini");
            var modelo = _configuracion["Ia:Modelo"] ?? "gemini-2.0-flash";

            var cuerpo = new
            {
                model = modelo,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                temperature = 0.2,
                response_format = new { type = "json_object" }
            };

            var json = JsonSerializer.Serialize(cuerpo);
            var contenido = new StringContent(json, Encoding.UTF8, "application/json");

            var respuesta = await cliente.PostAsync("chat/completions", contenido);
            respuesta.EnsureSuccessStatusCode();

            var cuerpoRespuesta = await respuesta.Content.ReadAsStringAsync();

            // Extraer el contenido del mensaje de la respuesta compatible con OpenAI
            using var documento = JsonDocument.Parse(cuerpoRespuesta);
            var mensaje = documento.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return mensaje;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al llamar al servicio de IA");
            return null;
        }
    }
}
