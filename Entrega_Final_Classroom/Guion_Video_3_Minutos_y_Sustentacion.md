# Guion de Video (Máximo 3 Minutos) y Guía Rápida de Sustentación en Vivo

**Proyecto Final:** Galería de Arte Urbano de Cali (BDM — Grupo 51)  
**Integrantes:** Juan Pablo Campo Bertin (`2235544`) y Jean Deiby García  

---

## Parte 1: Guion Cronometrado para el Video (2:50 min en total)

### Bloque 1 — Juan Pablo Campo (0:00 a 1:25) — MER, MRD, MySQL 8.0 y Acceso a Datos
1. **(0:00 – 0:25) Modelo de Datos (14 Tablas en 3FN):**
   * Mostrar en pantalla `Galeria_Arte_Urbano_Cali_MER.jpg` y `Galeria_Arte_Urbano_MRD.xlsx`.
   * *"Hola profesor David, somos Juan Pablo Campo y Jean Deiby García. Nuestro proyecto implementa las 14 tablas en singular del MER oficial en Tercera Forma Normal: separamos catálogos como `pais`, `tipo_usuario`, `comuna` -> `barrio` -> `ubicacion`, y `tipo_recurso` -> `formato_recurso` para eliminar dependencias transitivas. La relación muchos a muchos entre `ruta` y `obra` se resuelve en `ruta_obra` con llave primaria propia `CHAR(36)` y dos restricciones `UNIQUE` compuestas: `(idRutaFK, idObraFK)` para no repetir una obra en la misma ruta y `(idRutaFK, ordenVisita)` para no repetir el orden de recorrido."*
2. **(0:25 – 0:55) Script MySQL 8.0 (`Galeria_Arte_Urbano_Cali.sql`):**
   * Mostrar el script en MySQL Workbench.
   * *"En MySQL 8 implementamos las 14 tablas con llaves `CHAR(36)` UUID, restricciones `CHECK` para validar las coordenadas de Cali (`latitud` entre 3.25 y 3.55, `longitud` entre -76.60 y -76.45) y valores positivos, además de reglas `ON DELETE RESTRICT` (por ejemplo, un artista con obras no se borra físicamente, se desactiva con `estadoArtista = FALSE`) y `ON DELETE CASCADE` en las paradas de una ruta. También creamos 3 índices justificados, 4 vistas con `JOIN` y procedimientos almacenados como `sp_registrar_obra` y `sp_asignar_obra_a_ruta`."*
3. **(0:55 – 1:25) Capa de Modelos y Seguridad en C# (`models/`):**
   * Mostrar en Visual Studio `ConexionMysql.cs`, `SeguridadHash.cs` y `Obra.cs`.
   * *"Yo desarrollé el script SQL, el MRD y la capa de acceso a datos en `models/`. En `ConexionMysql.cs` centralizamos la conexión y garantizamos que todas las consultas usen parámetros `@param` para evitar inyección SQL. En `SeguridadHash.cs` ciframos las contraseñas con SHA-256, y en `Obra.cs`, `RecursoMultimedia.cs` y `RutaObra.cs` invocamos directamente los procedimientos almacenados de MySQL usando `CommandType.StoredProcedure`."*

---

### Bloque 2 — Jean Deiby García (1:25 a 2:50) — Controladores, Interfaz WinForms, Multimedia y Consultas JOIN
1. **(1:25 – 1:55) Login por Roles y Módulos CRUD (`controllers/` y `views/`):**
   * Mostrar la aplicación corriendo: `UI_Login` -> `FormPrincipal` -> `UI_GestionArtistas` -> `UI_GestionObras`.
   * *"Yo desarrollé las validaciones en la capa `controllers/` y los formularios en `views/`. En el login validamos el correo y el hash SHA-256 diferenciando el rol Administrador (que tiene acceso a crear, modificar y desactivar) del rol Visitante (modo solo consulta). En Artistas, si intentamos eliminar un artista que ya tiene obras como Guache o Gleo, el sistema aplica la regla de negocio y lo desactiva lógicamente sin borrar sus obras. En Obras seleccionamos Artista, Categoría y Ubicación desde listas desplegables y guardamos automáticamente el `idUsuarioFK` de la sesión activa."*
