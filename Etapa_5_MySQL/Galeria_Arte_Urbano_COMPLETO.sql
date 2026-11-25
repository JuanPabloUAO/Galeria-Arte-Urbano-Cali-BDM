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


-- =============================================================================
-- PROYECTO DE CURSO: GALERÍA DE ARTE URBANO DE CALI (DATOS DE PRUEBA Y DML)
-- Asignatura: Bases de Datos Multimedia (BDM) - Grupo 51
-- Universidad Autónoma de Occidente (UAO) - Facultad de Ingeniería
-- Integrantes: Juan Pablo Campo Bertin (2235544) · Jean Deiby García
-- Docente: David Alejandro Castro Benavides
-- =============================================================================

USE `galeria_arte_urbano_db`;

SET FOREIGN_KEY_CHECKS = 0;
TRUNCATE TABLE `recurso_multimedia`;
TRUNCATE TABLE `ruta_obra`;
TRUNCATE TABLE `ruta`;
TRUNCATE TABLE `obra`;
TRUNCATE TABLE `artista`;
TRUNCATE TABLE `usuario`;
TRUNCATE TABLE `formato_recurso`;
TRUNCATE TABLE `tipo_recurso`;
TRUNCATE TABLE `categoria`;
TRUNCATE TABLE `ubicacion`;
TRUNCATE TABLE `barrio`;
TRUNCATE TABLE `comuna`;
TRUNCATE TABLE `tipo_usuario`;
TRUNCATE TABLE `pais`;
SET FOREIGN_KEY_CHECKS = 1;

-- 1. PAIS
INSERT INTO `pais` (`idPais`, `nombrePais`) VALUES
('10000000-0000-4000-8000-000000000001', 'Colombia'),
('10000000-0000-4000-8000-000000000002', 'México'),
('10000000-0000-4000-8000-000000000003', 'Brasil'),
('10000000-0000-4000-8000-000000000004', 'Chile'),
('10000000-0000-4000-8000-000000000005', 'España');

-- 2. TIPO_USUARIO
INSERT INTO `tipo_usuario` (`idTipoUsuario`, `nombreTipoUsuario`) VALUES
('20000000-0000-4000-8000-000000000001', 'Administrador'),
('20000000-0000-4000-8000-000000000002', 'Visitante');

-- 3. COMUNA
INSERT INTO `comuna` (`idComuna`, `numeroComuna`) VALUES
('30000000-0000-4000-8000-000000000002', 2),
('30000000-0000-4000-8000-000000000003', 3),
('30000000-0000-4000-8000-000000000009', 9),
('30000000-0000-4000-8000-000000000019', 19),
('30000000-0000-4000-8000-000000000020', 20);

-- 4. BARRIO
INSERT INTO `barrio` (`idBarrio`, `nombreBarrio`, `idComunaFK`) VALUES
('40000000-0000-4000-8000-000000000001', 'San Pedro',    '30000000-0000-4000-8000-000000000003'),
('40000000-0000-4000-8000-000000000002', 'Siloé',        '30000000-0000-4000-8000-000000000020'),
('40000000-0000-4000-8000-000000000003', 'San Antonio',  '30000000-0000-4000-8000-000000000003'),
('40000000-0000-4000-8000-000000000004', 'San Fernando', '30000000-0000-4000-8000-000000000019'),
('40000000-0000-4000-8000-000000000005', 'El Peñón',     '30000000-0000-4000-8000-000000000003'),
('40000000-0000-4000-8000-000000000006', 'Santa Rosa',   '30000000-0000-4000-8000-000000000003'),
('40000000-0000-4000-8000-000000000007', 'Centenario',   '30000000-0000-4000-8000-000000000002'),
('40000000-0000-4000-8000-000000000008', 'Obrero',       '30000000-0000-4000-8000-000000000009');

-- 5. UBICACION
INSERT INTO `ubicacion` (`idUbicacion`, `nombreLugar`, `direccion`, `latitud`, `longitud`, `idBarrioFK`) VALUES
('50000000-0000-4000-8000-000000000001', 'Muro Bulevar del Río - Ermita',                      'Carrera 1 entre Calles 12 y 13', 3.45330000, -76.53280000, '40000000-0000-4000-8000-000000000001'),
('50000000-0000-4000-8000-000000000002', 'Museo Libre de Siloé (MULAS) - Sector La Estrella',  'Diagonal 51 con Calle 2 Oeste',  3.42150000, -76.55620000, '40000000-0000-4000-8000-000000000002'),
('50000000-0000-4000-8000-000000000003', 'Callejón del Acueducto y Colina de San Antonio',     'Carrera 10 con Calle 1',         3.44780000, -76.54120000, '40000000-0000-4000-8000-000000000003'),
('50000000-0000-4000-8000-000000000004', 'Parque Panamericano (Las Banderas) - Muros Calle 5', 'Calle 5 con Carrera 36',         3.43120000, -76.54250000, '40000000-0000-4000-8000-000000000004'),
('50000000-0000-4000-8000-000000000005', 'Calle de la Escopeta y Corredor del Peñón',          'Calle 2 Oeste con Carrera 3',    3.44950000, -76.53850000, '40000000-0000-4000-8000-000000000005'),
('50000000-0000-4000-8000-000000000006', 'Avenida Colombia - Pasaje del Río y Muros Túnel',    'Carrera 1 con Calle 8',          3.45100000, -76.53400000, '40000000-0000-4000-8000-000000000006'),
('50000000-0000-4000-8000-000000000007', 'Plazoleta Jairo Varela y Parque de los Poetas',      'Avenida 2 Norte con Calle 10',   3.45520000, -76.53350000, '40000000-0000-4000-8000-000000000007'),
('50000000-0000-4000-8000-000000000008', 'Paseo Bolívar y Entorno Puente Ortiz',               'Avenida Colombia con Calle 11',  3.45380000, -76.53390000, '40000000-0000-4000-8000-000000000001');

