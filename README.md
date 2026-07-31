# Ruta Accesible

API REST que recibe reportes ciudadanos de barreras de accesibilidad en espacios públicos de
Barranquilla y los clasifica automáticamente contra los criterios de la norma colombiana
**NTC 6047:2013**, devolviendo el criterio incumplido, una severidad y un ajuste razonable.

**ODS 11**, meta 11.7 (espacios públicos seguros e inclusivos, accesibles para personas con
discapacidad) y **ODS 10**, meta 10.2 (inclusión social de las personas con discapacidad).

> Proyecto final del Diplomado en Programación con .NET.
> Este README es la versión inicial. PLAN-04 lo completa con la tabla de endpoints, ejemplos
> JSON reales y capturas.

## El problema

En Colombia **3.134.036 personas, el 7,1% de la población** (Censo DANE 2018), declaran alguna
dificultad para realizar actividades básicas diarias. Casi 1 de cada 14 colombianos.

Una persona con movilidad reducida no tiene forma de saber si un espacio público es transitable
antes de desplazarse. Y cuando reporta una barrera, lo hace en lenguaje natural: "la rampa está
partida y siempre hay motos encima". Nadie conecta ese relato con la norma técnica que se
incumple.

Esta API recibe esa descripción libre, la clasifica contra la taxonomía de la NTC 6047 y
devuelve algo accionable: qué criterio se incumple, con qué severidad y qué adecuación exige la
norma.

## Estado

En desarrollo. Ver `PLANES/README.md` para el avance de cada frente de trabajo.

## Stack

| | |
|---|---|
| Framework | ASP.NET Core Web API, .NET 8 LTS (`net8.0`) |
| Persistencia | Entity Framework Core con SQLite y migraciones |
| Documentación | Swagger / OpenAPI |
| Modelo de lenguaje | Gemini `gemini-3.6-flash`, vía su endpoint compatible con OpenAI |

## Estructura

```
RutaAccesible.sln
Api/
  Controllers/     un controlador por entidad
  Models/          entidades de EF Core
  Dtos/            Request/Response, separados de las entidades
  Data/            AppDbContext + SeedData
  Services/        servicio del modelo de lenguaje
  Configuracion/   extensiones de inyeccion de dependencias
  Migrations/      generadas por dotnet ef
docs/
  especificacion.md        diseno acordado: modelo de datos, endpoints, integracion con IA
  referencia-ntc6047.md    taxonomia de la norma, cifras y riesgos del modelo
PLANES/            reparto del trabajo por integrante
```

## Cómo ejecutarlo

Requisitos: SDK de .NET (el proyecto apunta a `net8.0`) y la herramienta `dotnet-ef`.

```bash
git clone https://github.com/dony-aep/ruta-accesible-api.git
cd ruta-accesible-api
dotnet restore
dotnet ef database update --project Api
dotnet run --project Api
```

Swagger queda en `/swagger`.

### Configurar la clave del modelo de lenguaje

La clave **no está en el repositorio** y no debe agregarse nunca. Se configura en local:

```bash
dotnet user-secrets init --project Api
dotnet user-secrets set "Ia:ApiKey" "<tu-clave-de-google-ai-studio>" --project Api
```

Se obtiene gratis en [Google AI Studio](https://aistudio.google.com/), sin tarjeta de crédito.
Sin clave, la API funciona igual: el endpoint de análisis responde que el servicio no está
disponible y el resto del CRUD no se ve afectado.

## Aviso sobre el uso del modelo de lenguaje

La clasificación que devuelve la API es una **sugerencia generada por un modelo de lenguaje, no
un dictamen normativo**. Requiere verificación técnica. El modelo justifica cada clasificación y
declara un nivel de certeza para que los casos dudosos puedan revisarse.

Las categorías de la NTC 6047 usadas como taxonomía provienen de fuentes secundarias. Las
medidas numéricas de la norma no están verificadas contra el texto original y por eso el
proyecto no las afirma. Detalle en `docs/referencia-ntc6047.md`.

## Equipo

| Integrante | Rol | Plan |
|---|---|---|
| Doney (@dony-aep) | Backend / TL | PLAN-00 |
| Edwin | por asignar | por asignar |
| Zamith | por asignar | por asignar |
| Gabriela | por asignar | por asignar |
| Dilan | por asignar | por asignar |

## Cómo contribuir

1. Toma tu plan en `PLANES/`. Dice qué archivos te pertenecen y qué tareas hacer.
2. Rama por plan: `git checkout -b plan-0X-nombre`.
3. **No edites archivos que otro plan declara como propios.** Si necesitas un cambio ahí,
   pídeselo a su dueño.
4. PR contra `main` con la salida real de `dotnet build` pegada en la descripción.
5. Revisión de al menos un compañero antes de mergear.

Español en clases, propiedades, comentarios y mensajes de commit. Sin emojis. Ninguna clave en
el repositorio.
