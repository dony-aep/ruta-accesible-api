### Batería de Pruebas — Módulo de Reportes e IA 

| Endpoint | Método | Tipo de Caso | Parámetros / Body de Entrada | Código Esperado | Código Obtenido | Estado |
|---|---|---|---|---|---|---|
| `/api/Reportes` | `GET` | Válido | Ninguno | 200 OK | 200 OK | ✅ Pass |
| `/api/Reportes/{id}` | `GET` | Válido | `id: 12` | 200 OK | 200 OK | ✅ Pass |
| `/api/Reportes/{id}` | `GET` | Inválido | `id: 1000` (ID inexistente) | 404 Not Found | 404 Not Found | ✅ Pass |
| `/api/Reportes` | `POST` | Válido | `{ "usuario": "Admin", "descripcion": "Prueba de calidad", "lugarId": 1 }` | 201 Created | 201 Created | ✅ Pass |
| `/api/Reportes` | `POST` | Inválido | `{ "usuario": "Admin", "descripcion": "Prueba de calidad", "lugarId": }` (Sintaxis incompleta) | 400 Bad Request | 400 Bad Request | ✅ Pass |
| `/api/Reportes/{id}` | `PUT` | Válido | `id: 18`<br>`{ "estado": "Analizado" }` | 204 No Content | 204 No Content | ✅ Pass |
| `/api/Reportes/{id}` | `PUT` | Inválido | `id: 18`<br>`{ "estado": "" }` | 400 Bad Request | 400 Bad Request | ✅ Pass |
| `/api/Reportes/{id}` | `PUT` | Inválido | `id: 19`<br>`{ "estado": "Registrado" }` (ID inexistente) | 404 Not Found | 404 Not Found | ✅ Pass |
| `/api/Reportes/{id}` | `DELETE` | Válido | `id: 18` | 204 No Content | 204 No Content | ✅ Pass |
| `/api/Reportes/{id}` | `DELETE` | Inválido | `id: 19` (ID inexistente) | 404 Not Found | 404 Not Found | ✅ Pass |
| `/api/Reportes/{id}/analizar` | `POST` | Válido (Degradación) | `id: 17` (Servicio IA no disponible) | 200 OK | 200 OK (Manejo de fallo) | ✅ Pass |
| `/api/Reportes/{id}/analizar` | `POST` | Inválido | `id: 19` (ID inexistente) | 404 Not Found | 404 Not Found | ✅ Pass |

### Batería de Pruebas — Módulo de Lugares

| Endpoint | Método | Tipo de Caso | Parámetros / Body de Entrada | Código Esperado | Código Obtenido | Estado |
|---|---|---|---|---|---|---|
| `/api/Lugares` | `GET` | Válido | Ninguno | 200 OK | 200 OK | ✅ Pass |
| `/api/Lugares/{id}` | `GET` | Válido | `id: 1` | 200 OK | 200 OK | ✅ Pass |
| `/api/Lugares/{id}` | `GET` | Inválido | `id: 21` (ID inexistente) | 404 Not Found | 404 Not Found | ✅ Pass |
| `/api/Lugares` | `POST` | Válido | `{ "nombre": "Admin", "tipo": "Admin", "zona": "String", "direccion": "string", "latitud": 90, "longitud": 180, "esServicioAlCiudadano": true }` | 201 Created | 201 Created | ✅ Pass |
| `/api/Lugares` | `POST` | Inválido | `{ "nombre": "Admin", "tipo": "", "zona": "String", "direccion": "string", "latitud": 90, "longitud": 180, "esServicioAlCiudadano": true }` (Campo obligatorio `tipo` vacío) | 400 Bad Request | 400 Bad Request | ✅ Pass |
| `/api/Lugares/buscar` | `GET` | Válido | Query params por defecto / vacíos | 200 OK | 200 OK | ✅ Pass |
| `/api/Lugares/{id}` | `PUT` | Válido | `id: 12`<br>`{ "nombre": "Admin", "tipo": "Admin", "zona": "ded prueba", "direccion": "de prueba", "latitud": 90, "longitud": 180, "esServicioAlCiudadano": true }` | 204 No Content | 204 No Content | ✅ Pass |
| `/api/Lugares/{id}` | `PUT` | Inválido | `id: 12`<br>`{ "nombre": "string", "tipo": "string", "zona": "string", "direccion": "string", "latitud": , "longitud": 180, "esServicioAlCiudadano": }` (Sintaxis con valores incompletos) | 400 Bad Request | 400 Bad Request | ✅ Pass |
| `/api/Lugares/{id}` | `PUT` | Inválido | `id: 21`<br>`{ "nombre": "string", "tipo": "string", "zona": "string", "direccion": "string", "latitud": 90, "longitud": 180, "esServicioAlCiudadano": true }` (ID inexistente) | 404 Not Found | 404 Not Found | ✅ Pass |
| `/api/Lugares/{id}` | `DELETE` | Válido | `id: 12` | 204 No Content | 204 No Content | ✅ Pass |
| `/api/Lugares/{id}` | `DELETE` | Inválido | `id: 14` (ID inexistente) | 404 Not Found | 404 Not Found | ✅ Pass |

### Batería de Pruebas — Módulo de Tipos de Barrera y Esstadisticas

| Endpoint | Método | Tipo de Caso | Parámetros / Body de Entrada | Código Esperado | Código Obtenido | Estado |
|---|---|---|---|---|---|---|
| `/api/TiposBarrera` | `GET` | Válido | Ninguno | 200 OK | 200 OK | ✅ Pass |
| `/api/TiposBarrera/{id}` | `GET` | Válido | `id: 1` | 200 OK | 200 OK | ✅ Pass |
| `/api/TiposBarrera/{id}` | `GET` | Inválido | `id: 21` (ID inexistente) | 404 Not Found | 404 Not Found | ✅ Pass |
| `/api/TiposBarrera` | `POST` | Válido | `{ "codigo": "string", "nombre": "string", "criterioNorma": "string" }` | 201 Created | 201 Created | ✅ Pass |
| `/api/TiposBarrera` | `POST` | Inválido | `{ "codigo": "", "nombre": "string", "criterioNorma": "string" }` (Código obligatorio vacío) | 400 Bad Request | 400 Bad Request | ✅ Pass |
| `/api/TiposBarrera/{id}` | `PUT` | Válido | `id: 1`<br>`{ "codigo": "1001", "nombre": "string", "criterioNorma": "string" }` | 204 No Content | 204 No Content | ✅ Pass |
| `/api/TiposBarrera/{id}` | `PUT` | Inválido | `id: 1`<br>`{ "codigo": "string", "nombre": "string", "criterioNorma": "string" }` (Código duplicado) | 400 Bad Request | 400 Bad Request | ✅ Pass |
| `/api/TiposBarrera/{id}` | `PUT` | Inválido | `id: 21`<br>`{ "codigo": "string", "nombre": "string", "criterioNorma": "string" }` (ID inexistente) | 404 Not Found | 404 Not Found | ✅ Pass |
| `/api/TiposBarrera/{id}` | `DELETE` | Válido | `id: 1` | 204 No Content | 204 No Content | ✅ Pass |
| `/api/TiposBarrera/{id}` | `DELETE` | Inválido | `id: 3` (Tiene reportes asociados) | 400 Bad Request | 400 Bad Request | ✅ Pass |
| `/api/TiposBarrera/{id}` | `DELETE` | Inválido | `id: 21` (ID inexistente) | 404 Not Found | 404 Not Found | ✅ Pass |
| `/api/Estadisticas/barreras-por-zona` | `GET` | Válido | Ninguno | 200 OK | 200 OK | ✅ Pass |