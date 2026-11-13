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
