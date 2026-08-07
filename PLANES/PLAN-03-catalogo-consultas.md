# PLAN-03 — Catálogo y consultas

| | |
|---|---|
| Rol | BD / DTOs (segundo integrante del rol) |
| Responsable | Gabriela — @gabyd20 |
| Rama | `plan-03-catalogo` |
| Depende de | PLAN-00 mergeado en `main` |
| Desbloquea | PLAN-04 |
| Fecha límite | Antes del check-in 2 (tras clase 18) |

Este plan existe porque el equipo es de 5 personas y el rol BD/DTOs se dobla. La separación
es por archivo: PLAN-01 posee el controlador de `Lugar`, este posee el de `TipoBarrera`.
Nunca se edita el mismo archivo desde las dos ramas.

## Precondiciones

- [ ] PLAN-00 está en `main`: modelos, `AppDbContext` y migración inicial disponibles.
- [ ] Acordado con el responsable de PLAN-01 qué DTOs comparten y quién los crea primero, para no
      duplicar clases en `Api/Dtos/`.

## Archivos que este plan posee

    Api/Controllers/TiposBarreraController.cs
    Api/Controllers/EstadisticasController.cs
    Api/Dtos/TipoBarreraDto.cs
    Api/Dtos/TipoBarreraCrearDto.cs
    Api/Dtos/EstadisticasDto.cs

No tocar `Api/Models/`, `AppDbContext.cs`, `Program.cs` ni los archivos de PLAN-01 y PLAN-02.

## Tareas

1. Crear los DTOs de `TipoBarrera` (salida y entrada) con Data Annotations. El catálogo son
   los diez criterios de la NTC 6047 listados en `../docs/referencia-ntc6047.md`, sección 2:
   estacionamientos, circulación horizontal, rampas, escaleras, ascensores, pasillos y
   puertas, baños accesibles, señalización, mobiliario, y entradas y ventanillas.
   No incluir medidas numéricas: no están verificadas contra el texto de la norma
   → verificar: `dotnet build`.
2. Implementar el CRUD completo de `TiposBarreraController`: GET lista, GET por id, POST,
   PUT, DELETE, con DTOs y 404 cuando corresponda
   → verificar: los cinco endpoints responden desde Swagger con los códigos correctos.
3. Impedir borrar un tipo de barrera que tenga reportes asociados: devolver 400 con un
   mensaje claro en vez de dejar que falle la restricción de clave foránea
   → verificar: intentar borrar uno en uso devuelve 400 y el registro sigue existiendo.
4. Implementar un endpoint de agregación
   `GET /api/estadisticas/barreras-por-zona` que devuelva, por zona, el conteo de reportes
   agrupado por tipo de barrera, usando LINQ (`GroupBy`). Va en su **propio controlador**,
   `EstadisticasController.cs`, porque la ruta es `/api/estadisticas` y la convención del
   curso es `[Route("api/[controller]")]`: meterlo en `TiposBarreraController` obligaría a
   romper esa convención con una ruta absoluta
   → verificar: los números coinciden con los datos del seed contados a mano.
5. Documentar en el PR los dos endpoints nuevos con un ejemplo de solicitud y respuesta
   → verificar: PLAN-04 puede copiarlos al README sin volver a probarlos.

## Contrato que entrega a los demás

| Elemento | Ruta | Lo usa |
|---|---|---|
| CRUD de `TipoBarrera` | `Api/Controllers/TiposBarreraController.cs` | PLAN-04 (tabla de endpoints) |
| `GET /api/estadisticas/barreras-por-zona` | `Api/Controllers/EstadisticasController.cs` | PLAN-04, PLAN-05 (dato fuerte para la demo) |
| Regla de borrado protegido | mismo controlador | PLAN-04 (caso de prueba de error 400) |

## Buenas prácticas obligatorias

- Sin entidades crudas en las respuestas: DTOs siempre.
- La agregación se resuelve en la consulta LINQ, no trayendo todo a memoria y contando en C#.
- Coordinar con PLAN-01 antes de crear un DTO que pueda existir ya: dos clases con el mismo
  propósito y distinto nombre es el tipo de desorden que se nota en la revisión.
- Español en nombres y commits.

## Definición de terminado

- [x] `dotnet build` con 0 errores.
- [x] Los cinco endpoints de `TipoBarrera` responden correctamente.
- [x] Borrar un tipo en uso devuelve 400 y no rompe la base de datos.
- [x] El endpoint de estadísticas devuelve conteos que coinciden con el seed.
- [x] PR mergeado y estado actualizado en `PLANES/README.md`.

PR #4, mergeado el 6 de agosto de 2026. Las cinco casillas se verificaron con la API
corriendo, no por lectura del código.

Dos correcciones sobre la revisión, aplicadas por el TL sobre la rama por la cercanía de
la presentación:

- El endpoint de estadísticas devolvía un grupo con `tipoBarrera: null` por zona, porque
  `TipoBarreraId` es nulo hasta que la IA clasifica el reporte. Con el seed actual eran 12
  de 17 reportes. Ahora se agrupan bajo `"Sin clasificar"`.
- Un `codigo` repetido devolvía 500: hay índice único en `AppDbContext` y el duplicado
  reventaba en `SaveChangesAsync`. Ahora se comprueba antes de guardar y devuelve 400, como
  pide `../docs/especificacion.md`.
