# Api/Configuracion

`Program.cs` es el archivo con más probabilidad de generar conflictos de merge del proyecto:
los cinco frentes de trabajo necesitan registrar algo en él. Por eso **solo lo toca el TL**.

Cada rol crea aquí su propio archivo de extensión y le pide al TL una sola línea de llamada en
`Program.cs`. Así dos personas nunca editan el mismo archivo.

| Archivo | Dueño | Registra |
|---|---|---|
| `ServiciosPersistencia.cs` | PLAN-00 | `AppDbContext` con SQLite |
| `ServiciosDatos.cs` | PLAN-01 | Seed data |
| `ServiciosIa.cs` | PLAN-02 | `HttpClient` nombrado y el servicio del modelo de lenguaje |

## Forma que deben seguir

```csharp
namespace Api.Configuracion;

public static class ServiciosLoQueSea
{
    public static IServiceCollection AgregarLoQueSea(
        this IServiceCollection servicios,
        IConfiguration configuracion)
    {
        // registros aqui
        return servicios;
    }
}
```

Y en `Program.cs`, una sola línea que agrega el TL:

```csharp
builder.Services.AgregarLoQueSea(builder.Configuration);
```
