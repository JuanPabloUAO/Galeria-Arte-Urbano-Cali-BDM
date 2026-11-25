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
