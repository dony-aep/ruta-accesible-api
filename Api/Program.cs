using Api.Configuracion;

var builder = WebApplication.CreateBuilder(args);

// Persistencia: AppDbContext con SQLite. Ver Api/Configuracion/README.md.
builder.Services.AgregarPersistencia(builder.Configuration);

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

app.Run();
