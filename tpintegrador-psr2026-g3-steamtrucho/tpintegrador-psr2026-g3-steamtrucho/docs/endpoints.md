# Endpoints — API REST

Formato: JSON. Fechas en ISO 8601 (`2026-09-03T18:00:00Z`).

Arquitectura: `Cliente → Controller → Service → DAO → Base de datos`. Los
errores de negocio no se manejan con `try/catch` en los Controllers: un
middleware centralizado traduce las excepciones de los Services en
respuestas HTTP con este formato:

```json
{ "error": "descripción legible del problema" }
```

| Código | Cuándo se devuelve |
|---|---|
| `404 Not Found` | La entidad solicitada (por Id) no existe. |
| `400 Bad Request` | Se violó una regla de negocio o los datos son inválidos. |
| `500 Internal Server Error` | Error inesperado no controlado. |

## Resumen

| Recurso | Método | Ruta | Descripción |
|---|---|---|---|
| Health | GET | `/api/health` | Chequeo de que la API está viva. |
| Videojuegos | GET | `/api/videojuegos` | Catálogo completo (admite `?nombre=`). |
| Videojuegos | GET | `/api/videojuegos/{id}` | Detalle de un videojuego. |
| Videojuegos | GET | `/api/videojuegos/categoria/{categoriaId}` | Busca por categoría. |
| Videojuegos | GET | `/api/videojuegos/desarrolladora/{desarrolladoraId}` | Busca por desarrolladora. |
| Videojuegos | GET | `/api/videojuegos/{id}/precio-actual` | Precio con la mejor promoción activa. |
| Videojuegos | POST | `/api/videojuegos` | Registra un videojuego. |
| Videojuegos | PUT | `/api/videojuegos/{id}` | Modifica un videojuego. |
| Usuarios | POST | `/api/usuarios` | Registra un usuario. |
| Usuarios | GET | `/api/usuarios/{id}/biblioteca` | Biblioteca personal del usuario. |
| Usuarios | GET | `/api/usuarios/{id}/compras` | Historial de compras del usuario. |
| Compras | POST | `/api/compras` | Inicia una compra (queda Pendiente). |
| Compras | GET | `/api/compras/{id}` | Detalle de una compra. |
| Compras | PATCH | `/api/compras/{id}/confirmar` | Confirma una compra pendiente. |
| Compras | PATCH | `/api/compras/{id}/cancelar` | Cancela una compra pendiente. |
| Campañas | GET | `/api/campanas/activas` | Campañas vigentes ahora. |
| Campañas | POST | `/api/campanas` | Registra una campaña. |
| Categorías * | GET | `/api/categorias` | Lista las categorías. |
| Categorías * | POST | `/api/categorias` | Registra una categoría. |
| Desarrolladoras * | GET | `/api/desarrolladoras` | Lista las desarrolladoras. |
| Desarrolladoras * | POST | `/api/desarrolladoras` | Registra una desarrolladora. |

\* No están en la lista mínima del enunciado, pero son necesarias para dar de
alta esos datos desde afuera y poder probar Videojuegos/Campañas de punta a
punta.

---

## Videojuegos

### `GET /api/videojuegos`
Consulta el catálogo completo. Admite `?nombre=` (opcional) para filtrar por
coincidencia parcial de texto.

**200 OK**
```json
[
  {
    "id": 1,
    "nombre": "The Last Frontier",
    "descripcion": "...",
    "precio": 4999.00,
    "fechaLanzamiento": "2024-03-15",
    "desarrolladoraId": 2,
    "desarrolladoraNombre": "Nova Studios",
    "categorias": [{ "id": 1, "nombre": "Aventura" }],
    "estado": "Disponible"
  }
]
```

### `GET /api/videojuegos/{id}`
Detalle de un videojuego.
**200 OK** (mismo shape que arriba, un solo objeto) · **404** si no existe.

### `GET /api/videojuegos/categoria/{categoriaId}`
Videojuegos de una categoría. **200 OK** · **404** si la categoría no existe.

### `GET /api/videojuegos/desarrolladora/{desarrolladoraId}`
Videojuegos de una desarrolladora. **200 OK** · **404** si no existe.

### `GET /api/videojuegos/{id}/precio-actual`
Calcula el precio aplicando la mejor promoción activa.

**200 OK**
```json
{
  "juegoId": 1,
  "precioOriginal": 4999.00,
  "precioFinal": 3749.25,
  "porcentajeDescuento": 25.0,
  "campanaAplicada": "Festival de Verano"
}
```
**404** si el videojuego no existe.

### `POST /api/videojuegos`
Registra un videojuego.