-- 6. CATEGORIA (5 categorías >= mínimo 5 exigido por la rúbrica)
INSERT INTO `categoria` (`idCategoria`, `nombreCategoria`, `descripcionCategoria`, `estadoCategoria`) VALUES
('60000000-0000-4000-8000-000000000001', 'Muralismo Social y Comunitario',   'Intervenciones pictóricas a gran escala orientadas a narrar la memoria colectiva, la dignidad comunitaria y la identidad de sectores urbanos y laderas.', TRUE),
('60000000-0000-4000-8000-000000000002', 'Grafiti Wildstyle / Lettering',    'Composiciones tipográficas dinámicas con aerosol, letras entrelazadas complejas, brillos, sombras y estilización formal del tag urbano.', TRUE),
('60000000-0000-4000-8000-000000000003', 'Stencil y Cartelismo Tipográfico', 'Técnicas reproductivas basadas en matrices cortadas a mano, estarcido en aerosol y cartelería tipográfica tradicional paste-up adherida al espacio público.', TRUE),
('60000000-0000-4000-8000-000000000004', 'Retrato Realista Urbano',          'Representaciones hiperrealistas o figurativas de personajes locales, cultores de la salsa, campesinos, líderes sociales y rostros del Valle del Cauca.', TRUE),
('60000000-0000-4000-8000-000000000005', 'Neomuralismo y Biodiversidad',     'Propuestas contemporáneas que fusionan estética psicodélica, fauna y flora del Parque Nacional Natural Farallones y cosmogonía indígena.', TRUE);

-- 7. TIPO_RECURSO
INSERT INTO `tipo_recurso` (`idTipoRecurso`, `nombreTipoRecurso`, `carpetaBase`) VALUES
('70000000-0000-4000-8000-000000000001', 'IMAGEN',    'RecursosMultimedia/Imagenes/'),
('70000000-0000-4000-8000-000000000002', 'VIDEO',     'RecursosMultimedia/Videos/'),
('70000000-0000-4000-8000-000000000003', 'AUDIO',     'RecursosMultimedia/Audios/'),
('70000000-0000-4000-8000-000000000004', 'MODELO_3D', 'RecursosMultimedia/Modelos3D/'),
('70000000-0000-4000-8000-000000000005', 'AR',        'RecursosMultimedia/AR/');

-- 8. FORMATO_RECURSO
INSERT INTO `formato_recurso` (`idFormato`, `extension`, `idTipoRecursoFK`) VALUES
('80000000-0000-4000-8000-000000000001', 'JPG',  '70000000-0000-4000-8000-000000000001'),
('80000000-0000-4000-8000-000000000002', 'PNG',  '70000000-0000-4000-8000-000000000001'),
('80000000-0000-4000-8000-000000000003', 'WEBP', '70000000-0000-4000-8000-000000000001'),
('80000000-0000-4000-8000-000000000004', 'MP4',  '70000000-0000-4000-8000-000000000002'),
('80000000-0000-4000-8000-000000000005', 'MP3',  '70000000-0000-4000-8000-000000000003'),
('80000000-0000-4000-8000-000000000006', 'WAV',  '70000000-0000-4000-8000-000000000003'),
('80000000-0000-4000-8000-000000000007', 'GLB',  '70000000-0000-4000-8000-000000000004'),
('80000000-0000-4000-8000-000000000008', 'USDZ', '70000000-0000-4000-8000-000000000005');

-- 9. USUARIO (contraseña cifrada con SHA-256 para 'Cali2026*': 1eba23bb8ef5681b22aeb5c62c87fcaca1c319be5843db51d04ca048358f22cf)
INSERT INTO `usuario` (`idUsuario`, `nombre1Usuario`, `nombre2Usuario`, `apellido1Usuario`, `apellido2Usuario`, `correoUsuario`, `contrasenaHash`, `fechaRegistroUsuario`, `estadoUsuario`, `idTipoUsuarioFK`) VALUES
('90000000-0000-4000-8000-000000000001', 'Juan',      'Pablo',     'Campo',  'Bertin',    'juan.campo@uao.edu.co',        '1eba23bb8ef5681b22aeb5c62c87fcaca1c319be5843db51d04ca048358f22cf', '2026-09-01', TRUE, '20000000-0000-4000-8000-000000000001'),
('90000000-0000-4000-8000-000000000002', 'Jean',      'Deiby',     'Garcia', NULL,        'jean.garcia@uao.edu.co',       '1eba23bb8ef5681b22aeb5c62c87fcaca1c319be5843db51d04ca048358f22cf', '2026-09-01', TRUE, '20000000-0000-4000-8000-000000000001'),
('90000000-0000-4000-8000-000000000003', 'David',     'Alejandro', 'Castro', 'Benavides', 'david.castro@talentum.edu.co', '1eba23bb8ef5681b22aeb5c62c87fcaca1c319be5843db51d04ca048358f22cf', '2026-09-05', TRUE, '20000000-0000-4000-8000-000000000002'),
('90000000-0000-4000-8000-000000000004', 'Valentina', NULL,        'Ospina', 'Restrepo',  'turismo.cultura@cali.gov.co',  '1eba23bb8ef5681b22aeb5c62c87fcaca1c319be5843db51d04ca048358f22cf', '2026-09-10', TRUE, '20000000-0000-4000-8000-000000000002');

