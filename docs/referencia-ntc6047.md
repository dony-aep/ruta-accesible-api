# Referencia — NTC 6047:2013

Extracto técnico de la investigación del equipo, con lo mínimo que hace falta para construir
la API. Lo usan PLAN-01 (seed del catálogo), PLAN-02 (prompt del modelo), PLAN-03 (DTOs del
catálogo) y PLAN-04 (planteamiento del problema en el README).

## 1. Magnitud del problema

**Censo DANE 2018: 3.134.036 personas, 7,1% de la población colombiana**, declararon alguna
dificultad para realizar actividades básicas diarias (era 6,3% en 2005). Casi 1 de cada 14
colombianos.

Fuente verificada: [nota estadística del DANE sobre medición de discapacidad](https://www.dane.gov.co/files/investigaciones/notas-estadisticas/abr_2022_nota_estadistica_Estado%20actual_de_la_medicion_de_discapacidad_en%20Colombia_presentacion.pdf).

No existen cifras oficiales desagregadas de "movilidad reducida" como categoría propia, solo
los totales de discapacidad. **No inventar una.**

## 2. Taxonomía — catálogo de `TipoBarrera`

La NTC 6047:2013 define criterios de accesibilidad y señalización para espacios de atención al
ciudadano, en obra nueva y en adecuaciones, e introduce el principio de **ajustes razonables**
(adaptaciones viables sin carga desproporcionada).

Estos diez criterios son el catálogo que se siembra en `TipoBarrera` y la **lista cerrada** que
recibe el modelo de lenguaje:

| Código sugerido | Criterio | Alcance |
|---|---|---|
| `NTC-ESTACIONAMIENTOS` | Estacionamientos accesibles | Plazas reservadas cerca de la entrada, con dimensiones mínimas |
| `NTC-CIRCULACION` | Circulación horizontal | Pasillos y senderos amplios, a nivel |
| `NTC-RAMPAS` | Rampas | Pendiente y ancho reglamentarios, superficie antideslizante |
| `NTC-ESCALERAS` | Escaleras | Huella y contrahuella limitadas, pasamanos en ambos lados |
| `NTC-ASCENSORES` | Ascensores y plataformas elevadoras | Espacio interior y controles alcanzables desde silla de ruedas |
| `NTC-PUERTAS` | Pasillos y puertas | Ancho libre mínimo, sin obstáculos |
| `NTC-BANOS` | Cuartos de baño accesibles | Área de maniobra y barras de apoyo; tipos A, B y C |
| `NTC-SENALIZACION` | Señalización | Visual y podotáctil, orientación y seguridad |
| `NTC-MOBILIARIO` | Mobiliario y áreas de atención | Mostradores bajos, asientos adaptados, espacios en salas de espera |
| `NTC-ENTRADAS` | Auditorios, entradas y ventanillas | Accesibles físicamente y con información adaptada |

## 3. Advertencia de uso — importante

El texto completo de la norma no está disponible libremente; estos criterios provienen de
resúmenes y de informes de auditoría.

- Las **categorías** son seguras para usar como taxonomía.
- Las **medidas exactas** (ancho de rampa, huella y contrahuella, ancho de puertas) **no están
  verificadas** contra el texto original. No afirmarlas como valores normativos: ni en el
  seed, ni en el prompt, ni en el README, ni en la demo.

## 4. Normativa que respalda el proyecto

- **Ley 1618 de 2013** (estatutaria de discapacidad): garantiza accesibilidad a todos los
  entornos, obliga a eliminar barreras y a adoptar ajustes razonables.
- **Ley 1712 de 2014** (transparencia) y **Decreto 103 de 2015, art. 14**: exigen que los
  puntos de servicio al ciudadano cumplan la NTC 6047, incluidos los ajustes razonables.

Por eso el seed prioriza espacios de servicio al ciudadano: es donde la norma sí aplica y la
clasificación tiene sentido jurídico, no solo técnico.

## 5. Riesgos del uso del modelo de lenguaje

Material obligatorio para PLAN-02 (diseño del prompt) y para la sección "qué mejoraríamos" de
la presentación:

- **Alucinaciones normativas:** sin la taxonomía anclada en el prompt, el modelo inventa un
  criterio de la NTC que no existe, o asigna el incorrecto a una descripción ambigua.
  **Mitigación: la lista de la sección 2 va cerrada en el prompt y se le prohíbe explícitamente
  proponer criterios fuera de ella.**
- **Sesgos del modelo:** puede infravalorar situaciones de barrios marginados o
  interpretaciones culturales distintas, con falsos negativos o falsos positivos.
- **Impacto en decisiones públicas:** una clasificación errónea desvía recursos o invisibiliza
  un problema grave.
- **Crítica desde la accesibilidad:** el reporte ciudadano simplifica una evaluación que
  requiere inspección técnica. Es la objeción más fuerte y hay que reconocerla, no esquivarla.

**Salvaguardas que el proyecto implementa:** el modelo justifica cada clasificación y declara
un nivel de certeza (transparencia), los casos de baja certeza quedan marcados para revisión
humana, y la salida se presenta siempre como **sugerencia, no como dictamen normativo**.

## 6. Qué no citar

| Dato | Estado |
|---|---|
| Cifras DANE 2018 | Verificado con fuente. Usar |
| Categorías de la NTC 6047 | Fiables como taxonomía, de fuentes secundarias. Usar |
| Medidas exactas de la NTC | **No verificadas.** No afirmar como normativas |
| "Rajan et al. 2026" | **No localizado.** No citar |
| "Más del 40% mal clasificadas" | **Sin fuente.** No citar |
