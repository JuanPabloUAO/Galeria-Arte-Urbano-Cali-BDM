-- =============================================================================
-- PROYECTO DE CURSO: GALERÍA DE ARTE URBANO DE CALI (ESTRUCTURA DDL EN 3FN)
-- Asignatura: Bases de Datos Multimedia (BDM) - Grupo 51
-- Universidad Autónoma de Occidente (UAO) - Facultad de Ingeniería
-- Integrantes: Juan Pablo Campo Bertin (2235544) · Jean Deiby García
-- Docente: David Alejandro Castro Benavides
-- Motor: MySQL 8.0+
-- =============================================================================

CREATE DATABASE IF NOT EXISTS `galeria_arte_urbano_db`
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE `galeria_arte_urbano_db`;

-- Eliminación previa en orden inverso de dependencias
DROP VIEW IF EXISTS `vw_estadisticas_culturales_comuna`;
DROP VIEW IF EXISTS `vw_obras_resumen_multimedia`;
DROP VIEW IF EXISTS `vw_rutas_itinerario_detallado`;
DROP VIEW IF EXISTS `vw_catalogo_obras_completo`;

DROP PROCEDURE IF EXISTS `sp_asignar_obra_a_ruta`;
DROP PROCEDURE IF EXISTS `sp_registrar_obra_con_recurso_principal`;
DROP PROCEDURE IF EXISTS `sp_asociar_recurso_multimedia`;
DROP PROCEDURE IF EXISTS `sp_registrar_obra`;

-- Limpieza tanto de nombres en singular (oficiales MER/MRD) como plurales previos
DROP TABLE IF EXISTS `recurso_multimedia`;
DROP TABLE IF EXISTS `recursos_multimedia`;
DROP TABLE IF EXISTS `ruta_obra`;
DROP TABLE IF EXISTS `rutas_obras`;
DROP TABLE IF EXISTS `ruta`;
DROP TABLE IF EXISTS `rutas`;
DROP TABLE IF EXISTS `obra`;
DROP TABLE IF EXISTS `obras`;
DROP TABLE IF EXISTS `artista`;
DROP TABLE IF EXISTS `artistas`;
DROP TABLE IF EXISTS `usuario`;
DROP TABLE IF EXISTS `usuarios`;
DROP TABLE IF EXISTS `formato_recurso`;
DROP TABLE IF EXISTS `formatos_recurso`;
DROP TABLE IF EXISTS `tipo_recurso`;
DROP TABLE IF EXISTS `tipos_recurso`;
DROP TABLE IF EXISTS `categoria`;
DROP TABLE IF EXISTS `categorias`;
DROP TABLE IF EXISTS `ubicacion`;
DROP TABLE IF EXISTS `ubicaciones`;
DROP TABLE IF EXISTS `barrio`;
DROP TABLE IF EXISTS `barrios`;
DROP TABLE IF EXISTS `comuna`;
DROP TABLE IF EXISTS `comunas`;
DROP TABLE IF EXISTS `tipo_usuario`;
DROP TABLE IF EXISTS `tipos_usuario`;
DROP TABLE IF EXISTS `pais`;
DROP TABLE IF EXISTS `paises`;

-- =============================================================================
-- 1. CREACIÓN DE LAS 14 TABLAS EN 3FN (NOMBRES EN SINGULAR IGUALES AL MER Y MRD)
-- =============================================================================

