# PLAN-04 — Documentación y QA

| | |
|---|---|
| Rol | Docs / QA |
| Responsable | Edwin — @Edwin252002 |
| Rama | `plan-04-docs` |
| Depende de | PLAN-01, PLAN-02 y PLAN-03 mergeados |
| Desbloquea | PLAN-05 |
| Fecha límite | Clase 19 |

## Precondiciones

- [ ] La API corre con `dotnet run` sobre `main` recién clonado.
- [ ] Todos los endpoints previstos existen y responden.
- [ ] El seed puebla la base de datos automáticamente.

## Archivos que este plan posee

    README.md
    screenshots/*
    docs/pruebas.md

Sobre el código, este plan solo agrega anotaciones de Swagger (`[ProducesResponseType]`,
comentarios XML) en los controladores, coordinando con su dueño antes de tocarlos.

## Tareas

1. Ejecutar la batería de pruebas manuales desde Swagger y registrar el resultado en
   `docs/pruebas.md`: por cada endpoint, un caso válido y uno inválido
   → verificar: la tabla cubre los códigos 200, 201, 204, 400 y 404.
2. Probar el endpoint de IA con una clave inválida para confirmar la degradación elegante
   → verificar: responde el mensaje de servicio no disponible, sin excepción sin controlar.
3. Escribir el `README.md` profesional: nombre, descripción, ODS con su meta (11.7 y 10.2),
   tecnologías, integrantes, requisitos previos, cómo ejecutar, cómo configurar la clave de
   Gemini. Incluir la cifra del DANE (`../docs/referencia-ntc6047.md`, sección 1) en el
   planteamiento del problema, la advertencia de que la salida del LLM es una sugerencia y no
   un dictamen normativo, y **la justificación de usar Gemini en vez de Groq** (el curso
   sugiere Groq; cambiar de proveedor exige documentar el porqué)
   → verificar: alguien que no tocó el proyecto lo clona, sigue el README y levanta la API.
4. Documentar la tabla de endpoints completa (método, ruta, descripción) con ejemplos JSON
   de solicitud y respuesta para los principales
   → verificar: los ejemplos son copias reales de Swagger, no inventados.
5. Tomar las capturas y guardarlas en `screenshots/`: Swagger completo, un GET con datos,
   un POST exitoso, un error 400 de validación, un 404 y la respuesta del análisis con IA
   → verificar: las imágenes se ven legibles y están enlazadas desde el README.
6. Agregar `[ProducesResponseType]` y comentarios XML en los controladores para que Swagger
   muestre los códigos y descripciones
   → verificar: Swagger documenta cada endpoint con sus respuestas posibles.

## Contrato que entrega a los demás

| Elemento | Ruta | Lo usa |
|---|---|---|
| README completo con tabla de endpoints | `README.md` | PLAN-05 (guion), evaluación (10% de la nota) |
| Capturas de casos válidos e inválidos | `screenshots/` | PLAN-05 (slides) |
| Registro de pruebas | `docs/pruebas.md` | PLAN-05 (respuestas al jurado) |

## Buenas prácticas obligatorias

- Los ejemplos JSON se copian de una ejecución real. Un ejemplo que no corresponde con la
  respuesta actual es peor que no ponerlo.
- Las capturas muestran también los errores: la rúbrica valora el manejo de 400 y 404.
- El README explica cómo configurar la clave del LLM, sin contenerla.
- Antes de escribir, clonar el repositorio en una carpeta limpia y seguir los propios pasos:
  es la única forma de detectar lo que falta.

## Definición de terminado

- [ ] Un clon limpio levanta la API siguiendo solo el README.
- [ ] La tabla de endpoints cubre todos los endpoints existentes.
- [ ] Hay ejemplos JSON de solicitud y respuesta reales.
- [ ] `screenshots/` contiene las seis capturas mínimas y están enlazadas.
- [ ] `docs/pruebas.md` documenta caso válido e inválido por endpoint.
- [ ] Swagger muestra los códigos de respuesta documentados.
- [ ] PR mergeado y estado actualizado en `PLANES/README.md`.
