CREATE TABLE Heroes (
    Id INTEGER NOT NULL CONSTRAINT PK_Heroes PRIMARY KEY AUTOINCREMENT,
    Nombre TEXT NOT NULL,
    Ciudad TEXT NOT NULL,
    IdentidadSecreta TEXT NULL
);

CREATE TABLE SuperPoderes (
    Id INTEGER NOT NULL CONSTRAINT PK_SuperPoderes PRIMARY KEY AUTOINCREMENT,
    Nombre TEXT NOT NULL,
    Descripcion TEXT NULL,
    HeroeId INTEGER NOT NULL,
    CONSTRAINT FK_SuperPoderes_Heroes FOREIGN KEY (HeroeId) REFERENCES Heroes (Id) ON DELETE CASCADE
);

INSERT INTO Heroes (Nombre, Ciudad, IdentidadSecreta) VALUES
('Superman', 'Metrópolis', 'Clark Kent'),
('Batman', 'Gotham', 'Bruce Wayne');

INSERT INTO SuperPoderes (Nombre, Descripcion, HeroeId) VALUES
('Volar', 'Puede desplazarse por el aire.', 1),
('Superfuerza', 'Tiene una fuerza extraordinaria.', 1),
('Visión de calor', 'Emite rayos de energía desde los ojos.', 1),
('Inteligencia estratégica', 'Planifica y analiza situaciones complejas.', 2),
('Artes marciales', 'Tiene entrenamiento físico y de combate.', 2);
