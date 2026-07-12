/*
  Script minimo inicial - Proyecto Web Avanzado
  Motor: SQL Server
  Objetivo: Crear tablas base para Roles, Usuarios y Actividades
*/

-- 1) Crear base de datos (opcional si ya existe)
IF DB_ID('ProyectoWebAvanzadoDB') IS NULL
BEGIN
    CREATE DATABASE ProyectoWebAvanzadoDB;
END;
GO

USE ProyectoWebAvanzadoDB;
GO

-- 2) Eliminar tablas en orden seguro (solo para ambiente de desarrollo)
IF OBJECT_ID('dbo.Actividades', 'U') IS NOT NULL DROP TABLE dbo.Actividades;
IF OBJECT_ID('dbo.Usuarios', 'U') IS NOT NULL DROP TABLE dbo.Usuarios;
IF OBJECT_ID('dbo.Roles', 'U') IS NOT NULL DROP TABLE dbo.Roles;
GO

-- 3) Tabla centralizada de roles
CREATE TABLE dbo.Roles (
    RolId INT IDENTITY(1,1) NOT NULL,
    Nombre NVARCHAR(50) NOT NULL,
    Descripcion NVARCHAR(200) NULL,
    Estado CHAR NOT NULL CONSTRAINT DF_Roles_Estado DEFAULT ('A'),
    FechaCreacion DATETIME2(0) NOT NULL CONSTRAINT DF_Roles_FechaCreacion DEFAULT (SYSDATETIME()),
    CONSTRAINT PK_Roles PRIMARY KEY (RolId),
    CONSTRAINT UQ_Roles_Nombre UNIQUE (Nombre),
    CONSTRAINT CK_Roles_Estado CHECK (Estado IN ('A', 'I', 'N'))
);
GO

-- 4) Tabla de usuarios
CREATE TABLE dbo.Usuarios (
    UsuarioId INT IDENTITY(1,1) NOT NULL,
    Nombre NVARCHAR(120) NOT NULL,
    Email NVARCHAR(120) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    RolId INT NOT NULL,
    Estado NVARCHAR(1) NOT NULL CONSTRAINT DF_Usuarios_Estado DEFAULT ('A'),
    FechaRegistro DATE NOT NULL CONSTRAINT DF_Usuarios_FechaRegistro DEFAULT (CAST(GETDATE() AS DATE)),
    FechaActualizacion DATETIME2(0) NOT NULL CONSTRAINT DF_Usuarios_FechaActualizacion DEFAULT (SYSDATETIME()),
    CONSTRAINT PK_Usuarios PRIMARY KEY (UsuarioId),
    CONSTRAINT UQ_Usuarios_Email UNIQUE (Email),
    CONSTRAINT FK_Usuarios_Roles FOREIGN KEY (RolId) REFERENCES dbo.Roles(RolId),
    CONSTRAINT CK_Usuarios_Estado CHECK (Estado IN ('A', 'I', 'N'))
);
GO

-- 5) Tabla de actividades
CREATE TABLE dbo.Actividades (
    ActividadId INT IDENTITY(1,1) NOT NULL,
    Nombre NVARCHAR(150) NOT NULL,
    Tipo NVARCHAR(50) NOT NULL,
    Nivel NVARCHAR(30) NOT NULL,
    Estado NVARCHAR(1) NOT NULL CONSTRAINT DF_Actividades_Estado DEFAULT ('A'),
    Fecha DATE NOT NULL,
    ResponsableUsuarioId INT NULL,
    FechaCreacion DATETIME2(0) NOT NULL CONSTRAINT DF_Actividades_FechaCreacion DEFAULT (SYSDATETIME()),
    FechaActualizacion DATETIME2(0) NOT NULL CONSTRAINT DF_Actividades_FechaActualizacion DEFAULT (SYSDATETIME()),
    CONSTRAINT PK_Actividades PRIMARY KEY (ActividadId),
    CONSTRAINT FK_Actividades_Responsable FOREIGN KEY (ResponsableUsuarioId) REFERENCES dbo.Usuarios(UsuarioId),
    CONSTRAINT CK_Actividades_Estado CHECK (Estado IN ('A', 'I', 'N')),
    CONSTRAINT CK_Actividades_Nivel CHECK (Nivel IN ('Inicial', 'Basico', 'Intermedio', 'Avanzado'))
);
GO

-- 6) Indices utiles para consultas comunes
CREATE INDEX IX_Usuarios_RolId ON dbo.Usuarios (RolId);
CREATE INDEX IX_Usuarios_Estado ON dbo.Usuarios (Estado);
CREATE INDEX IX_Actividades_Estado ON dbo.Actividades (Estado);
CREATE INDEX IX_Actividades_ResponsableUsuarioId ON dbo.Actividades (ResponsableUsuarioId);
GO

-- 7) Datos semilla minimos de roles
INSERT INTO dbo.Roles (Nombre, Descripcion, Estado)
VALUES
('Admin', 'Control total de la plataforma', 'A'),
('Docente', 'Gestiona actividades y seguimiento academico', 'A'),
('Padre', 'Consulta avance y participa en seguimiento', 'A');
GO

-- 8) Usuario administrador inicial
-- Contraseña real: 12345  (ya viene hasheada con el PasswordHasher de ASP.NET Core Identity,
-- que es el mismo algoritmo que usa el backend para verificar el login)
INSERT INTO dbo.Usuarios (Nombre, Email, PasswordHash, RolId, Estado)
SELECT
    'Administrador Inicial',
    'admin@proyecto.local',
    '$2a$11$NhDtr6SG0nQW.nF3gjjYbekexE0vaRqEEABKrz0E9AolrCJIOOQr6',
    r.RolId,
    'A'
FROM dbo.Roles r
WHERE r.Nombre = 'Admin';
GO

-- 9) Consulta rapida de verificacion
SELECT 'Roles' AS Tabla, COUNT(*) AS Total FROM dbo.Roles
UNION ALL
SELECT 'Usuarios', COUNT(*) FROM dbo.Usuarios
UNION ALL
SELECT 'Actividades', COUNT(*) FROM dbo.Actividades;
GO


SELECT * FROM Roles;
SELECT * FROM Usuarios;
SELECT * FROM Actividades;