-- =============================================================
-- BibliotecaMVC - Script de base de datos para el modulo de Categorias
-- Ejecutar en SQL Server Management Studio (SSMS)
-- =============================================================

IF DB_ID('BibliotecaDB') IS NULL
BEGIN
    CREATE DATABASE BibliotecaDB;
END
GO

USE BibliotecaDB;
GO

IF OBJECT_ID('dbo.Categorias', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categorias
    (
        Id          INT IDENTITY(1,1) NOT NULL,
        Nombre      NVARCHAR(100)     NOT NULL,
        Descripcion NVARCHAR(250)     NULL,
        CONSTRAINT PK_Categorias PRIMARY KEY (Id)
    );
END
GO

-- Datos de prueba (solo si la tabla esta vacia)
IF NOT EXISTS (SELECT 1 FROM dbo.Categorias)
BEGIN
    INSERT INTO dbo.Categorias (Nombre, Descripcion) VALUES
        (N'Literatura',  N'Novelas, cuentos y poesia.'),
        (N'Ciencia',     N'Libros de fisica, quimica, biologia y matematica.'),
        (N'Historia',    N'Textos historicos y biografias.'),
        (N'Tecnologia',  N'Programacion, redes y sistemas informaticos.');
END
GO

SELECT Id, Nombre, Descripcion FROM dbo.Categorias;
GO
