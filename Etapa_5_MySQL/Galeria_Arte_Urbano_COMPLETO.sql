-- =====================================================================
--  Galería de Arte Urbano de Cali — SCRIPT COMPLETO (01 + 02 + 03 + 04 en un solo archivo)
--  Generado uniendo los cuatro scripts de esta carpeta. Ejecutarlo completo en MySQL Workbench.
-- =====================================================================


-- =====================================================================
--  Galería de Arte Urbano de Cali
--  Etapa 5 — Script 01: Estructura (base de datos, tablas y restricciones)
--  Motor: MySQL 8.0.16 o superior (requiere CHECK y DEFAULT (UUID()))
--  Integrantes: Juan Pablo Campo Bertin · Jean Deiby Garcia
--  Orden de ejecución: 01_estructura.sql → 02_datos_prueba.sql → 03_vistas_procedimientos_indices.sql → 04_consultas.sql
-- =====================================================================

SET NAMES utf8mb4;   -- la conexión envía tildes y ñ correctamente
DROP DATABASE IF EXISTS galeria_arte_urbano_cali;
CREATE DATABASE galeria_arte_urbano_cali
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_0900_ai_ci;   -- ai = insensible a tildes: 'Grafiti' y 'Grafíti' se consideran duplicados

USE galeria_arte_urbano_cali;

-- ---------------------------------------------------------------------
-- 1. USUARIO: personas con acceso a la aplicación
-- ---------------------------------------------------------------------
CREATE TABLE usuario (
    id_usuario      CHAR(36)     NOT NULL DEFAULT (UUID()),
    nombre          VARCHAR(50)  NOT NULL,
    apellido        VARCHAR(50)  NOT NULL,
    correo          VARCHAR(100) NOT NULL,
    password_hash   VARCHAR(255) NOT NULL,
    tipo_usuario    VARCHAR(20)  NOT NULL DEFAULT 'VISITANTE',
    fecha_registro  DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    estado          VARCHAR(10)  NOT NULL DEFAULT 'ACTIVO',
    CONSTRAINT pk_usuario          PRIMARY KEY (id_usuario),
    CONSTRAINT uq_usuario_correo   UNIQUE (correo),
    CONSTRAINT ck_usuario_nombre   CHECK (TRIM(nombre) <> '' AND TRIM(apellido) <> ''),
    CONSTRAINT ck_usuario_correo   CHECK (correo LIKE '%_@_%._%'),
    -- Regla 7: la contraseña nunca en texto plano. Solo se acepta un hash BCrypt ($2a$/$2b$/$2y$, 60 caracteres)
    CONSTRAINT ck_usuario_hash     CHECK (password_hash REGEXP '^\\$2[aby]\\$[0-9]{2}\\$.{53}$'),
    CONSTRAINT ck_usuario_tipo     CHECK (tipo_usuario IN ('ADMINISTRADOR', 'VISITANTE')),
    CONSTRAINT ck_usuario_estado   CHECK (estado IN ('ACTIVO', 'INACTIVO'))
) ENGINE = InnoDB;

-- ---------------------------------------------------------------------
-- 2. ARTISTA: autores de las obras (borrado lógico si tiene obras)
-- ---------------------------------------------------------------------
CREATE TABLE artista (
    id_artista        CHAR(36)     NOT NULL DEFAULT (UUID()),
    nombre_artistico  VARCHAR(100) NOT NULL,
    nombre_real       VARCHAR(100) NULL,
    biografia         TEXT         NULL,
    pais_origen       VARCHAR(60)  NOT NULL DEFAULT 'Colombia',
    red_social        VARCHAR(150) NULL,
    estado            VARCHAR(10)  NOT NULL DEFAULT 'ACTIVO',
    CONSTRAINT pk_artista            PRIMARY KEY (id_artista),
    CONSTRAINT uq_artista_nombre     UNIQUE (nombre_artistico),
    CONSTRAINT ck_artista_nombre     CHECK (TRIM(nombre_artistico) <> ''),
    CONSTRAINT ck_artista_red        CHECK (red_social IS NULL OR red_social LIKE 'http%' OR red_social LIKE '@%'),
    CONSTRAINT ck_artista_estado     CHECK (estado IN ('ACTIVO', 'INACTIVO'))
) ENGINE = InnoDB;

-- ---------------------------------------------------------------------
-- 3. CATEGORIA: clasificación técnica o temática
-- ---------------------------------------------------------------------
CREATE TABLE categoria (
    id_categoria  CHAR(36)    NOT NULL DEFAULT (UUID()),
    nombre        VARCHAR(50) NOT NULL,
    descripcion   TEXT        NULL,
    estado        VARCHAR(10) NOT NULL DEFAULT 'ACTIVO',
    CONSTRAINT pk_categoria         PRIMARY KEY (id_categoria),
    CONSTRAINT uq_categoria_nombre  UNIQUE (nombre),
    CONSTRAINT ck_categoria_nombre  CHECK (TRIM(nombre) <> ''),
    CONSTRAINT ck_categoria_estado  CHECK (estado IN ('ACTIVO', 'INACTIVO'))
) ENGINE = InnoDB;

-- ---------------------------------------------------------------------
-- 4. BARRIO (nueva en 3FN): elimina la dependencia transitiva barrio → comuna
-- ---------------------------------------------------------------------
CREATE TABLE barrio (
    id_barrio  INT UNSIGNED     NOT NULL AUTO_INCREMENT,
    nombre     VARCHAR(60)      NOT NULL,
    comuna     TINYINT UNSIGNED NOT NULL,
    CONSTRAINT pk_barrio         PRIMARY KEY (id_barrio),
    CONSTRAINT uq_barrio_nombre  UNIQUE (nombre),
    CONSTRAINT ck_barrio_comuna  CHECK (comuna BETWEEN 1 AND 22)
) ENGINE = InnoDB;