**Body**
```json
{
  "nombre": "The Last Frontier",
  "descripcion": "Un shooter cooperativo...",
  "precio": 4999.00,
  "fechaLanzamiento": "2024-03-15",
  "desarrolladoraId": 2,
  "categoriaIds": [1, 4],
  "estado": "Disponible"
}
```
`estado` es opcional (default `"Proximamente"`).

**201 Created** (header `Location` → `GET /api/videojuegos/{id}`) · **400** datos
inválidos · **404** si la desarrolladora o alguna categoría no existen.

### `PUT /api/videojuegos/{id}`
Reemplaza todos los campos de un videojuego, incluyendo `estado` (obligatorio
acá). Mismo body que el `POST` más `estado`.

**200 OK** · **400** datos inválidos o estado inexistente · **404** si el
videojuego, la desarrolladora o alguna categoría no existen.

---

## Usuarios

### `POST /api/usuarios`
```json
{ "nombre": "Juan Pérez", "email": "juan.perez@mail.com" }
```
**201 Created** · **400** si faltan datos o el email ya está registrado.

### `GET /api/usuarios/{id}/biblioteca`
**200 OK**
```json
{
  "usuarioId": 1,
  "items": [
    {
      "juegoId": 3,
      "nombreJuego": "The Last Frontier",
      "fechaAdquisicion": "2026-08-20T14:30:00Z",
      "horasJugadas": 0,
      "ultimaVezUsado": null
    }
  ]
}
```
**404** si el usuario no existe.

### `GET /api/usuarios/{id}/compras`
Historial completo (todos los estados). **200 OK** · **404** si el usuario no existe.

---

## Compras

### `POST /api/compras`
Valida disponibilidad, que el usuario no posea ya el juego, y calcula el
precio final con descuentos activos. Queda en estado **Pendiente**.

**Body**
```json
{ "usuarioId": 1, "juegoIds": [3, 5] }
```

**201 Created**
```json
{
  "id": 10,
  "usuarioId": 1,
  "fecha": "2026-09-03T18:00:00Z",
  "importeFinal": 8748.25,
  "estado": "Pendiente",
  "detalles": [
    { "juegoId": 3, "nombreJuego": "The Last Frontier", "precioFinal": 3749.25 },
    { "juegoId": 5, "nombreJuego": "Puzzle Kingdom", "precioFinal": 4999.00 }
  ]
}
```
**400** lista vacía, juego no disponible, o el usuario ya lo posee · **404**
si el usuario o algún juego no existen.

### `GET /api/compras/{id}`
**200 OK** · **404** si no existe.

### `PATCH /api/compras/{id}/confirmar`
Revalida todo y agrega los juegos a la biblioteca del usuario.
**200 OK** · **400** si no está Pendiente o algo dejó de cumplirse · **404**
si no existe.

### `PATCH /api/compras/{id}/cancelar`
No modifica la biblioteca.
**200 OK** · **400** si no está Pendiente · **404** si no existe.

---

## Campañas

### `GET /api/campanas/activas`
**200 OK**: lista de campañas vigentes en este momento (puede ser vacía).

### `POST /api/campanas`
Debe indicar al menos un criterio de alcance: `categoriaId`,
`desarrolladoraId`, o `juegosAfectados`.

**Body**
```json
{
  "nombre": "Festival de Verano",
  "fechaInicio": "2026-09-01T00:00:00Z",
  "fechaFin": "2026-09-30T23:59:59Z",
  "porcentajeDescuento": 25.0,
  "categoriaId": 1,
  "desarrolladoraId": null,
  "juegosAfectados": null
}
```
**201 Created** · **400** fechas/porcentaje inválidos o sin criterio de
alcance · **404** si la categoría o desarrolladora no existen.

---

## Categorías y Desarrolladoras (soporte)

### `GET` / `POST /api/categorias`
```json
{ "nombre": "Estrategia" }
```
**200 OK** (GET) · **201 Created** (POST) · **400** si falta el nombre.

### `GET` / `POST /api/desarrolladoras`
```json
{ "nombre": "Nova Studios" }
```
**200 OK** (GET) · **201 Created** (POST) · **400** si falta el nombre.

---

## Cómo probarlo

Con `npm run dev` corriendo, Swagger UI queda en
`http://localhost:5080/swagger`. Recorrido sugerido: crear una
Desarrolladora → crear una Categoría → crear un Videojuego (`Disponible`) →
crear un Usuario → crear una Campaña que lo alcance → ver el precio actual
(debe reflejar el descuento) → crear una Compra → confirmarla → verificar
que el juego aparece en la biblioteca del usuario.