-- 1. PAIS
CREATE TABLE `pais` (
    `idPais`        CHAR(36)        NOT NULL DEFAULT (UUID()),
    `nombrePais`    VARCHAR(60)     NOT NULL,

    PRIMARY KEY (`idPais`),
    UNIQUE KEY `uk_pais_nombre` (`nombrePais`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 2. TIPO_USUARIO
CREATE TABLE `tipo_usuario` (
    `idTipoUsuario`     CHAR(36)        NOT NULL DEFAULT (UUID()),
    `nombreTipoUsuario` VARCHAR(45)     NOT NULL,

    PRIMARY KEY (`idTipoUsuario`),
    UNIQUE KEY `uk_tipo_usuario_nombre` (`nombreTipoUsuario`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 3. COMUNA
CREATE TABLE `comuna` (
    `idComuna`      CHAR(36)        NOT NULL DEFAULT (UUID()),
    `numeroComuna`  INT             NOT NULL,

    PRIMARY KEY (`idComuna`),
    UNIQUE KEY `uk_comuna_numero` (`numeroComuna`),
    CONSTRAINT `chk_comuna_rango` CHECK (`numeroComuna` BETWEEN 1 AND 22)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 4. BARRIO
CREATE TABLE `barrio` (
    `idBarrio`      CHAR(36)        NOT NULL DEFAULT (UUID()),
    `nombreBarrio`  VARCHAR(80)     NOT NULL,
    `idComunaFK`    CHAR(36)        NOT NULL,

    PRIMARY KEY (`idBarrio`),
    UNIQUE KEY `uk_barrio_nombre_comuna` (`nombreBarrio`, `idComunaFK`),
    KEY `fk_barrio_comuna_idx` (`idComunaFK`),
    CONSTRAINT `fk_barrio_comuna`
        FOREIGN KEY (`idComunaFK`) REFERENCES `comuna` (`idComuna`)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 5. UBICACION
CREATE TABLE `ubicacion` (
    `idUbicacion`   CHAR(36)        NOT NULL DEFAULT (UUID()),
    `nombreLugar`   VARCHAR(100)    NOT NULL,
    `direccion`     VARCHAR(120)    NOT NULL,
    `latitud`       DECIMAL(10, 8)  NOT NULL,
    `longitud`      DECIMAL(11, 8)  NOT NULL,
    `idBarrioFK`    CHAR(36)        NOT NULL,

    PRIMARY KEY (`idUbicacion`),
    KEY `fk_ubicacion_barrio_idx` (`idBarrioFK`),
    CONSTRAINT `chk_ubicacion_latitud` CHECK (`latitud` BETWEEN 3.25000000 AND 3.55000000),
    CONSTRAINT `chk_ubicacion_longitud` CHECK (`longitud` BETWEEN -76.60000000 AND -76.45000000),
    CONSTRAINT `fk_ubicacion_barrio`
        FOREIGN KEY (`idBarrioFK`) REFERENCES `barrio` (`idBarrio`)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 6. CATEGORIA
CREATE TABLE `categoria` (
    `idCategoria`           CHAR(36)        NOT NULL DEFAULT (UUID()),
    `nombreCategoria`       VARCHAR(60)     NOT NULL,
    `descripcionCategoria`  TEXT            NULL,
    `estadoCategoria`       BOOLEAN         NOT NULL DEFAULT TRUE,

    PRIMARY KEY (`idCategoria`),
    UNIQUE KEY `uk_categoria_nombre` (`nombreCategoria`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 7. TIPO_RECURSO
CREATE TABLE `tipo_recurso` (
    `idTipoRecurso`     CHAR(36)        NOT NULL DEFAULT (UUID()),
    `nombreTipoRecurso` VARCHAR(45)     NOT NULL,
    `carpetaBase`       VARCHAR(100)    NOT NULL,

    PRIMARY KEY (`idTipoRecurso`),
    UNIQUE KEY `uk_tipo_recurso_nombre` (`nombreTipoRecurso`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 8. FORMATO_RECURSO
CREATE TABLE `formato_recurso` (
    `idFormato`         CHAR(36)        NOT NULL DEFAULT (UUID()),
    `extension`         VARCHAR(15)     NOT NULL,
    `idTipoRecursoFK`   CHAR(36)        NOT NULL,

    PRIMARY KEY (`idFormato`),
    UNIQUE KEY `uk_formato_extension` (`extension`),
    KEY `fk_formato_tipo_recurso_idx` (`idTipoRecursoFK`),
    CONSTRAINT `fk_formato_tipo_recurso`
        FOREIGN KEY (`idTipoRecursoFK`) REFERENCES `tipo_recurso` (`idTipoRecurso`)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 9. USUARIO
CREATE TABLE `usuario` (
    `idUsuario`             CHAR(36)        NOT NULL DEFAULT (UUID()),
    `nombre1Usuario`        VARCHAR(45)     NOT NULL,
    `nombre2Usuario`        VARCHAR(45)     NULL,
    `apellido1Usuario`      VARCHAR(45)     NOT NULL,
    `apellido2Usuario`      VARCHAR(45)     NULL,
    `correoUsuario`         VARCHAR(100)    NOT NULL,
    `contrasenaHash`        VARCHAR(255)    NOT NULL,
    `fechaRegistroUsuario`  DATE            NOT NULL DEFAULT (CURRENT_DATE),
    `estadoUsuario`         BOOLEAN         NOT NULL DEFAULT TRUE,
    `idTipoUsuarioFK`       CHAR(36)        NOT NULL,

    PRIMARY KEY (`idUsuario`),
    UNIQUE KEY `uk_usuario_correo` (`correoUsuario`),
    KEY `fk_usuario_tipo_usuario_idx` (`idTipoUsuarioFK`),
    CONSTRAINT `chk_usuario_correo` CHECK (`correoUsuario` LIKE '%@%.%'),
    CONSTRAINT `fk_usuario_tipo_usuario`
        FOREIGN KEY (`idTipoUsuarioFK`) REFERENCES `tipo_usuario` (`idTipoUsuario`)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 10. ARTISTA
CREATE TABLE `artista` (
    `idArtista`         CHAR(36)        NOT NULL DEFAULT (UUID()),
    `nombreArtistico`   VARCHAR(100)    NOT NULL,
    `nombre1Real`       VARCHAR(45)     NOT NULL,
    `nombre2Real`       VARCHAR(45)     NULL,
    `apellido1Real`     VARCHAR(45)     NOT NULL,
    `apellido2Real`     VARCHAR(45)     NULL,
    `biografia`         TEXT            NULL,
    `perfilRedSocial`   VARCHAR(150)    NULL,
    `estadoArtista`     BOOLEAN         NOT NULL DEFAULT TRUE,
    `idPaisFK`          CHAR(36)        NOT NULL,

    PRIMARY KEY (`idArtista`),
    UNIQUE KEY `uk_artista_nombre_artistico` (`nombreArtistico`),
    KEY `fk_artista_pais_idx` (`idPaisFK`),
    CONSTRAINT `fk_artista_pais`
        FOREIGN KEY (`idPaisFK`) REFERENCES `pais` (`idPais`)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 11. OBRA
CREATE TABLE `obra` (
    `idObra`                CHAR(36)        NOT NULL DEFAULT (UUID()),
    `nombreObra`            VARCHAR(120)    NOT NULL,
    `descripcionObra`       TEXT            NULL,
    `fechaCreacion`         DATE            NOT NULL,
    `estadoConservacion`    VARCHAR(30)     NOT NULL,
    `fechaRegistroObra`     DATE            NOT NULL DEFAULT (CURRENT_DATE),
    `idArtistaFK`           CHAR(36)        NOT NULL,
    `idCategoriaFK`         CHAR(36)        NOT NULL,
    `idUbicacionFK`         CHAR(36)        NOT NULL,
    `idUsuarioFK`           CHAR(36)        NOT NULL,

    PRIMARY KEY (`idObra`),
    KEY `fk_obra_artista_idx` (`idArtistaFK`),
    KEY `fk_obra_categoria_idx` (`idCategoriaFK`),
    KEY `fk_obra_ubicacion_idx` (`idUbicacionFK`),
    KEY `fk_obra_usuario_idx` (`idUsuarioFK`),
    CONSTRAINT `chk_obra_estado_conservacion` CHECK (
        `estadoConservacion` IN ('Excelente', 'Bueno', 'Regular', 'Deteriorado', 'Destruido')
    ),
    CONSTRAINT `fk_obra_artista`
        FOREIGN KEY (`idArtistaFK`) REFERENCES `artista` (`idArtista`)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT `fk_obra_categoria`
        FOREIGN KEY (`idCategoriaFK`) REFERENCES `categoria` (`idCategoria`)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT `fk_obra_ubicacion`
        FOREIGN KEY (`idUbicacionFK`) REFERENCES `ubicacion` (`idUbicacion`)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT `fk_obra_usuario`
        FOREIGN KEY (`idUsuarioFK`) REFERENCES `usuario` (`idUsuario`)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 12. RUTA
CREATE TABLE `ruta` (
    `idRuta`            CHAR(36)        NOT NULL DEFAULT (UUID()),
    `nombreRuta`        VARCHAR(100)    NOT NULL,
    `descripcionRuta`   TEXT            NULL,
    `duracionEstimada`  INT             NOT NULL,
    `distanciaEstimada` DECIMAL(6, 2)   NOT NULL,
    `estadoRuta`        BOOLEAN         NOT NULL DEFAULT TRUE,

    PRIMARY KEY (`idRuta`),
    UNIQUE KEY `uk_ruta_nombre` (`nombreRuta`),
    CONSTRAINT `chk_ruta_duracion` CHECK (`duracionEstimada` > 0),
    CONSTRAINT `chk_ruta_distancia` CHECK (`distanciaEstimada` > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 13. RUTA_OBRA
CREATE TABLE `ruta_obra` (
    `idRutaObra`        CHAR(36)        NOT NULL DEFAULT (UUID()),
    `ordenVisita`       INT             NOT NULL,
    `recomendacion`     TEXT            NULL,
    `tiempoPermanencia` INT             NOT NULL DEFAULT 15,
    `idRutaFK`          CHAR(36)        NOT NULL,
    `idObraFK`          CHAR(36)        NOT NULL,

    PRIMARY KEY (`idRutaObra`),
    UNIQUE KEY `uk_ruta_obra_parada` (`idRutaFK`, `idObraFK`),
    UNIQUE KEY `uk_ruta_obra_orden` (`idRutaFK`, `ordenVisita`),
    KEY `fk_ruta_obra_ruta_idx` (`idRutaFK`),
    KEY `fk_ruta_obra_obra_idx` (`idObraFK`),
    CONSTRAINT `chk_ruta_obra_orden` CHECK (`ordenVisita` > 0),
    CONSTRAINT `chk_ruta_obra_tiempo` CHECK (`tiempoPermanencia` > 0),
    CONSTRAINT `fk_ruta_obra_ruta`
        FOREIGN KEY (`idRutaFK`) REFERENCES `ruta` (`idRuta`)
        ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT `fk_ruta_obra_obra`
        FOREIGN KEY (`idObraFK`) REFERENCES `obra` (`idObra`)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 14. RECURSO_MULTIMEDIA
CREATE TABLE `recurso_multimedia` (
    `idRecurso`             CHAR(36)        NOT NULL DEFAULT (UUID()),
    `nombreRecurso`         VARCHAR(100)    NOT NULL,
    `descripcionRecurso`    TEXT            NULL,
    `rutaArchivo`           VARCHAR(255)    NOT NULL,
    `tamanio`               DECIMAL(12, 2)  NOT NULL,
    `duracion`              INT             NULL,
    `fechaRegistroRecurso`  DATE            NOT NULL DEFAULT (CURRENT_DATE),
    `idFormatoFK`           CHAR(36)        NOT NULL,
    `idObraFK`              CHAR(36)        NOT NULL,

    PRIMARY KEY (`idRecurso`),
    KEY `fk_recurso_formato_idx` (`idFormatoFK`),
    KEY `fk_recurso_obra_idx` (`idObraFK`),
    CONSTRAINT `chk_recurso_tamanio` CHECK (`tamanio` > 0),
    CONSTRAINT `chk_recurso_duracion` CHECK (`duracion` IS NULL OR `duracion` > 0),
    CONSTRAINT `chk_recurso_ruta_relativa` CHECK (
        `rutaArchivo` NOT LIKE 'C:%' AND `rutaArchivo` NOT LIKE '/%' AND `rutaArchivo` NOT LIKE '\\%'
    ),
    CONSTRAINT `fk_recurso_formato`
        FOREIGN KEY (`idFormatoFK`) REFERENCES `formato_recurso` (`idFormato`)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT `fk_recurso_obra`
        FOREIGN KEY (`idObraFK`) REFERENCES `obra` (`idObra`)
        ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;


-- =============================================================================
-- 2. ÍNDICES ADICIONALES JUSTIFICADOS POR LAS CONSULTAS QUE MEJORAN
-- =============================================================================

-- Mejora las consultas de obras filtradas por categoría técnica y estado de conservación
CREATE INDEX `idx_obra_categoria_estado`
    ON `obra` (`idCategoriaFK`, `estadoConservacion`);

-- Mejora la consulta de recursos multimedia asociados a una obra y su unión con formato_recurso
CREATE INDEX `idx_recurso_obra_formato`
    ON `recurso_multimedia` (`idObraFK`, `idFormatoFK`);

-- Mejora las búsquedas espaciales por coordenadas en ubicacion
CREATE INDEX `idx_ubicacion_coordenadas`
    ON `ubicacion` (`latitud`, `longitud`);


-- =============================================================================
-- 3. VISTAS MULTITABLA (LLAMADAS DESDE LA APLICACIÓN C#)
-- =============================================================================

-- Vista 1: Catálogo completo de obras (JOIN de 9 tablas)
CREATE VIEW `vw_catalogo_obras_completo` AS
SELECT
    o.`idObra`,
    o.`nombreObra` AS `titulo_obra`,
    o.`descripcionObra` AS `descripcion_obra`,
    o.`fechaCreacion`,
    o.`estadoConservacion`,
    o.`fechaRegistroObra`,
    a.`idArtista`,
    a.`nombreArtistico` AS `artista`,
    CONCAT_WS(' ', a.`nombre1Real`, a.`nombre2Real`, a.`apellido1Real`, a.`apellido2Real`) AS `artista_nombre_real`,
    p.`nombrePais` AS `pais_artista`,
    a.`perfilRedSocial` AS `red_social_artista`,
    c.`idCategoria`,
    c.`nombreCategoria` AS `categoria_tecnica`,
    u.`idUbicacion`,
    u.`nombreLugar`,
    u.`direccion`,
    b.`nombreBarrio` AS `barrio`,
    com.`numeroComuna` AS `numero_comuna`,
    CONCAT('Comuna ', com.`numeroComuna`) AS `comuna`,
    u.`latitud`,
    u.`longitud`,
    usr.`idUsuario` AS `id_usuario_registrador`,
    CONCAT_WS(' ', usr.`nombre1Usuario`, usr.`nombre2Usuario`, usr.`apellido1Usuario`, usr.`apellido2Usuario`) AS `registrado_por`,
    tu.`nombreTipoUsuario` AS `rol_registrador`
FROM `obra` o
INNER JOIN `artista` a          ON o.`idArtistaFK`       = a.`idArtista`
INNER JOIN `pais` p             ON a.`idPaisFK`          = p.`idPais`
INNER JOIN `categoria` c        ON o.`idCategoriaFK`     = c.`idCategoria`
INNER JOIN `ubicacion` u        ON o.`idUbicacionFK`     = u.`idUbicacion`
INNER JOIN `barrio` b           ON u.`idBarrioFK`        = b.`idBarrio`
INNER JOIN `comuna` com         ON b.`idComunaFK`        = com.`idComuna`
INNER JOIN `usuario` usr        ON o.`idUsuarioFK`       = usr.`idUsuario`
INNER JOIN `tipo_usuario` tu    ON usr.`idTipoUsuarioFK` = tu.`idTipoUsuario`;

-- Vista 2: Itinerario detallado de rutas (JOIN de 7 tablas)
CREATE VIEW `vw_rutas_itinerario_detallado` AS
SELECT
    r.`idRuta`,
    r.`nombreRuta` AS `nombre_ruta`,
    r.`descripcionRuta` AS `descripcion_ruta`,
    r.`duracionEstimada` AS `duracion_total_ruta_min`,
    r.`distanciaEstimada` AS `distancia_total_km`,
    r.`estadoRuta` AS `estado_ruta`,
    ro.`idRutaObra`,
    ro.`ordenVisita` AS `orden_visita`,
    ro.`tiempoPermanencia` AS `tiempo_permanencia_min`,
    ro.`recomendacion` AS `recomendacion_parada`,
    o.`idObra`,
    o.`nombreObra` AS `titulo_obra`,
    o.`estadoConservacion`,
    a.`nombreArtistico` AS `artista`,
    u.`nombreLugar`,
    u.`direccion`,
    b.`nombreBarrio` AS `barrio`,
    CONCAT('Comuna ', com.`numeroComuna`) AS `comuna`,
    u.`latitud`,
    u.`longitud`
FROM `ruta` r
INNER JOIN `ruta_obra` ro   ON r.`idRuta`        = ro.`idRutaFK`
INNER JOIN `obra` o         ON ro.`idObraFK`     = o.`idObra`
INNER JOIN `artista` a      ON o.`idArtistaFK`   = a.`idArtista`
INNER JOIN `ubicacion` u    ON o.`idUbicacionFK` = u.`idUbicacion`
INNER JOIN `barrio` b       ON u.`idBarrioFK`    = b.`idBarrio`
INNER JOIN `comuna` com     ON b.`idComunaFK`    = com.`idComuna`;

-- Vista 3: Resumen de recursos multimedia por obra (JOIN de 6 tablas)
CREATE VIEW `vw_obras_resumen_multimedia` AS
SELECT
    o.`idObra`,
    o.`nombreObra` AS `titulo_obra`,
    a.`nombreArtistico` AS `artista`,
    c.`nombreCategoria` AS `categoria`,
    COUNT(rm.`idRecurso`) AS `total_recursos`,
    SUM(CASE WHEN tr.`nombreTipoRecurso` = 'IMAGEN' THEN 1 ELSE 0 END) AS `cant_imagenes`,
    SUM(CASE WHEN tr.`nombreTipoRecurso` = 'VIDEO' THEN 1 ELSE 0 END) AS `cant_videos`,
    SUM(CASE WHEN tr.`nombreTipoRecurso` = 'AUDIO' THEN 1 ELSE 0 END) AS `cant_audios`,
    SUM(CASE WHEN tr.`nombreTipoRecurso` = 'MODELO_3D' THEN 1 ELSE 0 END) AS `cant_modelos_3d`,
    SUM(CASE WHEN tr.`nombreTipoRecurso` = 'AR' THEN 1 ELSE 0 END) AS `cant_experiencias_ar`,
    ROUND(COALESCE(SUM(rm.`tamanio`), 0), 2) AS `tamano_total_mb`,
    MIN(CASE WHEN tr.`nombreTipoRecurso` = 'IMAGEN' THEN CONCAT(tr.`carpetaBase`, rm.`rutaArchivo`) END) AS `imagen_principal_preview`
FROM `obra` o
INNER JOIN `artista` a                  ON o.`idArtistaFK`      = a.`idArtista`
INNER JOIN `categoria` c                ON o.`idCategoriaFK`    = c.`idCategoria`
LEFT JOIN `recurso_multimedia` rm       ON o.`idObra`           = rm.`idObraFK`
LEFT JOIN `formato_recurso` fr          ON rm.`idFormatoFK`     = fr.`idFormato`
LEFT JOIN `tipo_recurso` tr             ON fr.`idTipoRecursoFK` = tr.`idTipoRecurso`
GROUP BY
    o.`idObra`,
    o.`nombreObra`,
    a.`nombreArtistico`,
    c.`nombreCategoria`;

-- Vista 4: Estadísticas culturales por comuna (JOIN de 6 tablas + GROUP BY)
CREATE VIEW `vw_estadisticas_culturales_comuna` AS
SELECT
    com.`numeroComuna`,
    CONCAT('Comuna ', com.`numeroComuna`) AS `comuna`,
    COUNT(DISTINCT b.`idBarrio`) AS `barrios_con_obras`,
    COUNT(DISTINCT o.`idObra`) AS `total_obras`,
    COUNT(DISTINCT o.`idArtistaFK`) AS `total_artistas_intervinientes`,
    COUNT(DISTINCT o.`idCategoriaFK`) AS `total_tecnicas_presentes`,
    SUM(CASE WHEN o.`estadoConservacion` IN ('Excelente', 'Bueno') THEN 1 ELSE 0 END) AS `obras_buen_estado`,
    SUM(CASE WHEN o.`estadoConservacion` IN ('Regular', 'Deteriorado', 'Destruido') THEN 1 ELSE 0 END) AS `obras_requieren_intervencion`
FROM `comuna` com
INNER JOIN `barrio` b       ON com.`idComuna`    = b.`idComunaFK`
INNER JOIN `ubicacion` u    ON b.`idBarrio`      = u.`idBarrioFK`
INNER JOIN `obra` o         ON u.`idUbicacion`   = o.`idUbicacionFK`
INNER JOIN `categoria` c    ON o.`idCategoriaFK` = c.`idCategoria`
INNER JOIN `artista` a      ON o.`idArtistaFK`   = a.`idArtista`
GROUP BY com.`idComuna`, com.`numeroComuna`;


-- =============================================================================
-- 4. PROCEDIMIENTOS ALMACENADOS (LLAMADOS DESDE LA APLICACIÓN C#)
-- =============================================================================

DELIMITER $$

CREATE PROCEDURE `sp_registrar_obra` (
    IN  p_id_obra             CHAR(36),
    IN  p_nombre_obra         VARCHAR(120),
    IN  p_descripcion_obra    TEXT,
    IN  p_fecha_creacion      DATE,
    IN  p_estado_conservacion VARCHAR(30),
    IN  p_id_artista          CHAR(36),
    IN  p_id_categoria        CHAR(36),
    IN  p_id_ubicacion        CHAR(36),
    IN  p_id_usuario          CHAR(36),
    OUT p_nuevo_id_obra       CHAR(36)
)
proc_block: BEGIN
    DECLARE v_count INT DEFAULT 0;

    SELECT COUNT(*) INTO v_count FROM `artista` WHERE `idArtista` = p_id_artista AND `estadoArtista` = TRUE;
    IF v_count = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'El artista seleccionado no existe o está inactivo.';
    END IF;

    SELECT COUNT(*) INTO v_count FROM `categoria` WHERE `idCategoria` = p_id_categoria AND `estadoCategoria` = TRUE;
    IF v_count = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'La categoría seleccionada no existe o está inactiva.';
    END IF;

    SELECT COUNT(*) INTO v_count FROM `ubicacion` WHERE `idUbicacion` = p_id_ubicacion;
    IF v_count = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'La ubicación seleccionada no existe.';
    END IF;

    SELECT COUNT(*) INTO v_count FROM `usuario` WHERE `idUsuario` = p_id_usuario AND `estadoUsuario` = TRUE;
    IF v_count = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'El usuario registrador no existe o está inactivo.';
    END IF;

    IF p_id_obra IS NULL OR TRIM(p_id_obra) = '' THEN
        SET p_nuevo_id_obra = UUID();
    ELSE
        SET p_nuevo_id_obra = p_id_obra;
    END IF;

    INSERT INTO `obra` (
        `idObra`, `nombreObra`, `descripcionObra`, `fechaCreacion`,
        `estadoConservacion`, `fechaRegistroObra`,
        `idArtistaFK`, `idCategoriaFK`, `idUbicacionFK`, `idUsuarioFK`
    ) VALUES (
        p_nuevo_id_obra, p_nombre_obra, p_descripcion_obra, p_fecha_creacion,
        p_estado_conservacion, CURRENT_DATE,
        p_id_artista, p_id_categoria, p_id_ubicacion, p_id_usuario
    );
END proc_block$$

CREATE PROCEDURE `sp_asociar_recurso_multimedia` (
    IN  p_id_recurso        CHAR(36),
    IN  p_id_obra           CHAR(36),
    IN  p_id_formato        CHAR(36),
    IN  p_nombre_recurso    VARCHAR(100),
    IN  p_descripcion       TEXT,
    IN  p_ruta_archivo      VARCHAR(255),
    IN  p_tamanio_mb        DECIMAL(12,2),
    IN  p_duracion_seg      INT,
    OUT p_nuevo_id_recurso  CHAR(36)
)
proc_block: BEGIN
    DECLARE v_count INT DEFAULT 0;

    SELECT COUNT(*) INTO v_count FROM `obra` WHERE `idObra` = p_id_obra;
    IF v_count = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'La obra especificada no existe.';
    END IF;

    SELECT COUNT(*) INTO v_count FROM `formato_recurso` WHERE `idFormato` = p_id_formato;
    IF v_count = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'El formato multimedia especificado no es válido.';
    END IF;

    IF p_id_recurso IS NULL OR TRIM(p_id_recurso) = '' THEN
        SET p_nuevo_id_recurso = UUID();
    ELSE
        SET p_nuevo_id_recurso = p_id_recurso;
    END IF;

    INSERT INTO `recurso_multimedia` (
        `idRecurso`, `nombreRecurso`, `descripcionRecurso`, `rutaArchivo`,
        `tamanio`, `duracion`, `fechaRegistroRecurso`, `idFormatoFK`, `idObraFK`
    ) VALUES (
        p_nuevo_id_recurso, p_nombre_recurso, p_descripcion, p_ruta_archivo,
        p_tamanio_mb, p_duracion_seg, CURRENT_DATE, p_id_formato, p_id_obra
    );
END proc_block$$

CREATE PROCEDURE `sp_registrar_obra_con_recurso_principal` (
    IN  p_nombre_obra         VARCHAR(120),
    IN  p_descripcion_obra    TEXT,
    IN  p_fecha_creacion      DATE,
    IN  p_estado_conservacion VARCHAR(30),
    IN  p_id_artista          CHAR(36),
    IN  p_id_categoria        CHAR(36),
    IN  p_id_ubicacion        CHAR(36),
    IN  p_id_usuario          CHAR(36),
    IN  p_id_formato          CHAR(36),
    IN  p_nombre_recurso      VARCHAR(100),
    IN  p_ruta_archivo        VARCHAR(255),
    IN  p_tamanio_mb          DECIMAL(12,2),
    OUT p_id_obra_creada      CHAR(36),
    OUT p_id_recurso_creado   CHAR(36)
)
proc_block: BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    SET p_id_obra_creada = UUID();
    SET p_id_recurso_creado = UUID();

    CALL `sp_registrar_obra`(
        p_id_obra_creada, p_nombre_obra, p_descripcion_obra,
        p_fecha_creacion, p_estado_conservacion,
        p_id_artista, p_id_categoria, p_id_ubicacion, p_id_usuario,
        p_id_obra_creada
    );

    CALL `sp_asociar_recurso_multimedia`(
        p_id_recurso_creado, p_id_obra_creada, p_id_formato,
        p_nombre_recurso, 'Fotografía principal de la obra',
        p_ruta_archivo, p_tamanio_mb, NULL,
        p_id_recurso_creado
    );

    COMMIT;
END proc_block$$

CREATE PROCEDURE `sp_asignar_obra_a_ruta` (
    IN  p_id_ruta           CHAR(36),
    IN  p_id_obra           CHAR(36),
    IN  p_orden_visita      INT,
    IN  p_tiempo_min        INT,
    IN  p_recomendacion     TEXT,
    OUT p_nuevo_id_hito     CHAR(36)
)
proc_block: BEGIN
    SET p_nuevo_id_hito = UUID();

    INSERT INTO `ruta_obra` (
        `idRutaObra`, `ordenVisita`, `recomendacion`,
        `tiempoPermanencia`, `idRutaFK`, `idObraFK`
    ) VALUES (
        p_nuevo_id_hito, p_orden_visita, p_recomendacion,
        p_tiempo_min, p_id_ruta, p_id_obra
    );
END proc_block$$

DELIMITER ;