-- 10. ARTISTA (6 artistas >= mínimo 5 exigido por la rúbrica)
INSERT INTO `artista` (`idArtista`, `nombreArtistico`, `nombre1Real`, `nombre2Real`, `apellido1Real`, `apellido2Real`, `biografia`, `perfilRedSocial`, `estadoArtista`, `idPaisFK`) VALUES
('a1000000-0000-4000-8000-000000000001', 'Guache',             'Óscar',     'Mauricio', 'González', 'Ruiz',      'Reconocido artista visual y muralista colombiano. Combina elementos del arte popular tradicional, indigenismo latinoamericano y grafiti contemporáneo con presencia de iconografía prehispánica.', 'https://instagram.com/guache_art',     TRUE, '10000000-0000-4000-8000-000000000001'),
('a2000000-0000-4000-8000-000000000002', 'Gleo',               'Natalia',   NULL,       'Gallego',  'Sánchez',   'Destacada artista urbana nacida en Cali con proyección internacional. Sus obras presentan figuras místicas con antifaces amarillos, integrando fauna endémica, espiritualidad y naturaleza selvática.', 'https://instagram.com/gleo_art',       TRUE, '10000000-0000-4000-8000-000000000001'),
('a3000000-0000-4000-8000-000000000003', 'Sobera',             'Juan',      'David',    'Sobera',   'Vallecilla','Muralista y diseñador gráfico caleño. Su lenguaje pictórico resalta la biodiversidad de aves del Valle del Cauca, la cultura salsera y el retrato figurativo con paletas tropicales vibrantes.', 'https://instagram.com/sobera_mural',   TRUE, '10000000-0000-4000-8000-000000000001'),
('a4000000-0000-4000-8000-000000000004', 'Mesa Gráfica Siloé', 'Carlos',    'Andrés',   'Gómez',    'Perea',     'Líder y vocero del colectivo comunitario de artistas de la Comuna 20. Creadores del Museo Libre de Arte Urbano de Siloé (MULAS), transformando la ladera en galería a cielo abierto por la memoria y la paz.', 'https://instagram.com/mulas_siloe',    TRUE, '10000000-0000-4000-8000-000000000001'),
('a5000000-0000-4000-8000-000000000005', 'La Linterna Cali',   'Héctor',    'Fabio',    'Morales',  'Rojas',     'Maestro impresor de la mítica imprenta tipográfica tradicional de Cali fundada en 1934. Revitalizada por artistas urbanos mediante grabado en linóleo, cartelería callejera y stencil en San Antonio.', 'https://instagram.com/lalinternacali', TRUE, '10000000-0000-4000-8000-000000000001'),
('a6000000-0000-4000-8000-000000000006', 'Dexs Graffiti',      'Alexander', NULL,       'Ramos',    'Hurtado',   'Pionero de la cultura del grafiti y wildstyle en Cali. Con más de 20 años de trayectoria decorando muros del Túnel Mundialista, Paso del Comercio y corredores de la Calle Quinta.', 'https://instagram.com/dexsgraffiti',  TRUE, '10000000-0000-4000-8000-000000000001');

-- 11. OBRA (12 obras >= mínimo 10 exigido por la rúbrica)
INSERT INTO `obra` (`idObra`, `nombreObra`, `descripcionObra`, `fechaCreacion`, `estadoConservacion`, `fechaRegistroObra`, `idArtistaFK`, `idCategoriaFK`, `idUbicacionFK`, `idUsuarioFK`) VALUES
('d1000000-0000-4000-8000-000000000001',
 'El Vuelo de la Guacamaya y los Farallones',
 'Impresionante mural de gran formato que representa una deidad femenina alada con máscara dorada, rodeada por la exuberancia de la vegetación nativa del Valle y aves endémicas de la cordillera.',
 '2023-08-15', 'Excelente', '2026-09-02',
 'a2000000-0000-4000-8000-000000000002', '60000000-0000-4000-8000-000000000005', '50000000-0000-4000-8000-000000000001', '90000000-0000-4000-8000-000000000001'),

('d1000000-0000-4000-8000-000000000002',
 'Memoria, Resistencia y Dignidad de Siloé',
 'Mural icónico ubicado en la subida a La Estrella en Siloé. Relata la historia de lucha obrera minera, la vida cotidiana de las madres comunitarias y el renacer juvenil a través del arte y la música.',
 '2022-11-20', 'Bueno', '2026-09-02',
 'a4000000-0000-4000-8000-000000000004', '60000000-0000-4000-8000-000000000001', '50000000-0000-4000-8000-000000000002', '90000000-0000-4000-8000-000000000001'),

('d1000000-0000-4000-8000-000000000003',
 'Sinfonía Calima: Oro, Maíz y Tambor',
 'Obra pictórica en las escaleras empedradas de San Antonio. Resalta el legado de la cultura precolombina Calima, la espiritualidad del maíz y la vibración afroindígena del territorio vallecaucano.',
 '2023-04-10', 'Excelente', '2026-09-02',
 'a1000000-0000-4000-8000-000000000001', '60000000-0000-4000-8000-000000000005', '50000000-0000-4000-8000-000000000003', '90000000-0000-4000-8000-000000000002'),

