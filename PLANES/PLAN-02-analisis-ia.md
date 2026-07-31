# PLAN-02 — Análisis con IA

| | |
|---|---|
| Rol | API / IA |
| Responsable | (por asignar) |
| Rama | `plan-02-ia` |
| Depende de | PLAN-00 mergeado en `main` |
| Desbloquea | PLAN-04 |
| Fecha límite | Antes del check-in 2 (tras clase 18) |

## Proveedor y modelo

| | |
|---|---|
| Proveedor | Gemini (Google AI Studio) |
| Modelo | `gemini-3.6-flash` (disponibilidad general desde el 21 de julio de 2026) |
| URL base | `https://generativelanguage.googleapis.com/v1beta/openai/` |
| Autenticación | `Authorization: Bearer <clave>` |
| Free tier | Sin tarjeta de crédito. 15 RPM, 1.500 peticiones diarias, 1.000.000 TPM |
| Alternativa si falta cuota | `gemini-3.5-flash-lite`, cambiando solo `appsettings.json` |

Se eligió Gemini sobre Groq porque su límite de tokens por minuto es dos órdenes de magnitud
mayor (1.000.000 frente a 6.000), lo que importa al iterar el prompt, y porque soporta salida
JSON con esquema. **No usar modelos Pro:** pasaron a pago en abril de 2026 y fallarían por
cuota en plena presentación.

El endpoint compatible con OpenAI permite conservar la forma del código del profesor
(`clase-18.md:463-540`) cambiando solo URL base, modelo y clave.

## Precondiciones

- [ ] PLAN-00 está en `main`: modelos, `AppDbContext` y migración inicial disponibles.
- [ ] Clave de Gemini generada en Google AI Studio (gratis, sin tarjeta).
- [ ] Confirmado en AI Studio que `gemini-3.6-flash` tiene cuota en el free tier de la cuenta.
- [ ] Clave configurada en local con `dotnet user-secrets`, nunca en `appsettings.json`.

