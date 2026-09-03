# Trabajo Práctico Integrador

## Datos del proyecto

**Tema:**
Plataforma de Videojuegos — Biblioteca y Tienda Digital (estilo Steam / Epic
Games). Catálogo de videojuegos, usuarios con biblioteca personal, compras
con validaciones de negocio, y campañas comerciales con descuentos.

**Integrantes:**
- Apellido, Nombre
- Apellido, Nombre
- Apellido, Nombre
- Apellido, Nombre
- Apellido, Nombre

> ⚠️ Completar con los nombres reales antes de entregar.

## Enunciado

[Ver enunciado del TP](https://docs.google.com/document/d/1TDWUL1pNOTdqh9Lj__R_p1-kE66S-5o61o-rEIEEyQ8/edit?usp=sharing)

El detalle completo del dominio (entidades, reglas de negocio, estados)
está en [`docs/requerimientos.md`](docs/requerimientos.md).

---

## Estructura del repositorio

```text
/
├── docs/
│   ├── requerimientos.md
│   ├── endpoints.md
│   ├── schema.sql
│   └── diagrama-clases/
│       ├── diagrama-clases.png
│       └── diagrama-clases.pdf
│
├── backend/
│   ├── tpintegrador_psr2026.sln
│   ├── tpintegrador_psr2026.Api/
│       ├── Controllers/
│       ├── Services/
│       ├── DAO/
│       ├── Domain/
│       ├── DTOs/
│       ├── Middleware/
│       ├── Program.cs
│       └── tpintegrador_psr2026.Api.csproj
│   └── tpintegrador_psr2026.Tests/
│
└── frontend/
```

## Puesta en marcha

Requisitos: Node.js, npm, .NET SDK 10, y una instancia de **SQL Server**
accesible (local, LocalDB, o en Docker).

### 1. Base de datos

Ejecutar `docs/schema.sql` contra la instancia de SQL Server (crea la base
`SteamTruchoDb` y todas las tablas). Se puede correr desde SSMS, Azure Data
Studio, o `sqlcmd`.

### 2. Cadena de conexión

Revisar `backend/tpintegrador_psr2026.Api/appsettings.json` →
`ConnectionStrings:Default`. Por defecto asume autenticación de Windows
contra una instancia local:

```json
"Default": "Server=localhost;Database=SteamTruchoDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

Ajustarla si se usa LocalDB, Docker, o autenticación SQL (usuario/contraseña).

### 3. Levantar la API

Desde la raíz del repositorio:

```bash
npm run dev
```

La API queda disponible en `http://localhost:5080`. Para comprobarla:

- `GET /`
- `GET /api/health`
- `GET /swagger` → documentación interactiva de todos los endpoints.

Otros comandos útiles:

```bash
npm run build
npm test
```

### `docs/`
Documentación del proyecto: requerimientos, endpoints, script de base de
datos y diagrama de clases.

### `Domain/`
Clases principales del sistema: `Juego`, `Usuario`, `Biblioteca`,
`ItemBiblioteca`, `Categoria`, `Desarrolladora`, `Compra`, `DetalleCompra`,
`Campana`.

### `DAO/`
Acceso a los datos, contra SQL Server con ADO.NET (`SqlConnection`/
`SqlCommand`). Se encarga de buscar, guardar y modificar información
(`ListarTodos`, `BuscarPorId`, `Guardar`, `Actualizar`, etc.).

### `Services/`
Lógica y reglas de negocio del sistema: verificar disponibilidad de un
juego, impedir recompra, calcular el precio final aplicando la mejor
promoción activa, confirmar/cancelar compras, agregar juegos a la
biblioteca.

### `Controllers/`
Reciben las peticiones HTTP, utilizan los Services y devuelven las
respuestas de la API. No contienen lógica de negocio.

---

## Flujo del backend

```text
Frontend
   ↓
Controller
   ↓
Service
   ↓
DAO
   ↓
Datos
```

El **frontend** es la interfaz que utiliza el usuario (a definir/desarrollar
por el grupo).

El **backend** está desarrollado en **C# / .NET 10** y es responsable de la
lógica, los datos (SQL Server) y la API REST.

---

## API REST

El frontend se comunica con el backend mediante HTTP y JSON.

Los endpoints desarrollados están documentados en:

```text
docs/endpoints.md
```

---

## Reglas generales

- Mantener separadas las responsabilidades de cada capa.
- No colocar la lógica del sistema en los Controllers.
- El acceso a datos debe realizarse desde los DAO.
- Las reglas de negocio deben estar en los Services.
- El frontend debe comunicarse con el backend mediante la API REST.
- Mantener actualizada la documentación.
- No subir contraseñas, tokens ni credenciales al repositorio.

---

## Colaboradores

Este repositorio debe estar compartido entre todos los integrantes del
grupo como colaboradores. Para agregarlos (lo hace quien creó el
repositorio, con rol de administrador):

1. En GitHub, entrar al repositorio → **Settings**.
2. En el menú lateral, **Collaborators**.
3. **Add people** → buscar por usuario de GitHub o email → enviar invitación.
4. Cada integrante debe aceptar la invitación desde su correo o desde
   `github.com/<usuario>` → notificaciones.

Repetir el paso 3 por cada integrante que falte.