2. **(1:55 – 2:20) Gestión de Archivos Multimedia y Rutas (`UI_GestionMultimedia` y `UI_GestionRutas`):**
   * Mostrar la carga/previsualización de una imagen en `UI_GestionMultimedia` y las paradas en `UI_GestionRutas`.
   * *"En el módulo Multimedia seleccionamos un archivo del computador, validamos su extensión contra `formato_recurso`, lo copiamos automáticamente a la subcarpeta de `RecursosMultimedia/` y guardamos en MySQL únicamente la ruta relativa, nunca una ruta absoluta. Desde aquí previsualizamos imágenes y reproducimos audio o video. En Rutas agregamos obras validando que no se repita ni la obra ni el orden de visita."*
3. **(2:20 – 2:50) Consultas Multitabla con JOIN y Agregaciones (`UI_ConsultasYEstadisticas`):**
   * Hacer clic en las consultas del centro de consultas.
   * *"Finalmente, en el módulo de Consultas ejecutamos las consultas con `INNER JOIN` y `LEFT JOIN`: la ficha completa de obras uniendo 9 tablas mediante la vista `vw_catalogo_obras_completo`, obras filtradas por artista, categoría y ubicación (`ubicacion`–`barrio`–`comuna`), el itinerario de rutas con `ORDER BY orden_visita`, los recursos multimedia con su formato y tipo, y las 3 consultas de agregación con `GROUP BY`, `COUNT` y `HAVING` (obras por comuna, por categoría y artistas con 2 o más obras)."*

---

## Parte 2: Respuestas Rápidas para Preguntas de Sustentación en Vivo (Rúbrica)

1. **¿Cuándo usaron `INNER JOIN` y cuándo `LEFT JOIN` en las consultas?**
   * **Respuesta:** Usamos `INNER JOIN` cuando la relación es obligatoria (`NOT NULL`), por ejemplo entre `obra`, `artista`, `categoria` y `ubicacion` (porque toda obra tiene obligatoriamente un artista, una categoría y una ubicación). Usamos `LEFT JOIN` cuando queremos listar entidades padre aunque todavía no tengan registros hijos asociados, por ejemplo entre `obra` y `recurso_multimedia` (o `categoria` y `obra` en las estadísticas), para que una obra sin archivos multimedia o una categoría nueva aparezca con conteo `0` en lugar de desaparecer del resultado.
2. **¿Cómo modificar en vivo una consulta si el profesor lo pide?**
   * **Ubicación en el código:** Abrir [`models/ConsultasReportes.cs`](file:///C:/Users/Pablito/Documents/Bases%20de%20Datos/08_Proyecto_Semestre_Galeria_Arte_Urbano/GaleriaArteUrbanoCali/models/ConsultasReportes.cs). Todas las consultas están numeradas del 1 al 8 en ese único archivo con SQL limpio y parametrizado.
   * Ejemplo: Si pide agregar un filtro por estado de conservación en `ConsultarObrasPorArtista` (línea 34), basta agregar `AND o.estadoConservacion = @estado` en el `WHERE` y `{"@estado", "Excelente"}` en el diccionario `p`.
3. **¿Qué consulta mejora el índice creado?**
   * El índice compuesto `idx_recurso_obra_formato` sobre `recurso_multimedia(idObraFK, idFormatoFK)` evita un escaneo completo (`FULL TABLE SCAN`) de los archivos multimedia cuando se filtra por `idObraFK` en la Consulta 6 (`ConsultarRecursosPorObra`) y en la vista `vw_obras_resumen_multimedia`.