('d1000000-0000-4000-8000-000000000004',
 'Grito Gráfico en Tipografía Móvil',
 'Intervención en espacio público mediante técnica mixta de paste-up y grabado tradicional. Recupera fuentes tipográficas históricas con frases de poetas caleños y pregones de salsa.',
 '2024-02-14', 'Bueno', '2026-09-02',
 'a5000000-0000-4000-8000-000000000005', '60000000-0000-4000-8000-000000000003', '50000000-0000-4000-8000-000000000003', '90000000-0000-4000-8000-000000000002'),

('d1000000-0000-4000-8000-000000000005',
 'Cali Salsa, Brisa y Movimiento',
 'Retrato monumental de bailarines tradicionales de salsa caleña de la vieja guardia. Captura el movimiento de los pies, el brillo de los trajes y la calidez del pueblo caleño en la Calle 5.',
 '2023-12-05', 'Excelente', '2026-09-03',
 'a3000000-0000-4000-8000-000000000003', '60000000-0000-4000-8000-000000000004', '50000000-0000-4000-8000-000000000004', '90000000-0000-4000-8000-000000000001'),

('d1000000-0000-4000-8000-000000000006',
 'Guardianes del Río Cali y las Aguas',
 'Mural figurativo que rinde homenaje al río tutelar de la ciudad. Representa figuras híbridas protectoras de la flora riparia, iguanas y gualas que habitan el corredor ambiental del Peñón.',
 '2024-05-18', 'Excelente', '2026-09-03',
 'a2000000-0000-4000-8000-000000000002', '60000000-0000-4000-8000-000000000005', '50000000-0000-4000-8000-000000000005', '90000000-0000-4000-8000-000000000001'),

('d1000000-0000-4000-8000-000000000007',
 'Wildstyle Caleño: Letras de la Sucursal',
 'Pieza clásica de grafiti de alta dificultad técnica en aerosol. Tipografía entrelazada tridimensional con tonalidades fucsia, turquesa y amarillo sobre la Avenida Colombia.',
 '2023-09-22', 'Bueno', '2026-09-04',
 'a6000000-0000-4000-8000-000000000006', '60000000-0000-4000-8000-000000000002', '50000000-0000-4000-8000-000000000006', '90000000-0000-4000-8000-000000000002'),

('d1000000-0000-4000-8000-000000000008',
 'Las Manos de los Carboneros y la Ladera',
 'Mural que documenta las raíces mineras del carbón en Siloé. Rostros ajados de mineros ancestrales con manos en primer plano que sostienen picos y linternas de carburo.',
 '2021-07-10', 'Deteriorado', '2026-09-04',
 'a4000000-0000-4000-8000-000000000004', '60000000-0000-4000-8000-000000000001', '50000000-0000-4000-8000-000000000002', '90000000-0000-4000-8000-000000000001'),

('d1000000-0000-4000-8000-000000000009',
 'Retrato al Obrero y Campesino del Valle',
 'Muralismo de contenido agrario y popular frente al río. Rostro de un campesino cañero vallecaucano mirando hacia el horizonte con sombrero de paja y machete al cinto.',
 '2023-01-30', 'Bueno', '2026-09-04',
 'a1000000-0000-4000-8000-000000000001', '60000000-0000-4000-8000-000000000004', '50000000-0000-4000-8000-000000000001', '90000000-0000-4000-8000-000000000001'),

('d1000000-0000-4000-8000-000000000010',
 'Melodía de Viento y Trompetas del Niche',
 'Mural temático en honor a la composición musical del maestro Jairo Varela. Retrato expresionista de músicos tocando campanas, congas y trompetas al compás de Cali Pachanguero.',
 '2024-01-15', 'Excelente', '2026-09-05',
 'a3000000-0000-4000-8000-000000000003', '60000000-0000-4000-8000-000000000004', '50000000-0000-4000-8000-000000000007', '90000000-0000-4000-8000-000000000002'),

('d1000000-0000-4000-8000-000000000011',
 'Ancestralidad, Jaguar y Resistencia',
 'Intervención en el Paseo Bolívar con estética indigenista. Un jaguar amazónico en tonos ocre y azul petróleo emerge de patrones geométricos ceremoniales.',
 '2022-09-12', 'Bueno', '2026-09-05',
 'a1000000-0000-4000-8000-000000000001', '60000000-0000-4000-8000-000000000005', '50000000-0000-4000-8000-000000000008', '90000000-0000-4000-8000-000000000001'),

('d1000000-0000-4000-8000-000000000012',
 'Niños de la Estrella: Esperanza en Colores',
 'Pintura comunitaria en una cancha de Siloé con retratos de niños jugando, cometas de papel elevándose al cielo y el sol poniéndose tras la cordillera occidental.',
 '2024-03-20', 'Excelente', '2026-09-05',
 'a4000000-0000-4000-8000-000000000004', '60000000-0000-4000-8000-000000000001', '50000000-0000-4000-8000-000000000002', '90000000-0000-4000-8000-000000000001');

-- 12. RUTA (3 rutas >= mínimo 3 exigido por la rúbrica)
INSERT INTO `ruta` (`idRuta`, `nombreRuta`, `descripcionRuta`, `duracionEstimada`, `distanciaEstimada`, `estadoRuta`) VALUES
('f1000000-0000-4000-8000-000000000001',
 'Circuito Histórico y Neomuralista: Bulevar del Río a San Antonio',
 'Recorrido peatonal emblemático por la Comuna 3. Inicia en el Muro Bulevar junto a la Ermita, bordea el río Cali pasando por la Calle de la Escopeta y culmina en las escaleras patrimoniales de San Antonio admirando obras de Gleo y Guache.',
 120, 2.50, TRUE),

