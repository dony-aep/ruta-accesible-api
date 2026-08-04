using Api.Configuracion;

var builder = WebApplication.CreateBuilder(args);

// Persistencia: AppDbContext con SQLite. Ver Api/Configuracion/README.md.
builder.Services.AgregarPersistencia(builder.Configuration);

// Servicios de IA: HttpClient nombrado para Gemini y ServicioIa. Ver PLAN-02.
builder.Services.AgregarServiciosIa(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
