# PLAN-01 — Datos y contratos

| | |
|---|---|
| Rol | BD / DTOs (primer integrante del rol) |
| Responsable | Zamith — @Zamith101 |
| Rama | `plan-01-datos` |
| Depende de | PLAN-00 mergeado en `main` |
| Desbloquea | PLAN-04 |
| Fecha límite | Antes del check-in 2 (tras clase 18) |

## Precondiciones

- [ ] PLAN-00 está en `main`: existen los modelos, el `AppDbContext` y la migración inicial.
- [ ] `dotnet build` pasa sobre `main` recién clonado.
- [ ] `dotnet ef database update` crea la base de datos local.
- [ ] Acordado con el responsable de PLAN-03 (mismo rol) quién crea qué DTOs, para no duplicar
      clases en `Api/Dtos/`.

## Archivos que este plan posee

    Api/Dtos/*.cs
    Api/Data/SeedData.cs
    Api/Controllers/LugaresController.cs
    Api/Configuracion/ServiciosDatos.cs

No tocar `Api/Models/`, `AppDbContext.cs` ni `Program.cs`. Si un modelo necesita un campo
nuevo, pedirlo al responsable de PLAN-00.

## Tareas

1. Crear los DTOs de salida en `Api/Dtos/`: `LugarDto`, `ReporteDto`, con solo los campos
   que la API debe exponer (nunca la entidad cruda)
   → verificar: `dotnet build`.
2. Crear los DTOs de entrada: `LugarCrearDto`, `LugarActualizarDto`, con Data Annotations
   (`[Required]`, `[MaxLength]`, `[Range]` para latitud y longitud)
   → verificar: enviar un POST inválido desde Swagger devuelve 400 con los mensajes.
3. Completar el CRUD de `LugaresController`: GET lista, GET por id, POST, PUT, DELETE,
   todos operando con DTOs y devolviendo 404 cuando no exista el recurso
   → verificar: los cinco endpoints responden desde Swagger con los códigos correctos.
4. Implementar el endpoint de filtro
   `GET /api/lugares/buscar?tipo=&zona=&sinBarrerasCriticas=` con query params opcionales y
   consulta LINQ construida con `AsQueryable()`
   → verificar: cada combinación de filtros devuelve el subconjunto esperado.
5. Crear `Api/Data/SeedData.cs` con datos creíbles: los diez tipos de barrera de la taxonomía
   de la NTC 6047 (`../docs/referencia-ntc6047.md`, sección 2), entre 8 y 12 lugares reales de
   **Barranquilla** y unos 15 reportes repartidos con descripciones realistas en texto libre
   → verificar: tras `dotnet ef database update`, `GET /api/lugares` devuelve los registros.

   **Lugares sugeridos.** Priorizar espacios donde la NTC 6047 sí aplica, es decir de servicio
   al ciudadano en la administración pública: eso hace que la clasificación normativa tenga
   sentido jurídico y no solo técnico.

   | Lugar | Tipo |
   |---|---|
   | Centro Administrativo Distrital (Alcaldía) | Sede administrativa |
   | Biblioteca Departamental Meira Delmar | Biblioteca pública |
   | Parque Cultural del Caribe (Museo del Caribe) | Museo |
   | Terminal Metropolitana de Transporte | Terminal |
   | Estación Central de Transmetro | Estación de transporte |
   | Portal de Transmetro Joe Arroyo | Estación de transporte |
   | Universidad del Atlántico, sede norte | Universidad pública |
   | Gran Malecón del Río | Espacio público |
   | Plaza de la Paz Juan Pablo II | Plaza pública |
   | Estadio Metropolitano Roberto Meléndez | Escenario deportivo |
   | Mercado de Barranquilita | Mercado público |

   Direcciones y coordenadas: **tomarlas de Google Maps, no inventarlas**. La latitud y la
   longitud son dos campos `decimal`; no se necesita ningún servicio de geocodificación.
   Variar el tipo y la zona entre los lugares para que el endpoint de filtro tenga algo real
   que filtrar.
6. Registrar el seed vía `Api/Configuracion/ServiciosDatos.cs` y pedir al TL que agregue la
   línea de llamada en `Program.cs`
   → verificar: base de datos borrada y recreada queda poblada automáticamente.

## Contrato que entrega a los demás

| Elemento | Ruta | Lo usa |
|---|---|---|
| DTOs de entrada y salida | `Api/Dtos/` | PLAN-02 y PLAN-03 (para no duplicarlos) |
| Seed data poblada | `Api/Data/SeedData.cs` | PLAN-02, PLAN-03 y PLAN-04 (demo sin cargar datos a mano) |
| CRUD de `Lugar` funcionando | `Api/Controllers/LugaresController.cs` | PLAN-04 (capturas y tabla de endpoints) |
| `GET /api/lugares/buscar` | mismo controlador | PLAN-04, PLAN-05 (demo) |

Avisar en el PR qué descripciones de reporte quedaron sembradas: PLAN-02 las usará para
probar el análisis con IA sin inventar datos.

## Buenas prácticas obligatorias

- Los controladores nunca exponen entidades de EF Core: siempre DTOs. Es un punto explícito
  de la rúbrica.
- Validar con Data Annotations en los DTOs de entrada, no con `if` dentro del controlador.
- Devolver 404 con un mensaje claro, no una excepción sin controlar.
- Consultas asíncronas (`ToListAsync`, `FirstOrDefaultAsync`) y `Include()` solo donde haga
  falta.
- Seed con datos verosímiles: en la presentación se ven, y unos datos de relleno restan.

## Definición de terminado

- [ ] `dotnet build` con 0 errores.
- [ ] Los cinco endpoints CRUD de `Lugar` responden con los códigos correctos (200, 201, 204, 400, 404).
- [ ] `GET /api/lugares/buscar` filtra por los tres parámetros, juntos y por separado.
- [ ] Base de datos borrada y recreada queda poblada por el seed.
- [ ] Ningún endpoint devuelve entidades crudas.
- [ ] PR mergeado y estado actualizado en `PLANES/README.md`.
