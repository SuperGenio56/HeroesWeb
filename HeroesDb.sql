CREATE DATABASE HeroesDb;
GO

USE HeroesDb;
GO

CREATE TABLE Heroes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Ciudad NVARCHAR(100) NOT NULL,
    IdentidadSecreta NVARCHAR(100) NULL
);
GO

CREATE TABLE SuperPoderes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(250) NULL,
    HeroeId INT NOT NULL,

    CONSTRAINT FK_SuperPoderes_Heroes
        FOREIGN KEY (HeroeId)
        REFERENCES Heroes(Id)
        ON DELETE CASCADE
);
GO

INSERT INTO Heroes (Nombre, Ciudad, IdentidadSecreta)
VALUES
(N'Superman', N'Metrópolis', N'Clark Kent'),
(N'Batman', N'Gotham', N'Bruce Wayne');
GO

INSERT INTO SuperPoderes (Nombre, Descripcion, HeroeId)
VALUES
(N'Volar', N'Puede desplazarse por el aire.', 1),
(N'Superfuerza', N'Tiene una fuerza extraordinaria.', 1),
(N'Visión de calor', N'Emite rayos de energía desde los ojos.', 1),
(N'Inteligencia estratégica', N'Planifica y analiza situaciones complejas.', 2),
(N'Artes marciales', N'Tiene entrenamiento físico y de combate.', 2);
GO
