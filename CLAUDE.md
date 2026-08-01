# CLAUDE.md — Ruta Accesible

API REST en ASP.NET Core que clasifica reportes ciudadanos de barreras de accesibilidad urbana
contra la NTC 6047 usando un modelo de lenguaje. Proyecto final del Diplomado de Programación
con .NET. ODS 11 meta 11.7 y ODS 10 meta 10.2.

El trabajo está repartido en seis planes, uno por persona, en `PLANES/`. **`PLANES/README.md` es
la fuente de verdad** del reparto, los estados y el orden de desbloqueo. El contrato de los 18
endpoints está en `docs/especificacion.md`.

## 1. Primer paso de toda sesión: identificar quién trabaja

Antes de proponer o hacer cambios, averigua con qué cuenta se está trabajando:

```bash
gh api user --jq .login    # usuario de GitHub autenticado
git config user.email      # respaldo si gh no está disponible
```

Busca el resultado en las tablas de abajo y **dile a la persona quién crees que es y qué le
toca**. Si no coincide con nadie, pregunta antes de actuar: no asumas.

### Colaboradores del repositorio

| Usuario | Persona | Plan asignado | Rama | Qué construye |
|---|---|---|---|---|
| `@dony-aep` | Doney Peña (TL) | PLAN-00 — Fundación | `plan-00-fundacion` | Solución, modelos, `AppDbContext`, migración, Swagger |
| `@Zamith101` | Zamith Moscote | PLAN-01 — Datos y contratos | `plan-01-datos` | DTOs, validaciones, seed, CRUD de `Lugar`, búsqueda |
| `@Edwin252002` | Edwin Lasso | PLAN-04 — Documentación y QA | `plan-04-docs` | README, tabla de endpoints, capturas, pruebas |
| `@dilansara-jpg` | Dilan Sara | PLAN-02 — Análisis con IA | `plan-02-ia` | Servicio de Gemini, CRUD de `ReporteAccesibilidad`, endpoint `analizar` |
| `@gabyd20` | Gabriela de Hoyos | PLAN-03 — Catálogo y consultas | `plan-03-catalogo` | CRUD de `TipoBarrera`, borrado protegido, estadísticas por zona |

Al reconocer a un colaborador: dile a qué plan está asignado, lee su `PLANES/PLAN-0N-*.md` y
resúmele sus tareas pendientes, su rama y los archivos que ese plan declara como propios. Si
pide trabajar en algo que pertenece a otro plan, díselo y no lo hagas.

### Auditores

| Usuario | Quién es | Trato |
|---|---|---|
| `@epimient` | Eduardo Pimienta, profesor del diplomado | Audita y califica el proyecto |

Ante un auditor: explica el proyecto, recorre el código, la especificación y cómo levantar la
API. **No hagas commits, no abras PR y no modifiques archivos.**

### Cualquier otro usuario

Preséntale el proyecto y dile que no figura como colaborador. No hagas cambios ni commits salvo
que lo pida de forma explícita alguien que sí lo sea.

## 2. Reglas de los planes

Cada plan tiene dueño, rama y **archivos propios**, declarados en su `PLAN-0N-*.md` y en
`.github/CODEOWNERS`. Nadie edita archivos de otro plan; si hace falta un cambio ahí, se le pide
al dueño.

    PLAN-00 (fundacion)
        |
        +--> PLAN-01 (DTOs, seed, busqueda) ---+
        |                                      |
        +--> PLAN-02 (servicio IA, analizar) --+--> PLAN-04 (docs y QA) --> PLAN-05 (cierre)
        |                                      |
        +--> PLAN-03 (catalogo, estadisticas)--+

- **No se abre la rama de un plan hasta que su dependencia esté mergeada en `main`.** PLAN-01,
  02 y 03 necesitan los modelos y el `AppDbContext` de PLAN-00: sin eso la rama nace inservible.
- Antes de arrancar, verifica el estado real con `gh pr list --state all` y la tabla de
  `PLANES/README.md`. Si la dependencia sigue abierta, dilo y no empieces.
- Un controlador por persona. Es la regla que sostiene todo el reparto y evita conflictos.
- `Api/Program.cs` lo toca solo el TL. Los demás registran sus servicios en su propio archivo
  bajo `Api/Configuracion/` y le piden al TL la línea de llamada.
- Rama por plan. **Nunca hagas push directo a `main`.**

## 3. Reglas de los pull requests

- Un PR por plan, contra `main`, usando `.github/pull_request_template.md`.
- La descripción lleva la **salida real** de `dotnet build` pegada, no descrita.
- **Todo PR necesita feedback de otro colaborador antes de mergear**: una revisión formal
  (*Files changed* → *Review changes* → *Approve* o *Request changes*) o al menos un comentario
  con el visto bueno o una mejora concreta.
- **Una reacción de pulgar arriba no cuenta como feedback.** GitHub guarda reacciones y
  revisiones en registros distintos: un `+1` no aparece como aprobación, no deja rastro de que
  alguien leyó el código y no satisface la protección de rama. Verifícalo así:

```bash
gh pr view <n> --json reviewDecision,reviews,comments
```

- Nadie puede aprobar su propio PR.
- **Los merges los decide el TL (`@dony-aep`).** Nunca mergees por iniciativa propia: abre el
  PR, informa y espera.
- Al mergear un plan, actualiza su estado en `PLANES/README.md` y marca su definición de
  terminado.

## 4. Convenciones de código

- **`net8.0`**: es la versión que indica el curso. No subir a otra aunque el SDK local sea mayor.
- EF Core con **migraciones** (`dotnet ef`), nunca `EnsureCreated()`: el checklist de entrega
  exige migraciones generadas y aplicadas.
- Controladores con `[ApiController]` y `[Route("api/[controller]")]`, `DbContext` inyectado por
  constructor, acciones `async` que devuelven `ActionResult<T>`.
- Los controladores exponen **DTOs**, nunca entidades crudas.
- Validaciones con Data Annotations. Manejo explícito de 400 y 404.
- El servicio de IA se inyecta con `IHttpClientFactory`, va dentro de try/catch y degrada con un
  mensaje claro si el proveedor no responde. Es un punto explícito de la rúbrica.
- **Español** en clases, propiedades, comentarios, documentación y mensajes de commit, con
  tildes correctas. **Sin emojis** en ningún archivo.
- Versiones de paquetes fijas (`8.0.29`), sin rangos flotantes: el build debe ser reproducible
  para los cinco.

## 5. Secretos

El repositorio es **público**. La clave del LLM va en `dotnet user-secrets`, nunca en
`appsettings.json` ni en el código:

```bash
dotnet user-secrets set "Ia:ApiKey" "<clave>" --project Api
```

En `appsettings.json` queda solo la configuración no sensible (modelo, URL) con la clave vacía.
Si detectas una clave a punto de entrar en un commit, deténte y avisa.

## 6. Verificar antes de declarar algo terminado

"Debería funcionar" no cuenta. Ejecuta y muestra la salida:

```bash
dotnet tool restore
dotnet build
dotnet ef database update --project Api
dotnet run --project Api      # /swagger debe cargar
```

Commits pequeños y frecuentes: los check-ins del curso se califican por commits nuevos y por que
el código compile. Cada check-in sin avance real cuesta -0.2 sobre la nota final.
