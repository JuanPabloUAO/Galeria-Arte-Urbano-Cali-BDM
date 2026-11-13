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
