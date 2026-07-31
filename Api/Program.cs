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

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<Api.Data.AppDbContext>();
    await Api.Data.SeedData.InicializarAsync(context);
}

app.Run();