## Archivos que este plan posee

    Api/Services/*.cs
    Api/Configuracion/ServiciosIa.cs
    Api/Controllers/ReportesController.cs
    Api/Dtos/AnalisisDto.cs

No tocar `Api/Models/`, `AppDbContext.cs` ni `Program.cs`.

## Tareas

1. Habilitar user-secrets y guardar la clave:
   `dotnet user-secrets init --project Api` y
   `dotnet user-secrets set "Ia:ApiKey" "<clave>" --project Api`
   → verificar: `dotnet user-secrets list --project Api` la muestra y `git status` no
     reporta cambios en archivos de configuración.
2. Dejar en `appsettings.json` solo lo no sensible (`Ia:Modelo` y `Ia:UrlBase`), con la clave
   vacía. El modelo se lee de configuración, nunca hardcodeado: cambiarlo debe costar una línea
   → verificar: el repositorio no contiene la clave en ningún commit.
3. Crear `Api/Services/ServicioIa.cs` con `IHttpClientFactory`, cliente nombrado, y un
   método `AnalizarAsync(string prompt)` que devuelva la respuesta del modelo. Pedir salida
   JSON con esquema para que se deserialice directo al DTO en vez de parsear prosa
   → verificar: `dotnet build`.
4. Registrar el `HttpClient` nombrado y el servicio en `Api/Configuracion/ServiciosIa.cs`
   mediante un método de extensión, y pedir al TL la línea de llamada en `Program.cs`
   → verificar: la API arranca y el servicio se resuelve por inyección.
5. Implementar el **CRUD completo** de `ReportesController` (GET lista, GET por id, POST,
   PUT, DELETE) operando con DTOs. El PUT actualiza el estado del reporte
   (Registrado → Analizado → Verificado → Atendido); no permite reescribir la clasificación
   que puso la IA
   → verificar: los cinco endpoints responden desde Swagger con los códigos correctos
     (200, 201, 204, 400, 404).

   `ReporteAccesibilidad` es el **recurso principal** de la API: la rúbrica exige CRUD
   completo sobre él, así que el PUT no es opcional.
6. Implementar `POST /api/reportes/{id}/analizar`: carga el reporte con `Include()` del
   lugar y del tipo de barrera, arma el prompt con esos datos más la descripción libre del
   ciudadano, y devuelve criterio de la NTC 6047 incumplido, severidad, ajuste razonable
   sugerido, justificación breve y nivel de certeza
   → verificar: con un reporte del seed, la respuesta clasifica correctamente.
   El prompt incluye la **taxonomía cerrada** de los diez criterios de la NTC 6047
   (`../docs/referencia-ntc6047.md`, sección 2) y prohíbe explícitamente inventar criterios fuera
   de esa lista: es la mitigación del riesgo de alucinación normativa documentado en la
   sección 5 de ese archivo.
7. Envolver la llamada al LLM en try/catch: si el servicio falla, responder 200 con
   `"Servicio de IA no disponible"` en vez de propagar la excepción
   → verificar: con una clave inválida a propósito, el endpoint sigue respondiendo y el
     resto de la API no se ve afectada.

## Contrato que entrega a los demás

| Elemento | Ruta | Lo usa |
|---|---|---|
| `POST /api/reportes/{id}/analizar` | `Api/Controllers/ReportesController.cs` | PLAN-04 (capturas), PLAN-05 (demo) |
| CRUD de `ReporteAccesibilidad` | mismo controlador | PLAN-04 (tabla de endpoints) |
| Ejemplo de respuesta del análisis | `Api/Dtos/AnalisisDto.cs` | PLAN-04 (ejemplos JSON del README) |
| Instrucciones para configurar la clave | descripción del PR | PLAN-04 (sección del README) |

En el PR incluir el prompt final usado y un ejemplo real de respuesta: PLAN-04 los necesita
para documentar y PLAN-05 para explicarlos en la presentación.

## Buenas prácticas obligatorias

- El prompt se diseña para el dominio propio. Copiar el del profesor está explícitamente
  desaconsejado en `clase-18.md:583`.
- El LLM se usa solo en el endpoint de análisis, nunca en la ruta crítica del CRUD: así el
  fallo del proveedor degrada un endpoint y no la aplicación.
- `IHttpClientFactory` con cliente nombrado, no `new HttpClient()`.
- La clave jamás en el repositorio. El README explica cómo configurarla, no la contiene.
- Prompt corto y con formato de salida pedido explícitamente ("responde en máximo 3
  oraciones"), o la respuesta será inconsistente en la demo.
- La taxonomía va cerrada en el prompt y el modelo debe elegir de esa lista, nunca proponer
  un criterio nuevo. Sin esa restricción, inventa criterios de la norma que no existen.
- El modelo justifica su clasificación y declara certeza. La respuesta se presenta como
  **sugerencia, no como dictamen normativo**: la salida de un LLM mal clasificada podría
  orientar una decisión pública equivocada.
- No afirmar medidas numéricas de la norma (anchos, pendientes): no están verificadas contra
  el texto original de la NTC 6047.
- **Nada de datos personales reales en los reportes**, ni en el seed ni en la demo. El free
  tier de Gemini puede usar los prompts para mejorar los productos de Google. Con datos
  sembrados y ficticios es irrelevante, pero la regla debe quedar escrita y respetarse.
- El modelo se lee de configuración. Si Google cambia cuotas o retira el modelo, el arreglo es
  una línea de `appsettings.json`, no una recompilación de la lógica.

## Definición de terminado

- [ ] `dotnet build` con 0 errores.
- [ ] El CRUD completo de reportes (GET, GET/{id}, POST, PUT, DELETE) responde con los
      códigos correctos.
- [ ] `POST /api/reportes/{id}/analizar` devuelve un análisis coherente con un reporte del seed.
- [ ] Con clave inválida, el endpoint responde el mensaje de servicio no disponible y la API
      sigue funcionando.
- [ ] `git log -p` no contiene la clave en ningún commit.
- [ ] PR mergeado con el prompt y un ejemplo de respuesta documentados.