-- ---------------------------------------------------------------------
-- 5. UBICACION: lugar físico donde está la obra
-- ---------------------------------------------------------------------
CREATE TABLE ubicacion (
    id_ubicacion  CHAR(36)      NOT NULL DEFAULT (UUID()),
    nombre_lugar  VARCHAR(100)  NOT NULL,
    direccion     VARCHAR(120)  NOT NULL,
    id_barrioFK   INT UNSIGNED  NOT NULL,
    latitud       DECIMAL(10,8) NOT NULL,
    longitud      DECIMAL(11,8) NOT NULL,
    CONSTRAINT pk_ubicacion            PRIMARY KEY (id_ubicacion),
    CONSTRAINT uq_ubicacion_lugar_dir  UNIQUE (nombre_lugar, direccion),
    CONSTRAINT ck_ubicacion_textos     CHECK (TRIM(nombre_lugar) <> '' AND TRIM(direccion) <> ''),
    -- RN-06: coordenadas dentro del área de Cali
    CONSTRAINT ck_ubicacion_latitud    CHECK (latitud  BETWEEN 3.25   AND 3.55),
    CONSTRAINT ck_ubicacion_longitud   CHECK (longitud BETWEEN -76.60 AND -76.45),
    CONSTRAINT fk_ubicacion_barrio     FOREIGN KEY (id_barrioFK) REFERENCES barrio (id_barrio)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB;

-- ---------------------------------------------------------------------
-- 6. OBRA: entidad central
-- ---------------------------------------------------------------------
CREATE TABLE obra (
    id_obra              CHAR(36)     NOT NULL DEFAULT (UUID()),
    nombre               VARCHAR(120) NOT NULL,
    descripcion          TEXT         NOT NULL,
    fecha_creacion       DATE         NULL,
    estado_conservacion  VARCHAR(15)  NOT NULL DEFAULT 'BUENO',
    fecha_registro       DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    id_artistaFK         CHAR(36)     NOT NULL,
    id_categoriaFK       CHAR(36)     NOT NULL,
    id_ubicacionFK       CHAR(36)     NOT NULL,
    id_usuarioFK         CHAR(36)     NOT NULL,
    CONSTRAINT pk_obra                 PRIMARY KEY (id_obra),
    -- Evita duplicados: no puede haber dos obras con el mismo nombre en el mismo lugar
    CONSTRAINT uq_obra_ubicacion_nombre UNIQUE (id_ubicacionFK, nombre),
    CONSTRAINT ck_obra_nombre          CHECK (TRIM(nombre) <> ''),
    CONSTRAINT ck_obra_estado          CHECK (estado_conservacion IN ('EXCELENTE', 'BUENO', 'DETERIORADO', 'DESTRUIDO')),
    CONSTRAINT ck_obra_fechas          CHECK (fecha_creacion IS NULL OR
                                              (fecha_creacion >= '1950-01-01' AND fecha_creacion <= DATE(fecha_registro))),
    -- RN-02: no se borra un artista/categoría/ubicación/usuario que tenga obras
    CONSTRAINT fk_obra_artista    FOREIGN KEY (id_artistaFK)   REFERENCES artista   (id_artista)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_obra_categoria  FOREIGN KEY (id_categoriaFK) REFERENCES categoria (id_categoria)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_obra_ubicacion  FOREIGN KEY (id_ubicacionFK) REFERENCES ubicacion (id_ubicacion)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_obra_usuario    FOREIGN KEY (id_usuarioFK)   REFERENCES usuario   (id_usuario)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB;

-- ---------------------------------------------------------------------
-- 7. FORMATO_MULTIMEDIA (nueva en 3FN): elimina la dependencia formato → tipo_recurso
-- ---------------------------------------------------------------------
CREATE TABLE formato_multimedia (
    codigo_formato  VARCHAR(10)  NOT NULL,
    tipo_recurso    VARCHAR(15)  NOT NULL,
    mime_type       VARCHAR(60)  NOT NULL,
    descripcion     VARCHAR(150) NULL,
    CONSTRAINT pk_formato        PRIMARY KEY (codigo_formato),
    CONSTRAINT ck_formato_codigo CHECK (codigo_formato = UPPER(codigo_formato) AND TRIM(codigo_formato) <> ''),
    CONSTRAINT ck_formato_tipo   CHECK (tipo_recurso IN ('IMAGEN', 'VIDEO', 'AUDIO', 'MODELO_3D', 'AR'))
) ENGINE = InnoDB;

-- ---------------------------------------------------------------------
-- 8. RECURSO_MULTIMEDIA: metadatos del archivo (el archivo vive en disco, no en MySQL)
-- ---------------------------------------------------------------------
CREATE TABLE recurso_multimedia (
    id_recurso         CHAR(36)        NOT NULL DEFAULT (UUID()),
    nombre             VARCHAR(100)    NOT NULL,
    descripcion        TEXT            NULL,
    ruta_archivo       VARCHAR(255)    NOT NULL,
    codigo_formatoFK   VARCHAR(10)     NOT NULL,
    tamano_bytes       BIGINT UNSIGNED NOT NULL,
    duracion_segundos  INT UNSIGNED    NULL,
    fecha_registro     DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    id_obraFK          CHAR(36)        NOT NULL,
    CONSTRAINT pk_recurso           PRIMARY KEY (id_recurso),
    CONSTRAINT uq_recurso_ruta      UNIQUE (ruta_archivo),
    CONSTRAINT ck_recurso_nombre    CHECK (TRIM(nombre) <> ''),
    -- La ruta debe ser RELATIVA a RecursosMultimedia/: sin unidad (C:), sin raíz (/ o \) y sin '..'
    CONSTRAINT ck_recurso_ruta_rel  CHECK (ruta_archivo NOT LIKE '%:%'
                                       AND ruta_archivo NOT LIKE '/%'
                                       AND ruta_archivo NOT LIKE '\\\\%'
                                       AND ruta_archivo NOT LIKE '%..%'),
    CONSTRAINT ck_recurso_tamano    CHECK (tamano_bytes > 0),
    CONSTRAINT ck_recurso_duracion  CHECK (duracion_segundos IS NULL OR duracion_segundos > 0),
    CONSTRAINT fk_recurso_formato   FOREIGN KEY (codigo_formatoFK) REFERENCES formato_multimedia (codigo_formato)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_recurso_obra      FOREIGN KEY (id_obraFK) REFERENCES obra (id_obra)
        ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB;

-- ---------------------------------------------------------------------
-- 9. RUTA: recorrido compuesto por varias obras
-- ---------------------------------------------------------------------
CREATE TABLE ruta (
    id_ruta                CHAR(36)          NOT NULL DEFAULT (UUID()),
    nombre                 VARCHAR(100)      NOT NULL,
    descripcion            TEXT              NOT NULL,
    duracion_estimada_min  SMALLINT UNSIGNED NOT NULL,
    distancia_estimada_km  DECIMAL(5,2)      NOT NULL,
    estado                 VARCHAR(20)       NOT NULL DEFAULT 'ACTIVO',
    CONSTRAINT pk_ruta            PRIMARY KEY (id_ruta),
    CONSTRAINT uq_ruta_nombre     UNIQUE (nombre),
    CONSTRAINT ck_ruta_nombre     CHECK (TRIM(nombre) <> ''),
    CONSTRAINT ck_ruta_duracion   CHECK (duracion_estimada_min > 0),
    CONSTRAINT ck_ruta_distancia  CHECK (distancia_estimada_km > 0),
    CONSTRAINT ck_ruta_estado     CHECK (estado IN ('ACTIVO', 'INACTIVO', 'EN_MANTENIMIENTO'))
) ENGINE = InnoDB;

-- ---------------------------------------------------------------------
-- 10. RUTA_OBRA: entidad asociativa que resuelve la relación M:N Ruta–Obra
-- ---------------------------------------------------------------------
CREATE TABLE ruta_obra (
    id_ruta_obra         CHAR(36)          NOT NULL DEFAULT (UUID()),
    id_rutaFK            CHAR(36)          NOT NULL,
    id_obraFK            CHAR(36)          NOT NULL,
    orden_visita         SMALLINT UNSIGNED NOT NULL,
    tiempo_estimado_min  SMALLINT UNSIGNED NOT NULL DEFAULT 10,
    recomendacion        TEXT              NULL,
    CONSTRAINT pk_ruta_obra         PRIMARY KEY (id_ruta_obra),
    CONSTRAINT uq_ruta_obra_obra    UNIQUE (id_rutaFK, id_obraFK),     -- una obra no se repite en la ruta
    CONSTRAINT uq_ruta_obra_orden   UNIQUE (id_rutaFK, orden_visita),  -- el orden no se repite en la ruta
    CONSTRAINT ck_ruta_obra_orden   CHECK (orden_visita > 0),
    CONSTRAINT ck_ruta_obra_tiempo  CHECK (tiempo_estimado_min > 0),
    CONSTRAINT fk_ruta_obra_ruta    FOREIGN KEY (id_rutaFK) REFERENCES ruta (id_ruta)
        ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_ruta_obra_obra    FOREIGN KEY (id_obraFK) REFERENCES obra (id_obra)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB;


-- =====================================================================
--  Galería de Arte Urbano de Cali
--  Etapa 5 — Script 02: Datos de prueba
--  Requisito mínimo: 5 artistas, 5 categorías, 10 obras, 3 rutas, 15 recursos.
--  Este script carga: 3 usuarios, 7 artistas, 6 categorías, 9 barrios, 10 ubicaciones,
--                     6 formatos, 12 obras, 18 recursos, 3 rutas y 13 paradas de ruta.
--
--  Los UUID son fijos (no generados con UUID()) para que el script sea repetible y
--  las llaves foráneas sean legibles. El prefijo indica la tabla:
--  1=usuario 2=artista 3=categoria 4=ubicacion 5=obra 6=recurso 7=ruta 8=ruta_obra
--  Las personas, artistas y obras son FICTICIOS; los barrios y comunas son de Cali.
-- =====================================================================
SET NAMES utf8mb4;   -- la conexión envía tildes y ñ correctamente
USE galeria_arte_urbano_cali;

-- ---------------------------------------------------------------------
-- USUARIOS  (contraseñas guardadas como hash BCrypt, nunca en texto plano)
--   admin@galeriacali.co      / Admin123*      → ADMINISTRADOR
--   editora@galeriacali.co    / Editor123*     → ADMINISTRADOR
--   visitante@galeriacali.co  / Visitante123*  → VISITANTE
-- ---------------------------------------------------------------------
INSERT INTO usuario (id_usuario, nombre, apellido, correo, password_hash, tipo_usuario, estado) VALUES
('10000000-0000-4000-8000-000000000001', 'Juan Pablo', 'Campo Bertin', 'admin@galeriacali.co',
 '$2a$11$QyjN9.GvekjF8FH13KCmMuOqJJrfJtCobI5u4UuObyIRAK5iXzrkG', 'ADMINISTRADOR', 'ACTIVO'),
('10000000-0000-4000-8000-000000000002', 'Jean Deiby', 'Garcia', 'editora@galeriacali.co',
 '$2a$11$5iHaVPoA0IRhX2XBlcC8Qe7clge6/0C7msAukjWlF2kprbqJIfnLS', 'ADMINISTRADOR', 'ACTIVO'),
('10000000-0000-4000-8000-000000000003', 'Valentina', 'Rojas', 'visitante@galeriacali.co',
 '$2a$11$oRGb0fBOPgghHvqkhfvo.uh.5nVKP03jnFxFejsVED/V1COtDOwTy', 'VISITANTE', 'ACTIVO');

-- ---------------------------------------------------------------------
-- ARTISTAS (7; "Lienzo Errante" no tiene obras → se puede eliminar físicamente)
-- ---------------------------------------------------------------------
INSERT INTO artista (id_artista, nombre_artistico, nombre_real, biografia, pais_origen, red_social, estado) VALUES
('20000000-0000-4000-8000-000000000001', 'Kromo Salsa', 'Andrés Felipe Ortiz',
 'Muralista caleño que mezcla la iconografía de la salsa con el paisaje del río Cali.', 'Colombia', 'https://instagram.com/kromosalsa', 'ACTIVO'),
('20000000-0000-4000-8000-000000000002', 'Colectivo La Ceiba', NULL,
 'Colectivo de jóvenes artistas de ladera que trabaja procesos comunitarios de muralismo.', 'Colombia', '@colectivolaceiba', 'ACTIVO'),
('20000000-0000-4000-8000-000000000003', 'Nébula Andina', 'Laura Ximena Ríos',
 'Artista de lettering y grafiti con influencias de la tipografía popular de los buses.', 'Colombia', 'https://behance.net/nebulaandina', 'ACTIVO'),
('20000000-0000-4000-8000-000000000004', 'Trazo Pacífico', 'Yesid Mosquera',
 'Pintor de Buenaventura radicado en Cali; retrata la cultura afro del Pacífico.', 'Colombia', NULL, 'ACTIVO'),
('20000000-0000-4000-8000-000000000005', 'Mano Zurda', 'Diego Paredes',
 'Stencilista quiteño invitado al festival de arte urbano de Cali.', 'Ecuador', '@manozurda', 'ACTIVO'),
('20000000-0000-4000-8000-000000000006', 'Tinta Ladera', 'Camila Hurtado',
 'Retratista que pinta a los habitantes de los barrios donde interviene.', 'Colombia', '@tintaladera', 'ACTIVO'),
('20000000-0000-4000-8000-000000000007', 'Lienzo Errante', NULL,
 'Artista itinerante registrado para un próximo festival; aún sin obras en Cali.', 'Argentina', NULL, 'ACTIVO');

-- ---------------------------------------------------------------------
-- CATEGORÍAS (6; "Paste-up" está INACTIVA y sin obras)
-- ---------------------------------------------------------------------
INSERT INTO categoria (id_categoria, nombre, descripcion, estado) VALUES
('30000000-0000-4000-8000-000000000001', 'Muralismo',        'Pintura de gran formato sobre muros, generalmente con permiso.', 'ACTIVO'),
('30000000-0000-4000-8000-000000000002', 'Grafiti',          'Intervención con aerosol basada en letras, firmas o personajes.', 'ACTIVO'),
('30000000-0000-4000-8000-000000000003', 'Stencil',          'Técnica de plantilla recortada que permite reproducir la imagen.', 'ACTIVO'),
('30000000-0000-4000-8000-000000000004', 'Retrato',          'Representación de personas reales o simbólicas del territorio.', 'ACTIVO'),
('30000000-0000-4000-8000-000000000005', 'Arte tipográfico', 'Obras centradas en letras, frases y lettering.', 'ACTIVO'),
('30000000-0000-4000-8000-000000000006', 'Paste-up',         'Papel impreso o pintado pegado sobre el muro.', 'INACTIVO');

-- ---------------------------------------------------------------------
-- BARRIOS (comuna real de Cali)
-- ---------------------------------------------------------------------
INSERT INTO barrio (id_barrio, nombre, comuna) VALUES
(1, 'San Antonio', 3),
(2, 'El Peñón', 3),
(3, 'San Pedro', 3),
(4, 'Granada', 2),
(5, 'Siloé', 20),
(6, 'Obrero', 9),
(7, 'Alameda', 9),
(8, 'San Fernando Viejo', 19),
(9, 'Ciudad Jardín', 22);

-- ---------------------------------------------------------------------
-- UBICACIONES (10; "Muro Cañasgordas" aún no tiene obras)
-- ---------------------------------------------------------------------
INSERT INTO ubicacion (id_ubicacion, nombre_lugar, direccion, id_barrioFK, latitud, longitud) VALUES
('40000000-0000-4000-8000-000000000001', 'Bulevar del Río - Muro Oriental', 'Avenida Colombia # 8-20', 3, 3.45310000, -76.53280000),
('40000000-0000-4000-8000-000000000002', 'Escalinata Loma de San Antonio',  'Carrera 10 # 1-50',      1, 3.44860000, -76.54050000),
('40000000-0000-4000-8000-000000000003', 'Calle de la Escopeta',            'Calle 2 # 4-15',         1, 3.44750000, -76.53850000),
('40000000-0000-4000-8000-000000000004', 'Muro Galería El Peñón',           'Calle 3 Oeste # 3-10',   2, 3.45200000, -76.54200000),
('40000000-0000-4000-8000-000000000005', 'Mirador de Siloé',                'Carrera 52 # 1-80 Oeste',5, 3.42450000, -76.56200000),
('40000000-0000-4000-8000-000000000006', 'Fachada Casa Obrera',             'Calle 22 # 10-40',       6, 3.44300000, -76.52500000),
('40000000-0000-4000-8000-000000000007', 'Plaza de Mercado La Alameda',     'Calle 8 # 26-04',        7, 3.43800000, -76.53100000),
('40000000-0000-4000-8000-000000000008', 'Parque del Perro',                'Calle 3A # 34-10',       8, 3.43400000, -76.54400000),
('40000000-0000-4000-8000-000000000009', 'Corredor Avenida Sexta',          'Avenida 6 Norte # 15-30',4, 3.46200000, -76.53100000),
('40000000-0000-4000-8000-000000000010', 'Muro Cañasgordas',                'Calle 18 # 105-30',      9, 3.37000000, -76.53000000);

-- ---------------------------------------------------------------------
-- OBRAS (12)
-- ---------------------------------------------------------------------
INSERT INTO obra (id_obra, nombre, descripcion, fecha_creacion, estado_conservacion,
                  id_artistaFK, id_categoriaFK, id_ubicacionFK, id_usuarioFK) VALUES
('50000000-0000-4000-8000-000000000001', 'Memoria del Río',
 'Mural de 30 metros que narra la historia del río Cali desde los Farallones hasta el Bulevar.', '2022-08-15', 'EXCELENTE',
 '20000000-0000-4000-8000-000000000001', '30000000-0000-4000-8000-000000000001', '40000000-0000-4000-8000-000000000001', '10000000-0000-4000-8000-000000000001'),
('50000000-0000-4000-8000-000000000002', 'Raíces del Pacífico',
 'Homenaje a las cantadoras y a la marimba de chonta del Pacífico colombiano.', '2021-05-20', 'BUENO',
 '20000000-0000-4000-8000-000000000004', '30000000-0000-4000-8000-000000000001', '40000000-0000-4000-8000-000000000001', '10000000-0000-4000-8000-000000000001'),
('50000000-0000-4000-8000-000000000003', 'Guardianes de la Loma',
 'Figuras protectoras pintadas a lo largo de la escalinata de San Antonio.', '2019-11-02', 'BUENO',
 '20000000-0000-4000-8000-000000000002', '30000000-0000-4000-8000-000000000001', '40000000-0000-4000-8000-000000000002', '10000000-0000-4000-8000-000000000001'),
('50000000-0000-4000-8000-000000000004', 'Letras de Barrio',
 'Composición tipográfica con dichos populares caleños.', '2023-03-10', 'EXCELENTE',
 '20000000-0000-4000-8000-000000000003', '30000000-0000-4000-8000-000000000005', '40000000-0000-4000-8000-000000000003', '10000000-0000-4000-8000-000000000002'),
('50000000-0000-4000-8000-000000000005', 'Eco de Tambores',
 'Grafiti de personajes con tambores y trompetas inspirado en la salsa brava.', '2018-07-01', 'DETERIORADO',
 '20000000-0000-4000-8000-000000000001', '30000000-0000-4000-8000-000000000002', '40000000-0000-4000-8000-000000000003', '10000000-0000-4000-8000-000000000002'),
('50000000-0000-4000-8000-000000000006', 'Mirada de Abuela',
 'Retrato hiperrealista de una vecina del barrio El Peñón.', '2020-09-12', 'BUENO',
 '20000000-0000-4000-8000-000000000006', '30000000-0000-4000-8000-000000000004', '40000000-0000-4000-8000-000000000004', '10000000-0000-4000-8000-000000000001'),
('50000000-0000-4000-8000-000000000007', 'Ladera Viva',
 'Mural colectivo que cubre varias fachadas del mirador de Siloé.', '2024-02-25', 'EXCELENTE',
 '20000000-0000-4000-8000-000000000006', '30000000-0000-4000-8000-000000000001', '40000000-0000-4000-8000-000000000005', '10000000-0000-4000-8000-000000000002'),
('50000000-0000-4000-8000-000000000008', 'Niños de Siloé',
 'Retratos de niños del sector pintados durante un taller comunitario.', '2022-10-30', 'BUENO',
 '20000000-0000-4000-8000-000000000002', '30000000-0000-4000-8000-000000000004', '40000000-0000-4000-8000-000000000005', '10000000-0000-4000-8000-000000000001'),
('50000000-0000-4000-8000-000000000009', 'Pasos de Salsa',
 'Serie de stencils con siluetas de bailarines de salsa caleña.', '2021-12-05', 'BUENO',
 '20000000-0000-4000-8000-000000000001', '30000000-0000-4000-8000-000000000003', '40000000-0000-4000-8000-000000000006', '10000000-0000-4000-8000-000000000001'),
('50000000-0000-4000-8000-000000000010', 'Frutas del Mercado',
 'Mural de frutas tropicales en la fachada de la galería de La Alameda.', '2017-04-18', 'DETERIORADO',
 '20000000-0000-4000-8000-000000000005', '30000000-0000-4000-8000-000000000001', '40000000-0000-4000-8000-000000000007', '10000000-0000-4000-8000-000000000002'),
('50000000-0000-4000-8000-000000000011', 'Perro Guardián',
 'Stencil de un perro guardián, guiño al nombre popular del parque.', '2020-01-22', 'BUENO',
 '20000000-0000-4000-8000-000000000005', '30000000-0000-4000-8000-000000000003', '40000000-0000-4000-8000-000000000008', '10000000-0000-4000-8000-000000000001'),
('50000000-0000-4000-8000-000000000012', 'Neón Sexta',
 'Grafiti de letras en colores neón, cubierto en una remodelación (se conserva el registro).', '2016-06-06', 'DESTRUIDO',
 '20000000-0000-4000-8000-000000000003', '30000000-0000-4000-8000-000000000002', '40000000-0000-4000-8000-000000000009', '10000000-0000-4000-8000-000000000002');

-- ---------------------------------------------------------------------
-- FORMATOS MULTIMEDIA PERMITIDOS (nueva tabla 3FN)
-- ---------------------------------------------------------------------
INSERT INTO formato_multimedia (codigo_formato, tipo_recurso, mime_type, descripcion) VALUES
('JPG', 'IMAGEN',    'image/jpeg',       'Fotografía comprimida (.jpg / .jpeg)'),
('PNG', 'IMAGEN',    'image/png',        'Imagen sin pérdida (.png)'),
('MP4', 'VIDEO',     'video/mp4',        'Video H.264 (.mp4)'),
('MP3', 'AUDIO',     'audio/mpeg',       'Audio comprimido (.mp3)'),
('GLB', 'MODELO_3D', 'model/gltf-binary','Modelo 3D glTF binario (.glb)'),
('AR',  'AR',        'inode/directory',  'Carpeta con contenido de realidad aumentada (referencia)');

-- ---------------------------------------------------------------------
-- RECURSOS MULTIMEDIA (18). ruta_archivo es RELATIVA a la carpeta RecursosMultimedia/
-- tamano_bytes corresponde a los archivos de ejemplo incluidos en el proyecto.
-- ---------------------------------------------------------------------
INSERT INTO recurso_multimedia (id_recurso, nombre, descripcion, ruta_archivo, codigo_formatoFK,
                                tamano_bytes, duracion_segundos, id_obraFK) VALUES
('60000000-0000-4000-8000-000000000001', 'Fotografía frontal',      'Toma diurna del mural completo.',          'Imagenes/obra_001_frontal.jpg',     'JPG',  73433, NULL, '50000000-0000-4000-8000-000000000001'),
('60000000-0000-4000-8000-000000000002', 'Video del proceso',       'Timelapse de la pintura del mural.',       'Videos/obra_001_proceso.mp4',       'MP4', 304978,    8, '50000000-0000-4000-8000-000000000001'),
('60000000-0000-4000-8000-000000000003', 'Audioguía',               'Narración sobre la historia del río.',     'Audios/obra_001_audioguia.mp3',     'MP3', 120730,   10, '50000000-0000-4000-8000-000000000001'),
('60000000-0000-4000-8000-000000000004', 'Fotografía frontal',      NULL,                                       'Imagenes/obra_002_frontal.jpg',     'JPG',  77148, NULL, '50000000-0000-4000-8000-000000000002'),
('60000000-0000-4000-8000-000000000005', 'Fotografía frontal',      NULL,                                       'Imagenes/obra_003_frontal.jpg',     'JPG',  89049, NULL, '50000000-0000-4000-8000-000000000003'),
('60000000-0000-4000-8000-000000000006', 'Modelo 3D de la escalinata','Levantamiento 3D simplificado del muro.','Modelos3D/obra_003_escalinata.glb', 'GLB',    884, NULL, '50000000-0000-4000-8000-000000000003'),
('60000000-0000-4000-8000-000000000007', 'Detalle tipográfico',     'Acercamiento a las letras principales.',   'Imagenes/obra_004_detalle.png',     'PNG',  78446, NULL, '50000000-0000-4000-8000-000000000004'),
('60000000-0000-4000-8000-000000000008', 'Fotografía frontal',      'Se aprecia el deterioro por humedad.',     'Imagenes/obra_005_frontal.jpg',     'JPG',  79167, NULL, '50000000-0000-4000-8000-000000000005'),
('60000000-0000-4000-8000-000000000009', 'Fotografía frontal',      NULL,                                       'Imagenes/obra_006_frontal.jpg',     'JPG',  81695, NULL, '50000000-0000-4000-8000-000000000006'),
('60000000-0000-4000-8000-000000000010', 'Panorámica',              'Vista desde el mirador.',                  'Imagenes/obra_007_panoramica.jpg',  'JPG',  73024, NULL, '50000000-0000-4000-8000-000000000007'),
('60000000-0000-4000-8000-000000000011', 'Toma aérea con dron',     NULL,                                       'Videos/obra_007_dron.mp4',          'MP4', 243005,    6, '50000000-0000-4000-8000-000000000007'),
('60000000-0000-4000-8000-000000000012', 'Fotografía frontal',      NULL,                                       'Imagenes/obra_008_frontal.jpg',     'JPG',  71237, NULL, '50000000-0000-4000-8000-000000000008'),
('60000000-0000-4000-8000-000000000013', 'Testimonio comunitario',  'Entrevista a una líder del barrio.',       'Audios/obra_008_testimonio.mp3',    'MP3', 144867,   12, '50000000-0000-4000-8000-000000000008'),
('60000000-0000-4000-8000-000000000014', 'Fotografía frontal',      NULL,                                       'Imagenes/obra_009_frontal.png',     'PNG',  84585, NULL, '50000000-0000-4000-8000-000000000009'),
('60000000-0000-4000-8000-000000000015', 'Fotografía frontal',      NULL,                                       'Imagenes/obra_010_frontal.jpg',     'JPG',  71437, NULL, '50000000-0000-4000-8000-000000000010'),
('60000000-0000-4000-8000-000000000016', 'Fotografía frontal',      NULL,                                       'Imagenes/obra_011_frontal.jpg',     'JPG',  72916, NULL, '50000000-0000-4000-8000-000000000011'),
('60000000-0000-4000-8000-000000000017', 'Fotografía de archivo',   'Registro previo a la remodelación.',       'Imagenes/obra_012_archivo.jpg',     'JPG',  75365, NULL, '50000000-0000-4000-8000-000000000012'),
('60000000-0000-4000-8000-000000000018', 'Contenido de realidad aumentada', 'Marcador y modelo para una experiencia AR.', 'AR/obra_001/', 'AR', 82983, NULL, '50000000-0000-4000-8000-000000000001');

-- ---------------------------------------------------------------------
-- RUTAS (3)
-- ---------------------------------------------------------------------
INSERT INTO ruta (id_ruta, nombre, descripcion, duracion_estimada_min, distancia_estimada_km, estado) VALUES
('70000000-0000-4000-8000-000000000001', 'Ruta Centro Histórico y Río',
 'Recorrido a pie por el Bulevar del Río, San Antonio y El Peñón. Ideal para turistas.', 120, 2.80, 'ACTIVO'),
('70000000-0000-4000-8000-000000000002', 'Ruta Ladera de Siloé',
 'Recorrido guiado por la comunidad hasta el mirador de Siloé.', 90, 1.60, 'ACTIVO'),
('70000000-0000-4000-8000-000000000003', 'Ruta Salsa y Sabor',
 'Obras inspiradas en la salsa y la cultura popular entre el Obrero, La Alameda y San Fernando.', 150, 4.50, 'EN_MANTENIMIENTO');

-- ---------------------------------------------------------------------
-- RUTA_OBRA (13 paradas). "Memoria del Río" y "Eco de Tambores" están en 2 rutas (M:N)
-- ---------------------------------------------------------------------
INSERT INTO ruta_obra (id_ruta_obra, id_rutaFK, id_obraFK, orden_visita, tiempo_estimado_min, recomendacion) VALUES
-- Ruta Centro Histórico y Río
('80000000-0000-4000-8000-000000000001', '70000000-0000-4000-8000-000000000001', '50000000-0000-4000-8000-000000000001', 1, 15, 'Punto de partida. Mejor luz en la mañana.'),
('80000000-0000-4000-8000-000000000002', '70000000-0000-4000-8000-000000000001', '50000000-0000-4000-8000-000000000002', 2, 10, NULL),
('80000000-0000-4000-8000-000000000003', '70000000-0000-4000-8000-000000000001', '50000000-0000-4000-8000-000000000003', 3, 15, 'Subida exigente; llevar agua.'),
('80000000-0000-4000-8000-000000000004', '70000000-0000-4000-8000-000000000001', '50000000-0000-4000-8000-000000000004', 4, 10, NULL),
('80000000-0000-4000-8000-000000000005', '70000000-0000-4000-8000-000000000001', '50000000-0000-4000-8000-000000000005', 5, 10, 'Obra deteriorada: observar desde la acera.'),
('80000000-0000-4000-8000-000000000006', '70000000-0000-4000-8000-000000000001', '50000000-0000-4000-8000-000000000006', 6, 10, 'Cierre con vista al atardecer.'),
-- Ruta Ladera de Siloé
('80000000-0000-4000-8000-000000000007', '70000000-0000-4000-8000-000000000002', '50000000-0000-4000-8000-000000000008', 1, 20, 'Ir con guía local de la comunidad.'),
('80000000-0000-4000-8000-000000000008', '70000000-0000-4000-8000-000000000002', '50000000-0000-4000-8000-000000000007', 2, 25, 'Panorámica de la ciudad desde el mirador.'),
-- Ruta Salsa y Sabor
('80000000-0000-4000-8000-000000000009', '70000000-0000-4000-8000-000000000003', '50000000-0000-4000-8000-000000000009', 1, 10, NULL),
('80000000-0000-4000-8000-000000000010', '70000000-0000-4000-8000-000000000003', '50000000-0000-4000-8000-000000000010', 2, 20, 'Aprovechar para probar fruta en la galería.'),
('80000000-0000-4000-8000-000000000011', '70000000-0000-4000-8000-000000000003', '50000000-0000-4000-8000-000000000011', 3, 10, NULL),
('80000000-0000-4000-8000-000000000012', '70000000-0000-4000-8000-000000000003', '50000000-0000-4000-8000-000000000005', 4, 10, NULL),
('80000000-0000-4000-8000-000000000013', '70000000-0000-4000-8000-000000000003', '50000000-0000-4000-8000-000000000001', 5, 15, 'Final frente al río.');


-- =====================================================================
--  Galería de Arte Urbano de Cali
--  Etapa 5 — Script 03: Vistas, procedimientos almacenados e índices
-- =====================================================================
SET NAMES utf8mb4;
USE galeria_arte_urbano_cali;

-- =====================================================================
-- VISTAS
-- Una vista es una consulta guardada con nombre. La aplicación C# consulta
-- la vista como si fuera una tabla y no repite los JOIN en cada formulario.
-- =====================================================================

-- Vista 1: ficha completa de cada obra (consulta "Consulta de obra" del enunciado)
CREATE OR REPLACE VIEW vw_obra_detalle AS
SELECT  o.id_obra,
        o.nombre                  AS obra,
        o.descripcion,
        o.fecha_creacion,
        o.estado_conservacion,
        o.fecha_registro,
        a.id_artista,
        a.nombre_artistico        AS artista,
        c.id_categoria,
        c.nombre                  AS categoria,
        u.id_ubicacion,
        u.nombre_lugar            AS lugar,
        u.direccion,
        b.id_barrio,
        b.nombre                  AS barrio,
        b.comuna,
        u.latitud,
        u.longitud,
        o.id_usuarioFK            AS id_usuario,
        CONCAT(us.nombre, ' ', us.apellido) AS registrado_por,
        (SELECT COUNT(*) FROM recurso_multimedia r WHERE r.id_obraFK = o.id_obra) AS total_recursos
FROM    obra o
        INNER JOIN artista   a  ON a.id_artista   = o.id_artistaFK
        INNER JOIN categoria c  ON c.id_categoria = o.id_categoriaFK
        INNER JOIN ubicacion u  ON u.id_ubicacion = o.id_ubicacionFK
        INNER JOIN barrio    b  ON b.id_barrio    = u.id_barrioFK
        INNER JOIN usuario   us ON us.id_usuario  = o.id_usuarioFK;

-- Vista 2: recorrido de cada ruta en orden (consulta "Consulta de ruta")
CREATE OR REPLACE VIEW vw_ruta_detalle AS
SELECT  r.id_ruta,
        r.nombre                AS ruta,
        r.estado                AS estado_ruta,
        r.duracion_estimada_min,
        r.distancia_estimada_km,
        ro.id_ruta_obra,
        ro.orden_visita,
        o.id_obra,
        o.nombre                AS obra,
        a.nombre_artistico      AS artista,
        u.nombre_lugar          AS lugar,
        u.direccion,
        b.nombre                AS barrio,
        b.comuna,
        u.latitud,
        u.longitud,
        ro.tiempo_estimado_min,
        ro.recomendacion
FROM    ruta r
        INNER JOIN ruta_obra ro ON ro.id_rutaFK   = r.id_ruta
        INNER JOIN obra      o  ON o.id_obra      = ro.id_obraFK
        INNER JOIN artista   a  ON a.id_artista   = o.id_artistaFK
        INNER JOIN ubicacion u  ON u.id_ubicacion = o.id_ubicacionFK
        INNER JOIN barrio    b  ON b.id_barrio    = u.id_barrioFK;

-- Vista 3: recursos con su tipo (sale de FORMATO_MULTIMEDIA gracias a la 3FN)
CREATE OR REPLACE VIEW vw_recurso_detalle AS
SELECT  rm.id_recurso,
        rm.nombre,
        rm.descripcion,
        rm.ruta_archivo,
        f.codigo_formato        AS formato,
        f.tipo_recurso,
        f.mime_type,
        rm.tamano_bytes,
        rm.duracion_segundos,
        rm.fecha_registro,
        o.id_obra,
        o.nombre                AS obra
FROM    recurso_multimedia rm
        INNER JOIN formato_multimedia f ON f.codigo_formato = rm.codigo_formatoFK
        INNER JOIN obra               o ON o.id_obra        = rm.id_obraFK;


-- =====================================================================
-- PROCEDIMIENTOS ALMACENADOS
-- Encapsulan operaciones de varias sentencias que deben ejecutarse juntas
-- (transacción): si algo falla, no queda la ruta con el orden "roto".
-- =====================================================================
DROP PROCEDURE IF EXISTS sp_agregar_obra_a_ruta;
DROP PROCEDURE IF EXISTS sp_retirar_obra_de_ruta;
DROP PROCEDURE IF EXISTS sp_mover_obra_en_ruta;
DROP PROCEDURE IF EXISTS sp_eliminar_o_desactivar_artista;

DELIMITER //

-- ---------------------------------------------------------------------
-- SP 1: agrega una obra al FINAL de una ruta calculando el siguiente orden.
-- Evita que el usuario tenga que adivinar el número de orden (RN-05).
-- ---------------------------------------------------------------------
CREATE PROCEDURE sp_agregar_obra_a_ruta(
    IN p_id_ruta        CHAR(36),
    IN p_id_obra        CHAR(36),
    IN p_tiempo_min     SMALLINT UNSIGNED,
    IN p_recomendacion  TEXT
)
BEGIN
    DECLARE v_siguiente SMALLINT UNSIGNED;
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;

    IF NOT EXISTS (SELECT 1 FROM ruta WHERE id_ruta = p_id_ruta) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'La ruta no existe.';
    END IF;
    IF NOT EXISTS (SELECT 1 FROM obra WHERE id_obra = p_id_obra) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'La obra no existe.';
    END IF;
    IF EXISTS (SELECT 1 FROM ruta_obra WHERE id_rutaFK = p_id_ruta AND id_obraFK = p_id_obra) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'La obra ya forma parte de esta ruta.';
    END IF;

    START TRANSACTION;
        -- FOR UPDATE bloquea las filas de la ruta: dos administradores no obtienen el mismo número
        SELECT COALESCE(MAX(orden_visita), 0) + 1 INTO v_siguiente
        FROM ruta_obra WHERE id_rutaFK = p_id_ruta FOR UPDATE;

        INSERT INTO ruta_obra (id_ruta_obra, id_rutaFK, id_obraFK, orden_visita, tiempo_estimado_min, recomendacion)
        VALUES (UUID(), p_id_ruta, p_id_obra, v_siguiente, COALESCE(p_tiempo_min, 10), p_recomendacion);
    COMMIT;
END //

-- ---------------------------------------------------------------------
-- SP 2: retira una obra de una ruta y "compacta" el orden (1,2,3,... sin huecos).
-- El UPDATE ... ORDER BY ASC evita choques con UNIQUE(id_rutaFK, orden_visita).
-- ---------------------------------------------------------------------
CREATE PROCEDURE sp_retirar_obra_de_ruta(
    IN p_id_ruta  CHAR(36),
    IN p_id_obra  CHAR(36)
)
BEGIN
    DECLARE v_orden SMALLINT UNSIGNED DEFAULT NULL;
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;

    SELECT orden_visita INTO v_orden
    FROM ruta_obra WHERE id_rutaFK = p_id_ruta AND id_obraFK = p_id_obra;

    IF v_orden IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'La obra no pertenece a esta ruta.';
    END IF;

    START TRANSACTION;
        DELETE FROM ruta_obra WHERE id_rutaFK = p_id_ruta AND id_obraFK = p_id_obra;

        UPDATE ruta_obra
           SET orden_visita = orden_visita - 1
         WHERE id_rutaFK = p_id_ruta AND orden_visita > v_orden
         ORDER BY orden_visita ASC;
    COMMIT;
END //

-- ---------------------------------------------------------------------
-- SP 3: cambia la posición de una obra dentro de la ruta y desplaza las demás.
-- Ej.: mover la parada 5 a la posición 2 → las paradas 2,3,4 pasan a 3,4,5.
-- ---------------------------------------------------------------------
CREATE PROCEDURE sp_mover_obra_en_ruta(
    IN p_id_ruta       CHAR(36),
    IN p_id_obra       CHAR(36),
    IN p_nuevo_orden   SMALLINT UNSIGNED
)
BEGIN
    DECLARE v_actual SMALLINT UNSIGNED DEFAULT NULL;
    DECLARE v_total  SMALLINT UNSIGNED;
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;

    SELECT orden_visita INTO v_actual
    FROM ruta_obra WHERE id_rutaFK = p_id_ruta AND id_obraFK = p_id_obra;
    SELECT COUNT(*) INTO v_total FROM ruta_obra WHERE id_rutaFK = p_id_ruta;

    IF v_actual IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'La obra no pertenece a esta ruta.';
    END IF;
    IF p_nuevo_orden < 1 OR p_nuevo_orden > v_total THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'El nuevo orden está fuera del rango de la ruta.';
    END IF;

    IF p_nuevo_orden <> v_actual THEN
        START TRANSACTION;
            -- 1) se "estaciona" la obra en una posición temporal libre
            UPDATE ruta_obra SET orden_visita = 60000
             WHERE id_rutaFK = p_id_ruta AND id_obraFK = p_id_obra;

            -- 2) se desplazan las obras intermedias (en el orden que evita duplicados)
            IF p_nuevo_orden < v_actual THEN
                UPDATE ruta_obra SET orden_visita = orden_visita + 1
                 WHERE id_rutaFK = p_id_ruta AND orden_visita >= p_nuevo_orden AND orden_visita < v_actual
                 ORDER BY orden_visita DESC;
            ELSE
                UPDATE ruta_obra SET orden_visita = orden_visita - 1
                 WHERE id_rutaFK = p_id_ruta AND orden_visita > v_actual AND orden_visita <= p_nuevo_orden
                 ORDER BY orden_visita ASC;
            END IF;

            -- 3) se ubica la obra en su nueva posición
            UPDATE ruta_obra SET orden_visita = p_nuevo_orden
             WHERE id_rutaFK = p_id_ruta AND id_obraFK = p_id_obra;
        COMMIT;
    END IF;
END //

-- ---------------------------------------------------------------------
-- SP 4: RN-02. Si el artista tiene obras → se DESACTIVA; si no tiene → se ELIMINA.
-- Devuelve en p_resultado lo que hizo, para que la aplicación lo informe.
-- ---------------------------------------------------------------------
CREATE PROCEDURE sp_eliminar_o_desactivar_artista(
    IN  p_id_artista  CHAR(36),
    OUT p_resultado   VARCHAR(15)
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM artista WHERE id_artista = p_id_artista) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'El artista no existe.';
    END IF;

    IF EXISTS (SELECT 1 FROM obra WHERE id_artistaFK = p_id_artista) THEN
        UPDATE artista SET estado = 'INACTIVO' WHERE id_artista = p_id_artista;
        SET p_resultado = 'DESACTIVADO';
    ELSE
        DELETE FROM artista WHERE id_artista = p_id_artista;
        SET p_resultado = 'ELIMINADO';
    END IF;
END //

DELIMITER ;


-- =====================================================================
-- ÍNDICES
-- MySQL ya crea índices automáticamente para PRIMARY KEY, UNIQUE y FOREIGN KEY.
-- Estos dos índices adicionales se justifican por consultas concretas de la app.
-- =====================================================================

-- Índice 1: búsqueda de obras por nombre (módulo Consultas: "buscar obra que empiece por...")
--   Consulta que mejora:  SELECT ... FROM obra WHERE nombre LIKE 'Mem%' ORDER BY nombre;
--   Sin índice: recorre toda la tabla (type = ALL). Con índice: rango sobre el índice (type = range)
--   y además entrega las filas ya ordenadas (sin "Using filesort").
--   Nota: el UNIQUE (id_ubicacionFK, nombre) NO sirve para esto porque su primera columna es la ubicación.
CREATE INDEX idx_obra_nombre ON obra (nombre);

-- Índice 2: estadísticas y filtros por comuna (consulta "obras por comuna")
--   Consulta que mejora:  ... FROM barrio b ... WHERE b.comuna = 3   /   GROUP BY b.comuna
--   Permite a MySQL ubicar los barrios de una comuna sin leer todos los barrios y
--   agrupar recorriendo el índice ya ordenado por comuna.
CREATE INDEX idx_barrio_comuna ON barrio (comuna);


-- =====================================================================
--  Galería de Arte Urbano de Cali
--  Etapa 5 — Script 04: Operaciones y consultas SQL
--  Contiene: INSERT / SELECT / UPDATE / DELETE, JOIN entre varias tablas,
--            agregaciones (GROUP BY, COUNT, SUM, HAVING), uso de vistas,
--            llamadas a procedimientos y verificación del índice (EXPLAIN).
--  Se puede ejecutar completo: al final la base queda igual que después del script 02.
-- =====================================================================
SET NAMES utf8mb4;
USE galeria_arte_urbano_cali;

-- =====================================================================
-- A. CONSULTAS REQUERIDAS POR EL ENUNCIADO (sección 12) — JOIN entre varias tablas
-- =====================================================================

-- A1. Consulta de obra: nombre, artista, categoría, ubicación y recursos multimedia
SELECT  o.nombre               AS obra,
        a.nombre_artistico     AS artista,
        c.nombre               AS categoria,
        CONCAT(u.nombre_lugar, ' - ', b.nombre, ' (comuna ', b.comuna, ')') AS ubicacion,
        rm.nombre              AS recurso,
        f.tipo_recurso,
        rm.ruta_archivo
FROM    obra o
        JOIN artista   a ON a.id_artista   = o.id_artistaFK
        JOIN categoria c ON c.id_categoria = o.id_categoriaFK
        JOIN ubicacion u ON u.id_ubicacion = o.id_ubicacionFK
        JOIN barrio    b ON b.id_barrio    = u.id_barrioFK
        LEFT JOIN recurso_multimedia rm ON rm.id_obraFK = o.id_obra          -- LEFT: muestra la obra aunque no tenga recursos
        LEFT JOIN formato_multimedia f  ON f.codigo_formato = rm.codigo_formatoFK
WHERE   o.nombre = 'Memoria del Río';

-- A2. Obras por artista
SELECT obra, categoria, lugar, barrio, estado_conservacion
FROM   vw_obra_detalle
WHERE  artista = 'Kromo Salsa'
ORDER  BY obra;

-- A3. Obras por categoría
SELECT obra, artista, lugar, barrio
FROM   vw_obra_detalle
WHERE  categoria = 'Muralismo'
ORDER  BY obra;

-- A4. Obras por ubicación: por barrio y por comuna
SELECT obra, artista, lugar FROM vw_obra_detalle WHERE barrio = 'San Antonio';
SELECT obra, artista, barrio, lugar FROM vw_obra_detalle WHERE comuna = 3 ORDER BY barrio, obra;

-- A5. Consulta de ruta: obras que la componen, orden y ubicación de cada una
SELECT orden_visita, obra, artista, lugar, barrio, tiempo_estimado_min, recomendacion
FROM   vw_ruta_detalle
WHERE  ruta = 'Ruta Centro Histórico y Río'
ORDER  BY orden_visita;

-- A6. Todos los recursos multimedia de una obra
SELECT nombre, tipo_recurso, formato, ruta_archivo,
       ROUND(tamano_bytes / 1024, 1) AS tamano_kb, duracion_segundos
FROM   vw_recurso_detalle
WHERE  obra = 'Memoria del Río'
ORDER  BY tipo_recurso, nombre;


-- =====================================================================
-- B. CONSULTAS CON AGREGACIÓN (mínimo 3: GROUP BY, COUNT, HAVING)
-- =====================================================================

-- B1. Número de obras por comuna
SELECT  b.comuna,
        COUNT(o.id_obra)                       AS total_obras,
        GROUP_CONCAT(DISTINCT b.nombre ORDER BY b.nombre SEPARATOR ', ') AS barrios
FROM    barrio b
        JOIN ubicacion u ON u.id_barrioFK    = b.id_barrio
        JOIN obra      o ON o.id_ubicacionFK = u.id_ubicacion
GROUP BY b.comuna
ORDER BY total_obras DESC, b.comuna;

-- B2. Artistas con más de N obras (HAVING). Aquí N = 1 → artistas con 2 o más obras
SELECT  a.nombre_artistico,
        COUNT(o.id_obra) AS total_obras
FROM    artista a
        JOIN obra o ON o.id_artistaFK = a.id_artista
GROUP BY a.id_artista, a.nombre_artistico
HAVING  COUNT(o.id_obra) > 1
ORDER BY total_obras DESC, a.nombre_artistico;

-- B3. Obras por categoría, incluyendo las categorías sin obras (LEFT JOIN + COUNT de la columna)
SELECT  c.nombre AS categoria, c.estado,
        COUNT(o.id_obra) AS total_obras
FROM    categoria c
        LEFT JOIN obra o ON o.id_categoriaFK = c.id_categoria
GROUP BY c.id_categoria, c.nombre, c.estado
ORDER BY total_obras DESC, categoria;

-- B4. Recursos por tipo: cantidad y espacio ocupado en disco (MB)
SELECT  f.tipo_recurso,
        COUNT(*)                                  AS cantidad,
        ROUND(SUM(rm.tamano_bytes) / 1048576, 2)  AS total_mb
FROM    recurso_multimedia rm
        JOIN formato_multimedia f ON f.codigo_formato = rm.codigo_formatoFK
GROUP BY f.tipo_recurso
ORDER BY cantidad DESC;

-- B5. Rutas con al menos 3 obras y el tiempo total de permanencia frente a las obras
SELECT  r.nombre, r.estado,
        COUNT(ro.id_obraFK)          AS obras,
        SUM(ro.tiempo_estimado_min)  AS minutos_frente_a_obras,
        r.duracion_estimada_min      AS duracion_total_estimada
FROM    ruta r
        JOIN ruta_obra ro ON ro.id_rutaFK = r.id_ruta
GROUP BY r.id_ruta, r.nombre, r.estado, r.duracion_estimada_min
HAVING  COUNT(ro.id_obraFK) >= 3;

-- B6. Estado de conservación del patrimonio (útil para priorizar restauraciones)
SELECT estado_conservacion, COUNT(*) AS obras
FROM   obra
GROUP BY estado_conservacion
ORDER BY FIELD(estado_conservacion, 'EXCELENTE', 'BUENO', 'DETERIORADO', 'DESTRUIDO');

-- B7. Obras que NO tienen ninguna imagen (control de calidad del catálogo)
SELECT o.nombre
FROM   obra o
WHERE  NOT EXISTS (SELECT 1
                   FROM recurso_multimedia rm
                   JOIN formato_multimedia f ON f.codigo_formato = rm.codigo_formatoFK
                   WHERE rm.id_obraFK = o.id_obra AND f.tipo_recurso = 'IMAGEN');


-- =====================================================================
-- C. OPERACIONES INSERT / UPDATE / DELETE
-- Se ejecutan dentro de una transacción y se deshacen con ROLLBACK para que
-- el script sea repetible. (Cambien ROLLBACK por COMMIT para dejarlas aplicadas.)
-- =====================================================================
START TRANSACTION;

-- C1. INSERT: nueva categoría y nueva obra (el UUID lo genera el DEFAULT (UUID()))
INSERT INTO categoria (nombre, descripcion) VALUES ('Instalación urbana', 'Obras tridimensionales en el espacio público.');

INSERT INTO obra (nombre, descripcion, fecha_creacion, estado_conservacion,
                  id_artistaFK, id_categoriaFK, id_ubicacionFK, id_usuarioFK)
SELECT 'Raíz de Cañasgordas', 'Instalación con materiales reciclados.', '2025-05-01', 'EXCELENTE',
       a.id_artista, c.id_categoria, u.id_ubicacion, us.id_usuario
FROM   artista a, categoria c, ubicacion u, usuario us
WHERE  a.nombre_artistico = 'Tinta Ladera' AND c.nombre = 'Instalación urbana'
  AND  u.nombre_lugar = 'Muro Cañasgordas' AND us.correo = 'admin@galeriacali.co';

SELECT obra, artista, categoria, barrio FROM vw_obra_detalle WHERE obra = 'Raíz de Cañasgordas';

-- C2. UPDATE: la obra "Frutas del Mercado" fue restaurada
UPDATE obra SET estado_conservacion = 'EXCELENTE' WHERE nombre = 'Frutas del Mercado';
-- UPDATE: desactivar una categoría (no se borra porque podría tener obras)
UPDATE categoria SET estado = 'INACTIVO' WHERE nombre = 'Stencil';

SELECT nombre, estado_conservacion FROM obra WHERE nombre = 'Frutas del Mercado';

-- C3. DELETE: eliminar un recurso multimedia
DELETE FROM recurso_multimedia WHERE ruta_archivo = 'Imagenes/obra_012_archivo.jpg';
-- DELETE con CASCADE: al borrar la obra nueva se borrarían también sus recursos
DELETE FROM obra WHERE nombre = 'Raíz de Cañasgordas';

SELECT COUNT(*) AS recursos_restantes FROM recurso_multimedia;

ROLLBACK;   -- deshace C1, C2 y C3

-- C4. DELETE que la base de datos RECHAZA (integridad referencial, RN-02).
--     Descomenten para verlo: Error 1451 "Cannot delete or update a parent row"
-- DELETE FROM artista WHERE nombre_artistico = 'Kromo Salsa';


-- =====================================================================
-- D. PROCEDIMIENTOS ALMACENADOS
-- (Los procedimientos usan su propia transacción, por eso se demuestran con
--  operaciones que se revierten manualmente en lugar de ROLLBACK.)
-- =====================================================================
SET @ruta_siloe = (SELECT id_ruta FROM ruta WHERE nombre = 'Ruta Ladera de Siloé');
SET @obra_abuela = (SELECT id_obra FROM obra WHERE nombre = 'Mirada de Abuela');

-- D1. Agregar una obra al final de la ruta (el procedimiento calcula el orden = 3)
CALL sp_agregar_obra_a_ruta(@ruta_siloe, @obra_abuela, 15, 'Parada opcional de regreso.');
SELECT orden_visita, obra FROM vw_ruta_detalle WHERE id_ruta = @ruta_siloe ORDER BY orden_visita;

-- D2. Moverla a la primera posición (las demás se desplazan)
CALL sp_mover_obra_en_ruta(@ruta_siloe, @obra_abuela, 1);
SELECT orden_visita, obra FROM vw_ruta_detalle WHERE id_ruta = @ruta_siloe ORDER BY orden_visita;

-- D3. Retirarla (el orden se compacta de nuevo a 1, 2)
CALL sp_retirar_obra_de_ruta(@ruta_siloe, @obra_abuela);
SELECT orden_visita, obra FROM vw_ruta_detalle WHERE id_ruta = @ruta_siloe ORDER BY orden_visita;

-- D4. Eliminar o desactivar artista
--   "Lienzo Errante" no tiene obras → se ELIMINA. Luego se vuelve a insertar para dejar los datos igual.
CALL sp_eliminar_o_desactivar_artista('20000000-0000-4000-8000-000000000007', @resultado);
SELECT @resultado AS resultado_lienzo_errante;   -- ELIMINADO
INSERT INTO artista (id_artista, nombre_artistico, biografia, pais_origen, estado)
VALUES ('20000000-0000-4000-8000-000000000007', 'Lienzo Errante',
        'Artista itinerante registrado para un próximo festival; aún sin obras en Cali.', 'Argentina', 'ACTIVO');
--   Con un artista que SÍ tiene obras, el procedimiento solo lo desactiva:
--   CALL sp_eliminar_o_desactivar_artista('20000000-0000-4000-8000-000000000001', @resultado);  -- DESACTIVADO


-- =====================================================================
-- E. VERIFICACIÓN DE LOS ÍNDICES CON EXPLAIN
-- =====================================================================
-- E1. Búsqueda por nombre: con idx_obra_nombre → type = range, key = idx_obra_nombre, sin filesort
EXPLAIN SELECT id_obra, nombre FROM obra WHERE nombre LIKE 'Mem%' ORDER BY nombre;

-- E2. Obras de una comuna: con idx_barrio_comuna → la tabla barrio se accede con type = ref
EXPLAIN SELECT b.comuna, COUNT(o.id_obra)
        FROM barrio b
        JOIN ubicacion u ON u.id_barrioFK = b.id_barrio
        JOIN obra o      ON o.id_ubicacionFK = u.id_ubicacion
        WHERE b.comuna = 3
        GROUP BY b.comuna;