('f2000000-0000-4000-8000-000000000002',
 'Ruta del Grafiti y Memoria Comunitaria en Siloé (MULAS)',
 'Experiencia cultural y social transformadora en la Comuna 20. Guiada por los propios jóvenes del colectivo, asciende hacia el monumento de La Estrella descubriendo murales de memoria obrera, resiliencia y esperanza.',
 150, 3.20, TRUE),

('f3000000-0000-4000-8000-000000000003',
 'Corredor de la Salsa, Letras Urbanas y Ritmo Caleño',
 'Circuito mixto peatonal-vehicular que conecta los templos de la música y la gráfica urbana: desde la Plazoleta Jairo Varela, pasando por los muros del Túnel Mundialista, el Paseo Bolívar hasta el Parque Panamericano en la Calle 5.',
 135, 4.80, TRUE);

-- 13. RUTA_OBRA (12 paradas ordenadas sin repetir obra ni orden en cada ruta)
INSERT INTO `ruta_obra` (`idRutaObra`, `ordenVisita`, `recomendacion`, `tiempoPermanencia`, `idRutaFK`, `idObraFK`) VALUES
('b1000000-0000-4000-8000-000000000001', 1, 'Punto de inicio en la Ermita. Recomendado llegar a las 4:00 pm para aprovechar la brisa caleña y la luz lateral sobre el mural de Gleo.', 25, 'f1000000-0000-4000-8000-000000000001', 'd1000000-0000-4000-8000-000000000001'),
('b1000000-0000-4000-8000-000000000002', 2, 'Ubicado a 200 metros sobre el mismo Bulevar. Excelente punto para fotografía en contrapicado del rostro campesino.', 20, 'f1000000-0000-4000-8000-000000000001', 'd1000000-0000-4000-8000-000000000009'),
('b1000000-0000-4000-8000-000000000003', 3, 'Caminar cruzando hacia la Calle de la Escopeta. Observar la integración del mural con la vegetación del río.', 25, 'f1000000-0000-4000-8000-000000000001', 'd1000000-0000-4000-8000-000000000006'),
('b1000000-0000-4000-8000-000000000004', 4, 'Ascenso a la Colina de San Antonio. Probar macetas de azúcar típicas y contemplar el atardecer sobre el mural de Guache.', 35, 'f1000000-0000-4000-8000-000000000001', 'd1000000-0000-4000-8000-000000000003'),

('b1000000-0000-4000-8000-000000000005', 1, 'Punto de encuentro en la estación del MÍO Cable Brisas de Mayo. Iniciar con la explicación histórica del colectivo MULAS.', 30, 'f2000000-0000-4000-8000-000000000002', 'd1000000-0000-4000-8000-000000000002'),
('b1000000-0000-4000-8000-000000000006', 2, 'Sector Brisas. Usar calzado cómodo para caminar por callejones de ladera. Escuchar el testimonio minero con audífonos.', 25, 'f2000000-0000-4000-8000-000000000002', 'd1000000-0000-4000-8000-000000000008'),
('b1000000-0000-4000-8000-000000000007', 3, 'Cancha deportiva de la comunidad. Espacio ideal para interactuar con niños y jóvenes artistas en formación.', 25, 'f2000000-0000-4000-8000-000000000002', 'd1000000-0000-4000-8000-000000000012'),
('b1000000-0000-4000-8000-000000000008', 4, 'Parada final conectando con cartelería popular de La Linterna. Vista panorámica nocturna de toda la ciudad de Cali.', 30, 'f2000000-0000-4000-8000-000000000002', 'd1000000-0000-4000-8000-000000000004'),

('b1000000-0000-4000-8000-000000000009', 1, 'Inicio frente a la trompeta monumental de Jairo Varela. Escuchar las anécdotas de composición de salsa con la audioguía.', 25, 'f3000000-0000-4000-8000-000000000003', 'd1000000-0000-4000-8000-000000000010'),
('b1000000-0000-4000-8000-000000000010', 2, 'Descenso hacia los muros del Túnel Mundialista. Excelente para apreciar la técnica de aerosol y letras wildstyle de Dexs.', 20, 'f3000000-0000-4000-8000-000000000003', 'd1000000-0000-4000-8000-000000000007'),
('b1000000-0000-4000-8000-000000000011', 3, 'Conexión por el Paseo Bolívar junto al Puente Ortiz. Apreciar el jaguar ceremonial prehispánico.', 25, 'f3000000-0000-4000-8000-000000000003', 'd1000000-0000-4000-8000-000000000011'),
('b1000000-0000-4000-8000-000000000012', 4, 'Cierre en el Parque Panamericano en la Calle 5. Visita a puestos de cholado caleño mientras se disfruta el mural de bailarines.', 35, 'f3000000-0000-4000-8000-000000000003', 'd1000000-0000-4000-8000-000000000005');

