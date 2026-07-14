USE ProyectoWebAvanzadoDB;

SET FOREIGN_KEY_CHECKS = 0;
DROP TABLE IF EXISTS Progresos;
DROP TABLE IF EXISTS Actividades;
DROP TABLE IF EXISTS Usuarios;
DROP TABLE IF EXISTS Roles;
SET FOREIGN_KEY_CHECKS = 1;

CREATE TABLE Roles (
    RolId INT NOT NULL AUTO_INCREMENT,
    Nombre VARCHAR(50) NOT NULL,
    Descripcion VARCHAR(200) NULL,
    Estado CHAR(1) NOT NULL DEFAULT 'A',
    FechaCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT PK_Roles PRIMARY KEY (RolId),
    CONSTRAINT UQ_Roles_Nombre UNIQUE (Nombre),
    CONSTRAINT CK_Roles_Estado CHECK (Estado IN ('A', 'I', 'N'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE Usuarios (
    UsuarioId INT NOT NULL AUTO_INCREMENT,
    Nombre VARCHAR(120) NOT NULL,
    Email VARCHAR(120) NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    RolId INT NOT NULL,
    Estado CHAR(1) NOT NULL DEFAULT 'A',
    FechaRegistro DATE NOT NULL DEFAULT CURRENT_DATE,
    FechaActualizacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT PK_Usuarios PRIMARY KEY (UsuarioId),
    CONSTRAINT UQ_Usuarios_Email UNIQUE (Email),
    CONSTRAINT FK_Usuarios_Roles FOREIGN KEY (RolId) REFERENCES Roles(RolId) ON DELETE RESTRICT,
    CONSTRAINT CK_Usuarios_Estado CHECK (Estado IN ('A', 'I', 'N'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE Actividades (
    ActividadId INT NOT NULL AUTO_INCREMENT,
    Nombre VARCHAR(150) NOT NULL,
    Tipo VARCHAR(50) NOT NULL,
    Nivel VARCHAR(30) NOT NULL,
    Estado CHAR(1) NOT NULL DEFAULT 'A',
    Fecha DATE NOT NULL,
    ResponsableUsuarioId INT NULL,
    FechaCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FechaActualizacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT PK_Actividades PRIMARY KEY (ActividadId),
    CONSTRAINT FK_Actividades_Responsable FOREIGN KEY (ResponsableUsuarioId) REFERENCES Usuarios(UsuarioId) ON DELETE SET NULL,
    CONSTRAINT CK_Actividades_Estado CHECK (Estado IN ('A', 'I', 'N')),
    CONSTRAINT CK_Actividades_Nivel CHECK (Nivel IN ('Inicial', 'Basico', 'Intermedio', 'Avanzado'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE Progresos (
    ProgresoId INT NOT NULL AUTO_INCREMENT,
    UsuarioId INT NOT NULL,
    ActividadId INT NOT NULL,
    AvancePorcentaje DECIMAL(5,2) NOT NULL DEFAULT 0,
    Nivel VARCHAR(30) NOT NULL,
    Estado CHAR(1) NOT NULL DEFAULT 'A',
    FechaRegistro DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FechaActualizacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT PK_Progresos PRIMARY KEY (ProgresoId),
    CONSTRAINT UQ_Progresos_Usuario_Actividad UNIQUE (UsuarioId, ActividadId),
    CONSTRAINT FK_Progresos_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios(UsuarioId) ON DELETE CASCADE,
    CONSTRAINT FK_Progresos_Actividades FOREIGN KEY (ActividadId) REFERENCES Actividades(ActividadId) ON DELETE CASCADE,
    CONSTRAINT CK_Progresos_Avance CHECK (AvancePorcentaje BETWEEN 0 AND 100),
    CONSTRAINT CK_Progresos_Estado CHECK (Estado IN ('A', 'I', 'N'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE INDEX IX_Usuarios_RolId ON Usuarios (RolId);
CREATE INDEX IX_Usuarios_Estado ON Usuarios (Estado);
CREATE INDEX IX_Actividades_Estado ON Actividades (Estado);
CREATE INDEX IX_Actividades_ResponsableUsuarioId ON Actividades (ResponsableUsuarioId);
CREATE INDEX IX_Progresos_UsuarioId ON Progresos (UsuarioId);
CREATE INDEX IX_Progresos_ActividadId ON Progresos (ActividadId);

INSERT INTO Roles (Nombre, Descripcion, Estado)
VALUES
('Admin', 'Control total de la plataforma', 'A'),
('Docente', 'Gestiona actividades y seguimiento academico', 'A'),
('Padre', 'Consulta avance y participa en seguimiento', 'A');

INSERT INTO Usuarios (Nombre, Email, PasswordHash, RolId, Estado)
SELECT
    'Administrador Inicial',
    'admin@proyecto.local',
    '$2a$11$aAGEL0Ni2JhILGNGoKShOesglqDp.O2ovuQhrZewv7w/mahXwNx0G',
    RolId,
    'A'
FROM Roles
WHERE Nombre = 'Admin';

INSERT INTO Actividades (Nombre, Tipo, Nivel, Estado, Fecha, ResponsableUsuarioId)
SELECT 'Reconocer vocales', 'Letras', 'Inicial', 'A', CURRENT_DATE, UsuarioId
FROM Usuarios
WHERE Email = 'admin@proyecto.local';

INSERT INTO Actividades (Nombre, Tipo, Nivel, Estado, Fecha, ResponsableUsuarioId)
SELECT 'Contar del 1 al 10', 'Numeros', 'Inicial', 'A', CURRENT_DATE, UsuarioId
FROM Usuarios
WHERE Email = 'admin@proyecto.local';

INSERT INTO Actividades (Nombre, Tipo, Nivel, Estado, Fecha, ResponsableUsuarioId)
SELECT 'Memoria', 'Juego educativo', 'Inicial', 'A', CURRENT_DATE, UsuarioId
FROM Usuarios
WHERE Email = 'admin@proyecto.local';

INSERT INTO Actividades (Nombre, Tipo, Nivel, Estado, Fecha, ResponsableUsuarioId)
SELECT 'Colores', 'Juego educativo', 'Inicial', 'A', CURRENT_DATE, UsuarioId
FROM Usuarios
WHERE Email = 'admin@proyecto.local';

INSERT INTO Actividades (Nombre, Tipo, Nivel, Estado, Fecha, ResponsableUsuarioId)
SELECT 'Rompecabezas', 'Juego educativo', 'Intermedio', 'A', CURRENT_DATE, UsuarioId
FROM Usuarios
WHERE Email = 'admin@proyecto.local';

INSERT INTO Actividades (Nombre, Tipo, Nivel, Estado, Fecha, ResponsableUsuarioId)
SELECT 'Asociacion', 'Juego educativo', 'Intermedio', 'A', CURRENT_DATE, UsuarioId
FROM Usuarios
WHERE Email = 'admin@proyecto.local';
