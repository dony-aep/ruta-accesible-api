# PLAN-05 — Cierre y presentación

| | |
|---|---|
| Rol | Compartido por los 5 integrantes |
| Coordina | Responsable de PLAN-04 (Docs/QA), con @dony-aep (TL) |
| Rama | `plan-05-cierre` |
| Depende de | PLAN-04 mergeado |
| Desbloquea | Entrega final |
| Fecha límite | Sábado 8 de agosto |

## Precondiciones

- [ ] El repositorio en `main` compila y corre desde un clon limpio.
- [ ] README y capturas completos.
- [ ] Todos los endpoints funcionan, incluido el de IA.

## Archivos que este plan posee

    slides/
    docs/guion-presentacion.md

## Tareas

1. Preparar entre 10 y 15 diapositivas siguiendo la estructura del profesor: problema,
   solución y tecnologías, modelo de datos, demo, código destacado, aprendizajes
   → verificar: el conteo está dentro del rango exigido.
2. Escribir el guion con tiempos por sección, ajustado a 12 minutos sobre un máximo de 15:
   intro 1:30 · solución 1:30 · modelo de datos 1:30 · demo CRUD e IA 2:30 ·
   código destacado 1:30 · aprendizajes 1:30 · preguntas 2:00
   → verificar: ensayo cronometrado dentro del tiempo.
3. Ensayar la demo en vivo tres veces, con la base de datos recreada desde cero cada vez
   → verificar: los tres ensayos terminan sin improvisar y en menos de 5 minutos.
4. Grabar el video demo de 3 a 5 minutos y enlazarlo desde el README
   → verificar: el enlace abre desde una ventana de incógnito (permisos públicos).
5. Preparar las respuestas a las preguntas previsibles: por qué este ODS, cómo funciona la
   integración con IA, qué harían distinto, qué fue lo más difícil. Material listo en
   `../docs/referencia-ntc6047.md`: abrir con la cifra del DANE (sección 1) y cerrar
   "qué mejoraríamos" con los riesgos de sesgo y alucinación normativa (sección 5)
   → verificar: cada integrante puede responder al menos una sin leer.
   Citar solo lo verificado: la sección 6 marca qué dato no tiene fuente y no debe usarse.
   El material sobre plataformas existentes (Wheelmap, AccessNow) lo tiene el TL y lo comparte
   por fuera del repositorio.
6. Repasar el checklist de entrega final de `clase-20.md:122-159` punto por punto
   → verificar: todas las casillas marcadas.

## Reparto en la presentación

Cada integrante habla de lo que construyó. Es lo más sólido frente al jurado y evita que
una sola persona responda por trabajo que no hizo.

| Plan | Integrante | Sección |
|---|---|---|
| PLAN-00 | @dony-aep | Arquitectura y modelo de datos |
| PLAN-01 | (por asignar) | DTOs, validaciones y búsqueda |
| PLAN-02 | (por asignar) | Integración con IA y manejo del fallo |
| PLAN-03 | (por asignar) | Catálogo, consultas y estadísticas |
| PLAN-04 | (por asignar) | Demo en vivo y aprendizajes |

## Buenas prácticas obligatorias

- Demo con datos del seed, nunca creando registros a mano en vivo.
- Tener un plan B para la demo: capturas o el video grabado, por si falla la red o el
  proveedor del LLM el día de la presentación.
- No mostrar código en pantalla más de 90 segundos seguidos.
- Ningún integrante presenta una parte que no tocó.

## Definición de terminado

- [ ] Slides entre 10 y 15, listas y ensayadas.
- [ ] Guion cronometrado bajo 15 minutos, con demo bajo 5.
- [ ] Video de 3 a 5 minutos grabado y enlazado en el README.
- [ ] Tres ensayos completos hechos.
- [ ] Checklist de `clase-20.md:122-159` completo.
- [ ] Repositorio final actualizado y público.