-- 14. RECURSO_MULTIMEDIA (30 recursos >= mínimo 15 exigido por la rúbrica)
INSERT INTO `recurso_multimedia` (`idRecurso`, `nombreRecurso`, `descripcionRecurso`, `rutaArchivo`, `tamanio`, `duracion`, `fechaRegistroRecurso`, `idFormatoFK`, `idObraFK`) VALUES
('c1000000-0000-4000-8000-000000000001', 'Fotografía Frontal Diurna',             'Toma en plano general con luz natural matutina en el Bulevar del Río',  'obra_001_frontal.jpg',        4.52, NULL, '2026-09-02', '80000000-0000-4000-8000-000000000001', 'd1000000-0000-4000-8000-000000000001'),
('c1000000-0000-4000-8000-000000000002', 'Detalle Rostro y Máscara',              'Primer plano del trabajo en spray y texturas del rostro mítico',        'obra_001_detalle.png',        6.13, NULL, '2026-09-02', '80000000-0000-4000-8000-000000000002', 'd1000000-0000-4000-8000-000000000001'),
('c1000000-0000-4000-8000-000000000003', 'Time-lapse Proceso Pictórico',          'Grabación acelerada de 4 días de pintura por la artista Gleo',          'obra_001_proceso.mp4',       54.89,  185, '2026-09-02', '80000000-0000-4000-8000-000000000004', 'd1000000-0000-4000-8000-000000000001'),
('c1000000-0000-4000-8000-000000000004', 'Audioguía Curatorial',                  'Narración en audio sobre la fauna y el concepto mítico de la obra',     'obra_001_audioguia.mp3',      8.45,  240, '2026-09-02', '80000000-0000-4000-8000-000000000005', 'd1000000-0000-4000-8000-000000000001'),
('c1000000-0000-4000-8000-000000000005', 'Modelo 3D del Muro y Relieve',          'Malla tridimensional fotogramétrica del muro intervenido',              'obra_001_fotogrametria.glb', 18.45, NULL, '2026-09-02', '80000000-0000-4000-8000-000000000007', 'd1000000-0000-4000-8000-000000000001'),
('c1000000-0000-4000-8000-000000000006', 'Fotografía Panorámica La Estrella',     'Vista panorámica con la ladera de Siloé y la ciudad al fondo',          'obra_002_panoramica.jpg',     5.12, NULL, '2026-09-02', '80000000-0000-4000-8000-000000000001', 'd1000000-0000-4000-8000-000000000002'),
('c1000000-0000-4000-8000-000000000007', 'Testimonio Lideresa Comunitaria',       'Entrevista testimonial a doña Carmen sobre la historia del mural',      'obra_002_testimonio.mp3',     9.80,  310, '2026-09-02', '80000000-0000-4000-8000-000000000005', 'd1000000-0000-4000-8000-000000000002'),
('c1000000-0000-4000-8000-000000000008', 'Experiencia Realidad Aumentada',        'Marcador de realidad aumentada para visualización interactiva móvil',   'obra_002_experiencia.usdz',  12.30, NULL, '2026-09-02', '80000000-0000-4000-8000-000000000008', 'd1000000-0000-4000-8000-000000000002'),
('c1000000-0000-4000-8000-000000000009', 'Fotografía Escalinatas San Antonio',    'Perspectiva ascendente del mural integrado en la arquitectura colonial', 'obra_003_escaleras.jpg',      4.92, NULL, '2026-09-02', '80000000-0000-4000-8000-000000000001', 'd1000000-0000-4000-8000-000000000003'),
('c1000000-0000-4000-8000-000000000010', 'Mini-Doc Entrevista con Guache',        'Entrevista al artista explicando la simbología orfebre Calima',         'obra_003_entrevista.mp4',    78.90,  260, '2026-09-02', '80000000-0000-4000-8000-000000000004', 'd1000000-0000-4000-8000-000000000003'),
('c1000000-0000-4000-8000-000000000011', 'Muestra Auditiva Marimba de Chonta',    'Pista sonora ambiental de música del Pacífico que acompaña la obra',    'obra_003_marimba.mp3',        6.20,  195, '2026-09-02', '80000000-0000-4000-8000-000000000005', 'd1000000-0000-4000-8000-000000000003'),
('c1000000-0000-4000-8000-000000000012', 'Fotografía Muro Paste-Up',              'Detalle de carteles tipográficos sobrepuestos en muro empedrado',       'obra_004_carteles.jpg',       3.80, NULL, '2026-09-02', '80000000-0000-4000-8000-000000000001', 'd1000000-0000-4000-8000-000000000004'),
('c1000000-0000-4000-8000-000000000013', 'Sonido de Prensa Tipográfica Marinoni', 'Registro sonoro histórico de la imprenta mecánica de 1890 en acción',   'obra_004_imprenta.mp3',       4.50,  120, '2026-09-02', '80000000-0000-4000-8000-000000000005', 'd1000000-0000-4000-8000-000000000004'),
('c1000000-0000-4000-8000-000000000014', 'Fotografía Frontal Mural Salsa',        'Composición completa con iluminación nocturna de reflectores',          'obra_005_frontal.jpg',        5.60, NULL, '2026-09-03', '80000000-0000-4000-8000-000000000001', 'd1000000-0000-4000-8000-000000000005'),
('c1000000-0000-4000-8000-000000000015', 'Cápsula de Video Bailarines',           'Bailarines de salsa ejecutando el paso frente al mural de Sobera',      'obra_005_bailarines.mp4',    62.30,  150, '2026-09-03', '80000000-0000-4000-8000-000000000004', 'd1000000-0000-4000-8000-000000000005'),
('c1000000-0000-4000-8000-000000000016', 'Fotografía Integración con el Río',     'Mural enmarcado por los árboles de samán y el cauce del río Cali',      'obra_006_rio.jpg',            4.85, NULL, '2026-09-03', '80000000-0000-4000-8000-000000000001', 'd1000000-0000-4000-8000-000000000006'),
('c1000000-0000-4000-8000-000000000017', 'Modelo 3D Relieve Escopeta',            'Reconstrucción volumétrica digital de la esquina patrimonial',          'obra_006_entorno.glb',       15.20, NULL, '2026-09-03', '80000000-0000-4000-8000-000000000007', 'd1000000-0000-4000-8000-000000000006'),
('c1000000-0000-4000-8000-000000000018', 'Fotografía Panorámica Graffiti',        'Toma en gran angular del muro completo de 25 metros de longitud',       'obra_007_wildstyle.jpg',      5.90, NULL, '2026-09-04', '80000000-0000-4000-8000-000000000001', 'd1000000-0000-4000-8000-000000000007'),
('c1000000-0000-4000-8000-000000000019', 'Entrevista al Escritor Dexs',           'Historia sobre los inicios de la cultura del spray en los años 90',     'obra_007_dexs.mp3',           7.50,  210, '2026-09-04', '80000000-0000-4000-8000-000000000005', 'd1000000-0000-4000-8000-000000000007'),
('c1000000-0000-4000-8000-000000000020', 'Fotografía Testimonial Minera',         'Encuadre dramático del rostro del minero y textura del muro',           'obra_008_minero.jpg',         4.30, NULL, '2026-09-04', '80000000-0000-4000-8000-000000000001', 'd1000000-0000-4000-8000-000000000008'),
('c1000000-0000-4000-8000-000000000021', 'Paisaje Sonoro de la Mina',             'Ecos y golpes de piqueta tradicionales grabados en socavón antiguo',    'obra_008_mina.mp3',           5.10,  160, '2026-09-04', '80000000-0000-4000-8000-000000000005', 'd1000000-0000-4000-8000-000000000008'),
('c1000000-0000-4000-8000-000000000022', 'Fotografía Luz Crepuscular',            'Toma capturada a las 6:00 pm con iluminación del atardecer',            'obra_009_campesino.jpg',      4.75, NULL, '2026-09-04', '80000000-0000-4000-8000-000000000001', 'd1000000-0000-4000-8000-000000000009'),
('c1000000-0000-4000-8000-000000000023', 'Detalle Ojos y Sombrero',               'Textura al óleo y spray en las arrugas del rostro campesino',           'obra_009_detalle.png',        5.80, NULL, '2026-09-04', '80000000-0000-4000-8000-000000000002', 'd1000000-0000-4000-8000-000000000009'),
('c1000000-0000-4000-8000-000000000024', 'Fotografía Plazoleta Monumento',        'Mural enmarcado con las trompetas gigantes del monumento a Varela',     'obra_010_plazoleta.jpg',      5.25, NULL, '2026-09-05', '80000000-0000-4000-8000-000000000001', 'd1000000-0000-4000-8000-000000000010'),
('c1000000-0000-4000-8000-000000000025', 'Acordes de Trompeta en Vivo',           'Solo de trompeta grabado en la plazoleta en tributo al Grupo Niche',    'obra_010_trompeta.mp3',       6.80,  175, '2026-09-05', '80000000-0000-4000-8000-000000000005', 'd1000000-0000-4000-8000-000000000010'),
('c1000000-0000-4000-8000-000000000026', 'Fotografía Frontal Jaguar',             'Composición simétrica del felino sagrado con paleta en azules',         'obra_011_jaguar.jpg',         5.10, NULL, '2026-09-05', '80000000-0000-4000-8000-000000000001', 'd1000000-0000-4000-8000-000000000011'),
('c1000000-0000-4000-8000-000000000027', 'Modelo 3D Escultura Jaguar',            'Asset digital del jaguar para visualización en tiempo real',            'obra_011_jaguar3d.glb',      16.70, NULL, '2026-09-05', '80000000-0000-4000-8000-000000000007', 'd1000000-0000-4000-8000-000000000011'),
('c1000000-0000-4000-8000-000000000028', 'Fotografía Niños y Cancha',             'Mural interactivo que sirve de fondo a las actividades infantiles',     'obra_012_infantil.jpg',       4.60, NULL, '2026-09-05', '80000000-0000-4000-8000-000000000001', 'd1000000-0000-4000-8000-000000000012'),
('c1000000-0000-4000-8000-000000000029', 'Canto Infantil de Siloé',               'Grabación de rondas y canciones infantiles del semillero de arte',      'obra_012_cantos.mp3',         4.90,  140, '2026-09-05', '80000000-0000-4000-8000-000000000005', 'd1000000-0000-4000-8000-000000000012'),
('c1000000-0000-4000-8000-000000000030', 'Video Resumen Proyecto Comunitario',    'Documental breve sobre la pintura colectiva del mural con la comunidad','obra_012_documental.mp4',    71.20,  240, '2026-09-05', '80000000-0000-4000-8000-000000000004', 'd1000000-0000-4000-8000-000000000012');


