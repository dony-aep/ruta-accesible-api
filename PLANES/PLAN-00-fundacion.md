# PLAN-00 — Fundación del proyecto

| | |
|---|---|
| Rol | Backend / TL |
| Responsable | @dony-aep |
| Rama | `plan-00-fundacion` |
| Depende de | Nada. Es el punto de partida |
| Desbloquea | PLAN-01, PLAN-02 y PLAN-03 |
| Fecha límite | Antes del check-in 1 (tras clase 17) |

## Identidad del repositorio

Decidido por el TL. Es lo primero que se crea y lo que se comparte con el profesor.

| | |
|---|---|
| Nombre | `ruta-accesible-api` |
| URL | `github.com/dony-aep/ruta-accesible-api` |
| Visibilidad | Público (lo exige el curso) |
| Descripción (campo *About* de GitHub) | API REST en ASP.NET Core que clasifica reportes ciudadanos de barreras de accesibilidad urbana contra la NTC 6047 con un modelo de lenguaje. ODS 11.7 y 10.2. |
| Topics | `aspnet-core`, `dotnet`, `entity-framework-core`, `sqlite`, `rest-api`, `accesibilidad`, `ods11` |
| Nombre de la solución | `RutaAccesible.sln` |
| Proyecto | `Api/` (ensamblado `Api`) |
| Rama por defecto | `main` |

El nombre sigue la convención del profesor en sus ejemplos (`eco-puntos-api`,
`sistema-biblioteca`): minúsculas, guiones, sufijo `-api`.

## Precondiciones

- [ ] Idea y entidades aprobadas en la reunión del jueves 30. **Hecho:** taller de definición
      entregado el 30 de julio (`../docs/especificacion.md`).
- [ ] Repositorio `ruta-accesible-api` creado en GitHub, público, con descripción y topics.
- [ ] Los 4 compañeros invitados como colaboradores con permiso de escritura.
- [ ] `PLANES/`, `.github/CODEOWNERS` y `.github/pull_request_template.md` copiados a la raíz
      del repositorio, con los roles ya asignados.
- [ ] Protección de `main` activada: sin push directo, PR con al menos una aprobación.

## Archivos que este plan posee

Nadie más los modifica mientras el plan esté abierto:

    RutaAccesible.sln
    Api/Api.csproj
    Api/Program.cs
    Api/Models/*.cs
    Api/Data/AppDbContext.cs
    Api/Migrations/*
    .gitignore
    appsettings.json

## Tareas

1. Crear solución y proyecto: `dotnet new sln -n RutaAccesible`, `dotnet new webapi -f net8.0 -n Api`,
   `dotnet sln add Api/Api.csproj`
   → verificar: `dotnet build` sin errores.
2. Agregar paquetes de EF Core:
   `dotnet add Api package Microsoft.EntityFrameworkCore.Sqlite` y
   `dotnet add Api package Microsoft.EntityFrameworkCore.Design`
   → verificar: `dotnet build`.
3. Crear `.gitignore` de .NET que excluya `bin/`, `obj/`, `*.db`, `*.db-shm`, `*.db-wal` y
   `appsettings.Development.json`
   → verificar: `git status` no lista binarios ni la base de datos.
4. Definir los modelos en `Api/Models/`: `Lugar`, `TipoBarrera` y `ReporteAccesibilidad`,
   con Data Annotations básicas (`[Required]`, `[MaxLength]`) y propiedades de navegación
   → verificar: `dotnet build`.
5. Crear `Api/Data/AppDbContext.cs` con los tres `DbSet` y las dos relaciones 1:N
   configuradas en `OnModelCreating`
   → verificar: `dotnet build`.
6. Registrar `AddDbContext` con SQLite en `Program.cs`, con la cadena de conexión en
   `appsettings.json`, y dejar Swagger habilitado
   → verificar: `dotnet run` levanta la API y `/swagger` carga.
7. Generar y aplicar la primera migración: `dotnet ef migrations add Inicial --project Api`
   y `dotnet ef database update --project Api`
   → verificar: ambos comandos terminan sin error y existe `Api/Migrations/`.
8. Exponer `GET /api/lugares` en `Api/Controllers/LugaresController.cs` devolviendo la lista
   desde la base de datos (solo este método; el CRUD completo es de PLAN-01)
   → verificar: responde 200 desde Swagger.
9. Crear la carpeta `Api/Configuracion/` con un archivo de ejemplo de extensión, para que
   los demás roles registren ahí sus servicios sin tocar `Program.cs`
   → verificar: `dotnet build`.

## Contrato que entrega a los demás

Al mergear este plan, los otros roles pueden asumir que existe:

| Elemento | Ruta | Lo usa |
|---|---|---|
| `Lugar`, `TipoBarrera`, `ReporteAccesibilidad` | `Api/Models/` | PLAN-01, PLAN-02, PLAN-03 |
| `AppDbContext` inyectable por constructor | `Api/Data/AppDbContext.cs` | PLAN-01, PLAN-02, PLAN-03 |
| Migración inicial aplicada | `Api/Migrations/` | PLAN-01 (seed) |
| Swagger accesible en `/swagger` | `Api/Program.cs` | PLAN-04 |
| Convención de registro de servicios | `Api/Configuracion/` | PLAN-01, PLAN-02, PLAN-03 |
| `LugaresController` con el GET base | `Api/Controllers/` | PLAN-01 lo extiende |

## Buenas prácticas obligatorias

- Migraciones con `dotnet ef`, nunca `EnsureCreated()`: el checklist de entrega
  (`clase-20.md:122-159`) exige migraciones generadas y aplicadas.
- `Program.cs` es el punto de conflicto de merge más probable del proyecto. Queda bajo un
  solo dueño y los demás registran servicios vía extensiones.
- Mantener este plan pequeño. Es preferible mergear una fundación flaca el primer día que
  una perfecta el cuarto: todo el equipo está bloqueado hasta que esto entre a `main`.
- Español en clases, propiedades y commits. Sin claves en el repositorio.

## Definición de terminado

- [ ] `dotnet build` termina con 0 errores.
- [ ] `dotnet ef database update` aplica la migración sobre una base de datos vacía.
- [ ] `dotnet run` levanta la API y `/swagger` carga.
- [ ] `GET /api/lugares` responde 200.
- [ ] El `.gitignore` impide subir `bin/`, `obj/` y `*.db`.
- [ ] PR mergeado y estado de PLAN-01, PLAN-02 y PLAN-03 actualizado a "Listo para empezar"
      en `PLANES/README.md`.
