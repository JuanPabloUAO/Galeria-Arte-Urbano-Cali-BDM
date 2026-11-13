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