-- =============================================================================
-- 15. EJEMPLOS DE INSERT, UPDATE Y DELETE QUE RESPETAN LA INTEGRIDAD REFERENCIAL
-- =============================================================================

-- Ejemplo INSERT: Registrar un artista invitado sin obras para prueba referencial
INSERT INTO `artista` (`idArtista`, `nombreArtistico`, `nombre1Real`, `nombre2Real`, `apellido1Real`, `apellido2Real`, `biografia`, `perfilRedSocial`, `estadoArtista`, `idPaisFK`)
VALUES ('a7000000-0000-4000-8000-000000000007', 'Vertigo Urbano', 'Santiago', NULL, 'Lozano', 'Caicedo', 'Muralista invitado del festival de arte urbano.', 'https://instagram.com/vertigourbano', TRUE, '10000000-0000-4000-8000-000000000001');

-- Ejemplo UPDATE 1: Actualizar el estado de conservación de una obra tras jornada de restauración
UPDATE `obra`
SET `estadoConservacion` = 'Regular'
WHERE `idObra` = 'd1000000-0000-4000-8000-000000000008';

-- Ejemplo UPDATE 2: Actualizar el tiempo de permanencia en una parada de ruta
UPDATE `ruta_obra`
SET `tiempoPermanencia` = 30
WHERE `idRutaObra` = 'b1000000-0000-4000-8000-000000000002';

