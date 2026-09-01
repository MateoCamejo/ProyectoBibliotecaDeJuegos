# GameLibrary – Entregas 1, 2 y 3

Proyecto ASP.NET Core Web API (.NET 8) completo y ejecutable.

## Estructura

```
GameLibrary/
├── Domain/                 (Entrega 1) Clases del modelo + enums
├── Repositories/           (Entrega 1) Interfaces + implementación en memoria
├── DTOs/                   (Entrega 2) Contratos de entrada/salida de la API
├── Services/                (Entrega 3) Lógica de negocio
│   ├── Interfaces/
│   ├── Implementations/
│   ├── Exceptions/          NotFoundException, BusinessRuleException
│   └── Extensions/          Registro en el contenedor de DI
├── Middleware/              Traduce excepciones de Services en códigos HTTP
├── Controllers/             (Entrega 2) Puntos de entrada HTTP
├── Program.cs
└── appsettings.json
```

## Cómo correrlo

```bash
cd GameLibrary
dotnet restore
dotnet run
```

Swagger UI queda disponible en la raíz: `http://localhost:5080/`

## Flujo de negocio: Cliente → Controller → Service → Dominio

Los Controllers **no** contienen lógica: reciben el request, llaman al Service
correspondiente y devuelven el resultado. Toda la lógica de negocio (validar
disponibilidad, calcular el mejor descuento, agregar a la biblioteca, etc.)
vive en `Services/Implementations/`.

Errores esperables (juego no encontrado, usuario que ya posee el juego, etc.)
se resuelven lanzando `NotFoundException` o `BusinessRuleException` desde el
Service. El `ExceptionHandlingMiddleware` las intercepta y las convierte en
`404` o `400` respectivamente, así los Controllers quedan sin try/catch.

## Recorrido sugerido para probar en Swagger

1. `POST /api/desarrolladoras` → crear una desarrolladora.
2. `POST /api/categorias` → crear una o más categorías.
3. `POST /api/videojuegos` → crear un juego (`Estado: "Disponible"`).
4. `POST /api/usuarios` → crear un usuario.
5. `GET /api/videojuegos/{id}/precio-actual` → ver el precio sin descuentos.
6. `POST /api/campanas` → crear una campaña que alcance a ese juego (por
   categoría, desarrolladora o `JuegosAfectados`), con `FechaInicio`/`FechaFin`
   que incluyan la fecha/hora actual (UTC).
7. `GET /api/videojuegos/{id}/precio-actual` de nuevo → ahora debería reflejar
   el descuento.
8. `POST /api/compras` con el `UsuarioId` y `JuegoIds` → queda en `Pendiente`.
9. `PATCH /api/compras/{id}/confirmar` → pasa a `Confirmada` y el juego
   aparece en `GET /api/usuarios/{id}/biblioteca`.

## Decisiones de diseño a tener en cuenta

- **`CategoriasController` y `DesarrolladorasController`** no están en la
  lista mínima del enunciado, pero se agregaron porque sin ellos no hay forma
  de dar de alta esos datos desde la API para poder probar Videojuegos y
  Campañas de punta a punta.
- **Compras en dos pasos** (`POST` crea en `Pendiente`, `PATCH .../confirmar`
  completa la operación): el enunciado pide explícitamente "confirmar o
  cancelar una compra" como una operación de Service, y esto permite
  demostrar los tres estados (`Pendiente`, `Confirmada`, `Cancelada`) con un
  flujo real. La disponibilidad y tenencia del juego se validan tanto al
  crear la compra como al confirmarla, por si algo cambió mientras estuvo
  pendiente.
- **Selección de la mejor promoción**: quien resuelve "cuál campaña aplica
  cuando hay varias" es `CampanaService.ObtenerMejorPromocion`, ordenando por
  `PorcentajeDescuento` descendente y tomando la primera (no acumulable, tal
  como pide el enunciado). Tanto `VideojuegoService` (precio-actual) como
  `CompraService` (al calcular el precio final de una compra) reutilizan este
  mismo método, para no duplicar la regla en dos lugares.
- **Manejo de errores centralizado**: se optó por excepciones + middleware en
  lugar de try/catch repetido en cada Controller, para que la separación de
  responsabilidades pedida en la Entrega 3 (Controller liviano, Service con
  la lógica) se note más claramente.
