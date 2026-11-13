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