-- Ejemplo DELETE: Eliminar físicamente al artista temporal que no posee obras asociadas (respeta ON DELETE RESTRICT en obra.idArtistaFK)
DELETE FROM `artista`
WHERE `idArtista` = 'a7000000-0000-4000-8000-000000000007'
  AND NOT EXISTS (SELECT 1 FROM `obra` o WHERE o.`idArtistaFK` = 'a7000000-0000-4000-8000-000000000007');


-- =============================================================================
-- 16. CONSULTAS MULTITABLA CON JOIN, AGREGACIÓN (GROUP BY, COUNT, HAVING) Y VISTAS
-- =============================================================================

-- Consulta 1: Artistas con 2 o más obras registradas y su volumen multimedia (JOIN 4 tablas + GROUP BY + HAVING)
SELECT
    a.`nombreArtistico` AS `artista`,
    CONCAT_WS(' ', a.`nombre1Real`, a.`nombre2Real`, a.`apellido1Real`, a.`apellido2Real`) AS `nombre_real`,
    p.`nombrePais` AS `pais`,
    COUNT(DISTINCT o.`idObra`) AS `total_obras_registradas`,
    COUNT(rm.`idRecurso`) AS `total_archivos_multimedia`,
    ROUND(COALESCE(SUM(rm.`tamanio`), 0), 2) AS `almacenamiento_total_mb`
FROM `artista` a
INNER JOIN `pais` p                     ON a.`idPaisFK`  = p.`idPais`
INNER JOIN `obra` o                     ON a.`idArtista` = o.`idArtistaFK`
LEFT JOIN `recurso_multimedia` rm       ON o.`idObra`    = rm.`idObraFK`
GROUP BY a.`idArtista`, a.`nombreArtistico`, `nombre_real`, p.`nombrePais`
HAVING COUNT(DISTINCT o.`idObra`) >= 2
ORDER BY `total_obras_registradas` DESC, `almacenamiento_total_mb` DESC;

-- Consulta 2: Distribución de recursos multimedia por tipo y formato (JOIN 3 tablas + GROUP BY)
SELECT
    tr.`nombreTipoRecurso` AS `tipo_recurso`,
    fr.`extension` AS `formato`,
    tr.`carpetaBase` AS `directorio_relativo`,
    COUNT(rm.`idRecurso`) AS `cantidad_archivos`,
    ROUND(COALESCE(SUM(rm.`tamanio`), 0), 2) AS `peso_acumulado_mb`
FROM `tipo_recurso` tr
INNER JOIN `formato_recurso` fr         ON tr.`idTipoRecurso` = fr.`idTipoRecursoFK`
INNER JOIN `recurso_multimedia` rm      ON fr.`idFormato`     = rm.`idFormatoFK`
GROUP BY tr.`idTipoRecurso`, tr.`nombreTipoRecurso`, fr.`extension`, tr.`carpetaBase`
ORDER BY `cantidad_archivos` DESC;

-- Consulta 3: Cantidad de obras por categoría técnica (JOIN + GROUP BY + COUNT)
SELECT
    c.`nombreCategoria` AS `categoria`,
    COUNT(DISTINCT o.`idObra`) AS `total_obras`,
    COUNT(rm.`idRecurso`) AS `total_recursos_multimedia`
FROM `categoria` c
LEFT JOIN `obra` o                  ON c.`idCategoria` = o.`idCategoriaFK`
LEFT JOIN `recurso_multimedia` rm   ON o.`idObra`      = rm.`idObraFK`
GROUP BY c.`idCategoria`, c.`nombreCategoria`
ORDER BY `total_obras` DESC;

-- Consulta 4: Verificación de Vista 1 (Catálogo completo de obras - JOIN 9 tablas)
SELECT `titulo_obra`, `artista`, `pais_artista`, `categoria_tecnica`, `barrio`, `comuna`, `estadoConservacion`, `registrado_por`
FROM `vw_catalogo_obras_completo`
ORDER BY `numero_comuna`, `titulo_obra`;

-- Consulta 5: Verificación de Vista 2 (Itinerario ordenado de rutas - JOIN 7 tablas)
SELECT `nombre_ruta`, `orden_visita`, `titulo_obra`, `artista`, `barrio`, `comuna`, `tiempo_permanencia_min`
FROM `vw_rutas_itinerario_detallado`
ORDER BY `nombre_ruta`, `orden_visita`;

-- Consulta 6: Verificación de Vista 3 (Resumen multimedia por obra - JOIN 6 tablas)
SELECT `titulo_obra`, `artista`, `total_recursos`, `cant_imagenes`, `cant_videos`, `cant_audios`, `cant_modelos_3d`, `cant_experiencias_ar`, `tamano_total_mb`
FROM `vw_obras_resumen_multimedia`
ORDER BY `tamano_total_mb` DESC;

-- Consulta 7: Verificación de Vista 4 (Estadísticas por comuna - JOIN 6 tablas)
SELECT * FROM `vw_estadisticas_culturales_comuna`
ORDER BY `total_obras` DESC;
