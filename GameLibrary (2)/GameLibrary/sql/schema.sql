-- =====================================================================
-- GameLibrary - Script de creación de base de datos (SQL Server)
-- =====================================================================
-- Ejecutar contra una instancia de SQL Server. Ajustar el nombre de la
-- base si hace falta (debe coincidir con la cadena de conexión en
-- appsettings.json).
-- =====================================================================

IF DB_ID('GameLibraryDb') IS NULL
BEGIN
    CREATE DATABASE GameLibraryDb;
END
GO

USE GameLibraryDb;
GO

-- ---------------------------------------------------------------------
-- Categorias
-- ---------------------------------------------------------------------
IF OBJECT_ID('dbo.Categorias', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categorias
    (
        Id     INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(100) NOT NULL UNIQUE
    );
END
GO

-- ---------------------------------------------------------------------
-- Desarrolladoras
-- ---------------------------------------------------------------------
IF OBJECT_ID('dbo.Desarrolladoras', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Desarrolladoras
    (
        Id     INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(150) NOT NULL UNIQUE
    );
END
GO

-- ---------------------------------------------------------------------
-- Juegos
-- ---------------------------------------------------------------------
IF OBJECT_ID('dbo.Juegos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Juegos
    (
        Id                INT IDENTITY(1,1) PRIMARY KEY,
        Nombre            NVARCHAR(200) NOT NULL,
        Descripcion       NVARCHAR(MAX) NULL,
        Precio            DECIMAL(10,2) NOT NULL,
        FechaLanzamiento  DATE NOT NULL,
        DesarrolladoraId  INT NOT NULL,
        Estado            NVARCHAR(20) NOT NULL DEFAULT 'Proximamente',
        CONSTRAINT FK_Juegos_Desarrolladoras
            FOREIGN KEY (DesarrolladoraId) REFERENCES dbo.Desarrolladoras(Id)
    );
END
GO

-- Relación N a N entre Juegos y Categorias
IF OBJECT_ID('dbo.JuegoCategorias', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.JuegoCategorias
    (
        JuegoId     INT NOT NULL,
        CategoriaId INT NOT NULL,
        CONSTRAINT PK_JuegoCategorias PRIMARY KEY (JuegoId, CategoriaId),
        CONSTRAINT FK_JuegoCategorias_Juegos
            FOREIGN KEY (JuegoId) REFERENCES dbo.Juegos(Id) ON DELETE CASCADE,
        CONSTRAINT FK_JuegoCategorias_Categorias
            FOREIGN KEY (CategoriaId) REFERENCES dbo.Categorias(Id)
    );
END
GO

-- ---------------------------------------------------------------------
-- Usuarios
-- ---------------------------------------------------------------------
IF OBJECT_ID('dbo.Usuarios', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuarios
    (
        Id     INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(150) NOT NULL,
        Email  NVARCHAR(150) NOT NULL UNIQUE
    );
END
GO

-- Biblioteca personal: no se modela como tabla propia, cada fila acá
-- es un ítem de la biblioteca de un usuario (relación N a N Usuario-Juego
-- con datos propios de uso, tal como la clase de asociación del dominio).
IF OBJECT_ID('dbo.ItemsBiblioteca', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ItemsBiblioteca
    (
        Id               INT IDENTITY(1,1) PRIMARY KEY,
        UsuarioId        INT NOT NULL,
        JuegoId          INT NOT NULL,
        FechaAdquisicion DATETIME2 NOT NULL,
        HorasJugadas     FLOAT NOT NULL DEFAULT 0,
        UltimaVezUsado   DATETIME2 NULL,
        CONSTRAINT UQ_ItemsBiblioteca_UsuarioJuego UNIQUE (UsuarioId, JuegoId),
        CONSTRAINT FK_ItemsBiblioteca_Usuarios
            FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id) ON DELETE CASCADE,
        CONSTRAINT FK_ItemsBiblioteca_Juegos
            FOREIGN KEY (JuegoId) REFERENCES dbo.Juegos(Id)
    );
END
GO

-- ---------------------------------------------------------------------
-- Compras
-- ---------------------------------------------------------------------
IF OBJECT_ID('dbo.Compras', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Compras
    (
        Id           INT IDENTITY(1,1) PRIMARY KEY,
        UsuarioId    INT NOT NULL,
        Fecha        DATETIME2 NOT NULL,
        ImporteFinal DECIMAL(10,2) NOT NULL,
        Estado       NVARCHAR(20) NOT NULL DEFAULT 'Pendiente',
        CONSTRAINT FK_Compras_Usuarios
            FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id)
    );
END
GO

IF OBJECT_ID('dbo.DetallesCompra', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DetallesCompra
    (
        Id           INT IDENTITY(1,1) PRIMARY KEY,
        CompraId     INT NOT NULL,
        JuegoId      INT NOT NULL,
        PrecioFinal  DECIMAL(10,2) NOT NULL,
        CONSTRAINT FK_DetallesCompra_Compras
            FOREIGN KEY (CompraId) REFERENCES dbo.Compras(Id) ON DELETE CASCADE,
        CONSTRAINT FK_DetallesCompra_Juegos
            FOREIGN KEY (JuegoId) REFERENCES dbo.Juegos(Id)
    );
END
GO

-- ---------------------------------------------------------------------
-- Campanas
-- ---------------------------------------------------------------------
IF OBJECT_ID('dbo.Campanas', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Campanas
    (
        Id                  INT IDENTITY(1,1) PRIMARY KEY,
        Nombre              NVARCHAR(150) NOT NULL,
        FechaInicio         DATETIME2 NOT NULL,
        FechaFin            DATETIME2 NOT NULL,
        PorcentajeDescuento DECIMAL(5,2) NOT NULL,
        CategoriaId         INT NULL,
        DesarrolladoraId    INT NULL,
        CONSTRAINT FK_Campanas_Categorias
            FOREIGN KEY (CategoriaId) REFERENCES dbo.Categorias(Id),
        CONSTRAINT FK_Campanas_Desarrolladoras
            FOREIGN KEY (DesarrolladoraId) REFERENCES dbo.Desarrolladoras(Id)
    );
END
GO

-- Selección específica de juegos alcanzados por una campaña (opcional).
IF OBJECT_ID('dbo.CampanaJuegos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CampanaJuegos
    (
        CampanaId INT NOT NULL,
        JuegoId   INT NOT NULL,
        CONSTRAINT PK_CampanaJuegos PRIMARY KEY (CampanaId, JuegoId),
        CONSTRAINT FK_CampanaJuegos_Campanas
            FOREIGN KEY (CampanaId) REFERENCES dbo.Campanas(Id) ON DELETE CASCADE,
        CONSTRAINT FK_CampanaJuegos_Juegos
            FOREIGN KEY (JuegoId) REFERENCES dbo.Juegos(Id)
    );
END
GO
