# Planes de trabajo — Ruta Accesible

> **Tema confirmado por votación unánime del equipo: 5 de 5.** Idea presentada y taller de
> definición entregado el 30 de julio de 2026.
> **Falta asignar los cinco planes a las cinco personas** y recoger sus usuarios de GitHub.

**Diseño acordado (problema, modelo de datos, los 18 endpoints, integración con IA):**
`../docs/especificacion.md`. Es la fuente de verdad del contrato de cada controlador.
Taxonomía de la NTC 6047, cifras y riesgos del modelo: `../docs/referencia-ntc6047.md`.

## Repositorio

| | |
|---|---|
| Nombre | `ruta-accesible-api` |
| URL | `github.com/dony-aep/ruta-accesible-api` |
| Visibilidad | Público |
| Descripción | API REST en ASP.NET Core que clasifica reportes ciudadanos de barreras de accesibilidad urbana contra la NTC 6047 con un modelo de lenguaje. ODS 11.7 y 10.2. |
| Solución | `RutaAccesible.sln`, proyecto `Api/` |
| Rama por defecto | `main`, protegida: sin push directo, PR con una aprobación |
| Lo crea | @dony-aep (TL), en PLAN-00 |

Nombre en minúsculas con guiones y sufijo `-api`, siguiendo los ejemplos del profesor
(`eco-puntos-api`, `sistema-biblioteca`).

## Idea

| | |
|---|---|
| Proyecto | Ruta Accesible — accesibilidad urbana: reporte y clasificación de barreras |
| ODS | 11, meta 11.7 (espacios públicos accesibles para personas con discapacidad) y 10, meta 10.2 (inclusión social) |
| Ciudad de referencia | Barranquilla |
| Recurso principal | `ReporteAccesibilidad` |
| Entidades | `Lugar`, `TipoBarrera`, `ReporteAccesibilidad` |
| Relaciones | `Lugar` 1:N `ReporteAccesibilidad` · `TipoBarrera` 1:N `ReporteAccesibilidad` |
| Endpoint de filtro | `GET /api/lugares/buscar` |
| Endpoint de IA | `POST /api/reportes/{id}/analizar` |

## Decisiones técnicas cerradas

| Decisión | Valor | Por qué |
|---|---|---|
| Target framework | `net8.0` | Es la versión que indica el curso |
| Base de datos | SQLite con EF Core y migraciones | Sugerencia del curso, suficiente para el dominio |
| Origen de los datos | **Seed propio escrito a mano** | Sin dependencias externas en ejecución: la demo no puede fallar por red |
| Proveedor del LLM | **Gemini**, modelo `gemini-3.6-flash` | Free tier sin tarjeta, 1.000.000 TPM frente a los 6.000 de Groq, y salida JSON con esquema |
| Forma de la llamada | Endpoint compatible con OpenAI de Gemini | Mantiene el código del profesor (`clase-18.md:463-540`) cambiando solo URL base, modelo y clave |

La API **no consume servicios externos de datos**. El único servicio externo es el LLM, y solo
en el endpoint de análisis. Coordenadas, direcciones y catálogo viven en la base de datos.

## Equipo

Cinco integrantes. Los roles del curso son cuatro, así que **BD/DTOs se dobla**: dos personas
con el mismo rol pero **controladores distintos**, que es lo que evita que se pisen el mismo
archivo.

| Integrante | Usuario de GitHub | Plan asignado |
|---|---|---|
| Doney | @dony-aep | PLAN-00 — Fundación (TL) |
| Edwin | por recoger | por asignar |
| Zamith | por recoger | por asignar |
| Gabriela | por recoger | por asignar |
| Dilan | por recoger | por asignar |

**Se asigna en la reunión.** Cada quien toma un plan de la tabla de abajo; el TL confirma que
no queden dos personas en el mismo plan ni planes sin dueño, y actualiza esta tabla,
`.github/CODEOWNERS` y la cabecera de cada `PLAN-*.md`.

## Planes

