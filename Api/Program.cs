using System.Reflection;
using Api.Configuracion;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Persistencia: AppDbContext con SQLite. Ver Api/Configuracion/README.md.
builder.Services.AgregarPersistencia(builder.Configuration);

// Servicios de IA: HttpClient nombrado para Gemini y ServicioIa. Ver PLAN-02.
builder.Services.AgregarServiciosIa(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Ruta Accesible",
        Version = "v1",
        Description =
            "API de reporte y clasificación de barreras de accesibilidad en lugares públicos " +
            "de Barranquilla. El endpoint de análisis clasifica el reporte contra los criterios " +
            "de la NTC 6047 con un modelo de lenguaje; su salida es una sugerencia, no un " +
            "dictamen normativo."
    });

    // Descripciones y códigos de respuesta que se escriben como comentarios XML
    // en los controladores. Requiere GenerateDocumentationFile en Api.csproj.
    var archivoXml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    opciones.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, archivoXml));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Seed inicial: catálogo de la NTC 6047, lugares y reportes. Ver PLAN-01.
await ServiciosDatos.CargarSeedAsync(app.Services);

app.Run();
