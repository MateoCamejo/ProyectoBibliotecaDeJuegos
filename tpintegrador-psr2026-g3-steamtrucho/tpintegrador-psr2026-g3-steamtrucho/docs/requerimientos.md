# Requerimientos — Plataforma de Videojuegos: Biblioteca y Tienda Digital

## 1. Temática

Sistema de gestión de usuarios, videojuegos, compras, bibliotecas personales
y campañas comerciales dentro de una plataforma digital de videojuegos.

## 2. Descripción del sistema

La Plataforma de Videojuegos es un servicio digital que permite a sus
usuarios explorar un catálogo de videojuegos, realizar compras y construir
progresivamente su propia biblioteca personal.

La plataforma administra los videojuegos disponibles para la venta, sus
características, precios, categorías y desarrolladoras, además de mantener
información sobre los usuarios registrados y las compras realizadas.

Una vez adquirido un videojuego, este se incorpora automáticamente a la
biblioteca del usuario y permanece disponible dentro de ella.

La plataforma participa periódicamente de campañas comerciales durante las
cuales determinados videojuegos reciben descuentos especiales. En futuras
etapas estas campañas se recibirán desde un servicio externo mediante una
API; por ahora el sistema debe permitir representarlas y gestionarlas
internamente.

### 2.1 Videojuegos

Cada videojuego tiene nombre, descripción, precio y fecha de lanzamiento.
Es publicado por una desarrolladora y puede pertenecer a una o varias
categorías (Acción, Aventura, Estrategia, Deportes, Simulación, Terror,
RPG, Carreras, Puzzle, entre otras).

Estados posibles:

- **Disponible**: puede adquirirse normalmente.
- **Próximamente**: publicado, pero todavía no puede comprarse.
- **Retirado**: ya no está disponible para nuevas compras (pero permanece
  en la biblioteca de quienes ya lo adquirieron).

### 2.2 Usuarios

Cada usuario tiene datos de identificación y una biblioteca personal.
Pueden realizar compras, consultar el catálogo, buscar videojuegos y ver
los títulos que ya poseen. **No pueden volver a comprar un juego que ya
tienen.**

### 2.3 Biblioteca personal

Cada usuario tiene una biblioteca donde se incorporan automáticamente los
videojuegos al completarse una compra. Un juego permanece en la biblioteca
aunque luego sea retirado de la tienda. Puede almacenar además: fecha de
adquisición, horas jugadas, última vez utilizado y estado dentro de la
biblioteca.

### 2.4 Compras

Cada compra registra el usuario, los videojuegos adquiridos, la fecha y el
importe final (que puede verse afectado por descuentos de campañas
activas). Antes de completarse, el sistema verifica:

- Que el videojuego esté disponible para la venta.
- Que el usuario no lo posea todavía.
- Que el precio utilizado sea el vigente al momento de la operación.
- Que se apliquen correctamente las promociones correspondientes.

Estados posibles: **Pendiente**, **Confirmada**, **Cancelada**. Una compra
confirmada agrega los juegos a la biblioteca; una cancelada no la modifica.

### 2.5 Desarrolladoras

Cada videojuego pertenece a una desarrolladora responsable de su creación y
publicación. Una desarrolladora puede tener varios videojuegos publicados
simultáneamente.

### 2.6 Campañas y eventos

Cada campaña tiene fecha de inicio y fin, y define qué videojuegos alcanza
y qué beneficio aplica (por ejemplo, un porcentaje de descuento). El
alcance puede definirse por categoría, por desarrolladora, por una
selección específica de juegos, o combinaciones de estos criterios.

### 2.7 Promociones simultáneas

Un mismo videojuego puede cumplir las condiciones de más de una campaña
activa a la vez. **Los descuentos no son acumulables**: se aplica la
promoción más beneficiosa para el usuario.

## 3. Reglas generales de negocio

- Un usuario no puede comprar nuevamente un videojuego que ya está en su
  biblioteca.
- Solo pueden comprarse videojuegos en estado **Disponible**.
- Un videojuego **Retirado** no puede generar nuevas ventas, pero
  permanece en las bibliotecas de quienes ya lo compraron.
- Una compra **Confirmada** agrega automáticamente los videojuegos a la
  biblioteca del usuario; una **Cancelada** no modifica la biblioteca.
- Las campañas solo aplican mientras están vigentes.
- Si un videojuego participa de varias campañas activas, se usa la más
  conveniente para el usuario (los descuentos no se acumulan).
- Las compras conservan el precio final utilizado al momento de la
  operación, aunque después cambie el precio del videojuego.

## 4. Alcance de esta entrega

- **Domain**: clases, atributos, relaciones y estados descriptos arriba.
- **DAO**: acceso a datos contra SQL Server (alta, búsqueda, listados).
- **Services**: reglas de negocio — verificar disponibilidad, impedir
  recompra, calcular precio final, detectar campañas activas, elegir la
  mejor promoción, confirmar/cancelar compras, mantener el historial de
  precios pagados.
- **Controllers**: API REST documentada en [`endpoints.md`](./endpoints.md).

Ver también el [diagrama de clases](./diagrama-clases/) y el
[script de base de datos](./schema.sql).