| Plan | Rol | Qué construye | Rama | Responsable | Estado |
|---|---|---|---|---|---|
| [PLAN-00](PLAN-00-fundacion.md) — Fundación | Backend / TL | Repositorio, solución, modelos, `AppDbContext`, migración inicial, Swagger | `plan-00-fundacion` | @dony-aep | No iniciado |
| [PLAN-01](PLAN-01-datos-contratos.md) — Datos y contratos | BD / DTOs (1) | DTOs, validaciones, seed de Barranquilla, CRUD de `Lugar`, endpoint de búsqueda | `plan-01-datos` | por asignar | Bloqueado por PLAN-00 |
| [PLAN-02](PLAN-02-analisis-ia.md) — Análisis con IA | API / IA | Servicio de Gemini, CRUD de `ReporteAccesibilidad`, endpoint `analizar`, manejo del fallo | `plan-02-ia` | por asignar | Bloqueado por PLAN-00 |
| [PLAN-03](PLAN-03-catalogo-consultas.md) — Catálogo y consultas | BD / DTOs (2) | CRUD de `TipoBarrera`, borrado protegido, estadísticas por zona | `plan-03-catalogo` | por asignar | Bloqueado por PLAN-00 |
| [PLAN-04](PLAN-04-documentacion-qa.md) — Documentación y QA | Docs / QA | README, tabla de endpoints, capturas, batería de pruebas, anotaciones de Swagger | `plan-04-docs` | por asignar | Bloqueado por 01, 02 y 03 |
| [PLAN-05](PLAN-05-cierre-presentacion.md) — Cierre | Todos | Slides, guion, video demo, ensayos, checklist final | `plan-05-cierre` | Los 5 | Bloqueado por PLAN-04 |

Reparto de controladores, uno por persona. **Es la regla que sostiene todo el reparto:**

| Controlador | Plan |
|---|---|
| `LugaresController` | PLAN-01 |
| `ReportesController` | PLAN-02 |
| `TiposBarreraController` y `EstadisticasController` | PLAN-03 |

## Orden de desbloqueo

    PLAN-00 (fundacion)
        |
        +--> PLAN-01 (DTOs, validaciones, seed, busqueda) --+
        |                                                   |
        +--> PLAN-02 (servicio IA, endpoint analizar) ------+--> PLAN-04 (docs y QA) --> PLAN-05
        |                                                   |
        +--> PLAN-03 (catalogo, estadisticas) --------------+

PLAN-00 debe estar mergeado en `main` antes de que alguien abra rama de PLAN-01, PLAN-02 o
PLAN-03: los tres necesitan los modelos y el DbContext. Esos tres corren en paralelo y no
comparten archivos. PLAN-04 necesita la API corriendo con todos los endpoints reales.

## Alineación con los check-ins

| Momento | Debe estar mergeado |
|---|---|
| Check-in 1 (tras clase 17) | PLAN-00 completo |
| Check-in 2 (tras clase 18) | PLAN-01, PLAN-02 y PLAN-03 completos |
| Sábado 8 de agosto | PLAN-04 y PLAN-05 completos |

Cada check-in sin avance real (sin commits nuevos o el código no compila) cuesta -0.2 sobre
la nota final, hasta -0.4.

## Reglas comunes a todos los planes

- Rama por plan, PR contra `main`, revisión de al menos un compañero antes de mergear.
- Nadie edita archivos que otro plan declara como propios. Si hace falta, se pide al dueño.
- Un controlador por persona. Es la regla que sostiene todo el reparto.
- Cuando dos personas comparten rol, se separan por archivo, nunca por "hacemos lo mismo".
- `Program.cs` lo toca solo el TL. Cada rol registra sus servicios en su propio archivo de
  extensión bajo `Api/Configuracion/` y el TL agrega la línea de llamada.
- Español en clases, propiedades, comentarios y mensajes de commit. Sin emojis.
- Nada de claves ni secretos en el repositorio: la clave del LLM va por `dotnet user-secrets`.
- Un PR no se mergea sin la salida real de `dotnet build` pegada en la descripción.
