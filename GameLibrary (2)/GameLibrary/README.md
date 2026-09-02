# GameLibrary – Entregas 1, 2 y 3 (con DAO + SQL Server)

Proyecto ASP.NET Core Web API (.NET 8), con persistencia real en SQL Server
usando ADO.NET puro (SqlConnection/SqlCommand) a través de una capa DAO.

## Estructura

```
GameLibrary/
├── Domain/                  Clases del modelo + enums (sin cambios desde la Entrega 1)
├── DAO/                     Capa de acceso a datos (patrón DAO)
│   ├── I*DAO.cs              Interfaces, con nombres en español (ListarTodos,
│   │                         BuscarPorId, Guardar, Actualizar, etc.)
│   ├── SqlServer/            Implementación con ADO.NET puro (SqlConnection/SqlCommand)
│   ├── Infraestructura/      ConexionSql: único lugar que conoce la cadena de conexión
│   └── Extensions/           Registro en el contenedor de DI
├── DTOs/                    Contratos de entrada/salida de la API
├── Services/                 Lógica de negocio (sin cambios de lógica, solo pasaron
│   │                         de hablarle a un Repository en memoria a hablarle a un DAO)
│   ├── Interfaces/
│   ├── Implementations/
│   ├── Exceptions/            NotFoundException, BusinessRuleException
│   └── Extensions/
├── Middleware/                Traduce excepciones de Services en códigos HTTP
├── Controllers/                Puntos de entrada HTTP
├── sql/schema.sql              Script de creación de la base y las tablas
├── Program.cs
└── appsettings.json             Acá va la cadena de conexión
```

## Cómo correrlo

1. **Base de datos**: ejecutá `sql/schema.sql` contra tu instancia de SQL Server
   (crea la base `GameLibraryDb` y todas las tablas). Podés correrlo desde SSMS,
   Azure Data Studio, o `sqlcmd`.

2. **Cadena de conexión**: revisá `appsettings.json` → `ConnectionStrings:Default`.
   Por defecto asume autenticación de Windows contra una instancia local:

   ```json
   "Default": "Server=localhost;Database=GameLibraryDb;Trusted_Connection=True;TrustServerCertificate=True;"
   ```

   Si usás autenticación SQL (usuario/contraseña) o LocalDB, ajustala, por ejemplo:

   ```json
   "Default": "Server=localhost;Database=GameLibraryDb;User Id=sa;Password=TuPassword;TrustServerCertificate=True;"
   ```

3. **Correr la API**:

   ```bash
   cd GameLibrary
   dotnet restore
   dotnet run
   ```

   Swagger UI queda en la raíz: `http://localhost:5080/`

## Qué cambió respecto a la entrega anterior

- Se **eliminó** la carpeta `Repositories/` (repositorios en memoria con
  `List<T>`) y se reemplazó por `DAO/`, que habla con SQL Server real.
- Los **métodos de los DAO están en español**, tal como lo pidió el
  profesor: `ListarTodos`, `BuscarPorId`, `Guardar`, `Actualizar`,
  `ListarPorCategoria`, `ListarActivas`, etc. (no es un CRUD genérico con
  los 5 métodos de siempre: cada DAO solo expone lo que su Service
  realmente necesita).
- Cada método de un DAO abre su propia `SqlConnection` (patrón clásico de
  ADO.NET: abrir, usar, cerrar), en vez de mantener listas compartidas en
  memoria.
- Se agregaron **transacciones** (`SqlTransaction`) en las operaciones que
  escriben en más de una tabla a la vez, para que no quede el sistema en un
  estado inconsistente si falla un paso intermedio:
  - `JuegoDAOSql.Guardar/Actualizar`: la fila de `Juegos` + las filas de
    `JuegoCategorias` (relación N a N).
  - `CompraDAOSql.Guardar`: la fila de `Compras` + todas las filas de
    `DetallesCompra`.
  - `CampanaDAOSql.Guardar`: la fila de `Campanas` + las filas de
    `CampanaJuegos` (si la campaña apunta a una selección específica de
    juegos).
- Los **Services no cambiaron su lógica de negocio**, solo la fuente de
  datos: donde antes decían `_juegoRepo.GetById(id)` ahora dicen
  `_juegoDAO.BuscarPorId(id)`. Esto es justamente lo que se busca separando
  en capas: la regla de negocio (elegir la mejor promoción, validar que no
  posea ya el juego, etc.) no tuvo que tocarse.

## Cómo se resolvió cada relación en la base

- **Juego ↔ Categoria** (N a N): tabla intermedia `JuegoCategorias`.
- **Campana → Categoria / Desarrolladora** (alcance opcional): columnas
  `CategoriaId`/`DesarrolladoraId` nulas en `Campanas`.
- **Campana ↔ Juego** (selección específica opcional): tabla intermedia
  `CampanaJuegos`.
- **Biblioteca personal**: no es una tabla propia; es la tabla
  `ItemsBiblioteca` (relación N a N Usuario–Juego con datos de uso:
  fecha de adquisición, horas jugadas, última vez usado). El objeto
  `Biblioteca` del dominio se reconstruye en memoria a partir de esas filas
  cada vez que se busca un usuario.
- **Compra → DetalleCompra** (1 a N): `DetallesCompra.CompraId` con
  `ON DELETE CASCADE`.

## Recorrido sugerido para probar en Swagger

1. `POST /api/desarrolladoras` → crear una desarrolladora.
2. `POST /api/categorias` → crear una o más categorías.
3. `POST /api/videojuegos` → crear un juego (`Estado: "Disponible"`).
4. `POST /api/usuarios` → crear un usuario.
5. `GET /api/videojuegos/{id}/precio-actual` → ver el precio sin descuentos.
6. `POST /api/campanas` → crear una campaña que alcance a ese juego, con
   `FechaInicio`/`FechaFin` que incluyan la fecha/hora actual (UTC).
7. `GET /api/videojuegos/{id}/precio-actual` de nuevo → ahora refleja el descuento.
8. `POST /api/compras` con `UsuarioId` y `JuegoIds` → queda en `Pendiente`.
9. `PATCH /api/compras/{id}/confirmar` → pasa a `Confirmada` y el juego
   aparece en `GET /api/usuarios/{id}/biblioteca`.

## Decisiones de diseño a tener en cuenta

- **`CategoriasController` y `DesarrolladorasController`** no están en la
  lista mínima del enunciado, pero son necesarias para poder crear esos
  datos desde afuera y probar Videojuegos/Campañas de punta a punta.
- **Compras en dos pasos** (`POST` crea en `Pendiente`, `PATCH .../confirmar`
  agrega recién ahí a la biblioteca): permite demostrar los tres estados
  (`Pendiente`, `Confirmada`, `Cancelada`) con un flujo real, y revalidar
  disponibilidad/tenencia al confirmar por si algo cambió mientras la
  compra estuvo pendiente.
- **`AddWithValue`** se usa en todos los parámetros de los `SqlCommand` por
  simplicidad y legibilidad; en un proyecto de producción se preferiría
  `Add` con el `SqlDbType` explícito para evitar conversiones implícitas,
  pero para esta entrega priorizamos que el código sea claro.
- Falta manejo de **concurrencia/reintentos de conexión** (por ejemplo, qué
  pasa si SQL Server no está disponible): no se pidió para esta entrega,
  pero es un buen próximo paso si el profesor pregunta por robustez.
