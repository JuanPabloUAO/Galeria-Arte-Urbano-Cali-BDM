# Galería de Arte Urbano de Cali

**Asignatura:** Bases de Datos Multimedia (BDM) — Grupo 51 (2026-2)  
**Universidad Autónoma de Occidente (UAO)** — Facultad de Ingeniería  
**Docente:** David Alejandro Castro Benavides  

### Integrantes
* **Juan Pablo Campo Bertin** — Código: `2235544`
* **Jean Deiby García**

---

## 1. Descripción General
Aplicación de escritorio en **C# Windows Forms (.NET 10)** conectada a **MySQL 8.0** para el registro, organización y consulta de obras de arte urbano en Santiago de Cali, sus artistas, categorías técnicas, ubicaciones georreferenciadas (comuna y barrio), rutas culturales (`ruta_obra`) y recursos multimedia asociados.

Los archivos físicos (imágenes, videos, audios, modelos 3D y referencias AR) se almacenan en el sistema de archivos dentro de la carpeta `RecursosMultimedia/`, mientras que MySQL guarda únicamente sus metadatos y la ruta relativa correspondiente.

---

## 2. Arquitectura por Capas del Proyecto C#

El proyecto `GaleriaArteUrbanoCali` está organizado en tres capas siguiendo la estructura trabajada en clase:

* **`models/` (Entidades y Acceso a Datos):**
  * `ConexionMysql.cs`: Administra la conexión a MySQL con `MySqlConnectionStringBuilder`, ejecución de consultas parametrizadas (`@param`) y resolución de rutas relativas hacia `RecursosMultimedia/`.
  * `SeguridadHash.cs`: Cifrado y verificación de contraseñas con **SHA-256** (`System.Security.Cryptography`).
  * Clases de las **14 tablas del modelo en 3FN**: `Pais.cs`, `Artista.cs`, `TipoUsuario.cs`, `Usuario.cs`, `Comuna.cs`, `Barrio.cs`, `Ubicacion.cs`, `Categoria.cs`, `Obra.cs`, `Ruta.cs`, `RutaObra.cs`, `TipoRecurso.cs`, `FormatoRecurso.cs`, `RecursoMultimedia.cs`.
  * `ConsultasReportes.cs`: Consultas multitabla con `INNER JOIN` y `LEFT JOIN`, vistas y agregaciones (`GROUP BY`, `COUNT`, `SUM`, `HAVING`).
* **`controllers/` (Reglas de Negocio y Validación):**
  * `ControllerUsuario.cs`, `ControllerArtista.cs`, `ControllerCategoria.cs`, `ControllerUbicacion.cs`, `ControllerObra.cs`, `ControllerRuta.cs`, `ControllerRecursoMultimedia.cs`, `ControllerConsultas.cs`.
* **`views/` (Interfaz Gráfica Windows Forms):**
  * `UI_Login.cs`, `FormPrincipal.cs`, `UI_GestionArtistas.cs`, `UI_GestionCategorias.cs`, `UI_GestionUbicaciones.cs`, `UI_GestionObras.cs`, `UI_GestionMultimedia.cs`, `UI_GestionRutas.cs`, `UI_ConsultasYEstadisticas.cs`.

---

## 3. Orden de Ejecución del Script SQL

En **MySQL Workbench** (conectado a una instancia local de MySQL 8.0):

1. Si se usa el archivo unificado:
   * Abrir y ejecutar completo **`Galeria_Arte_Urbano_Cali.sql`**.
2. Si se usan los dos scripts separados, ejecutarlos en este orden:
   * **Primero:** `galeria_arte_urbano_schema.sql` (crea la base de datos `galeria_arte_urbano_db`, las 14 tablas en singular en 3FN, restricciones `PRIMARY KEY`, `FOREIGN KEY`, `UNIQUE`, `CHECK`, `DEFAULT`, los 3 índices justificados, las 4 vistas y los 4 procedimientos almacenados).
   * **Segundo:** `galeria_arte_urbano_seed.sql` (inserta los catálogos base, 6 artistas, 5 categorías, 12 obras, 3 rutas con 12 paradas, 30 recursos multimedia, ejemplos de `INSERT`, `UPDATE` y `DELETE` y consultas de verificación).

---

## 4. Configuración de la Cadena de Conexión y Ejecución en C#

La cadena de conexión se configura en [`models/ConexionMysql.cs`](./models/ConexionMysql.cs) con los parámetros por defecto de MySQL local:
* **Servidor:** `localhost`
* **Puerto:** `3306`
* **Base de datos:** `galeria_arte_urbano_db`
* **Usuario:** `root`
* **Contraseña:** `root`

Para compilar y ejecutar la aplicación:
1. Abrir `GaleriaArteUrbanoCali.csproj` en **Visual Studio 2022** y presionar **F5** (o ejecutar `dotnet run --project GaleriaArteUrbanoCali.csproj`).
2. Iniciar sesión con cualquiera de las cuentas de prueba:
   * **Perfil Administrador (CRUD completo):**
     * Correo: `juan.campo@uao.edu.co` o `jean.garcia@uao.edu.co`
     * Contraseña: `Cali2026*`
   * **Perfil Visitante (Modo solo consulta):**
     * Correo: `david.castro@talentum.edu.co`
     * Contraseña: `Cali2026*`

---

## 5. Estructura de la Carpeta `RecursosMultimedia`

Ubicada en la raíz del proyecto, organizada por tipo de recurso (`tipo_recurso.carpetaBase`):

```text
RecursosMultimedia/
├── Imagenes/      (.jpg, .png, .webp)
├── Videos/        (.mp4)
├── Audios/        (.mp3, .wav)
├── Modelos3D/     (.glb)
└── AR/            (.usdz)
```

En la tabla `recurso_multimedia` se almacena únicamente el nombre relativo del archivo (`rutaArchivo`, por ejemplo `obra_001_frontal.jpg`), que al unirse con `tipo_recurso.carpetaBase` (`RecursosMultimedia/Imagenes/`) forma la ruta relativa completa dentro del proyecto.

---

## 6. Justificación de Índices, Vistas y Procedimientos Almacenados

* **Índices adicionales:**
  * `idx_obra_categoria_estado` sobre `obra(idCategoriaFK, estadoConservacion)`: acelera el filtrado de obras por categoría técnica y estado de conservación.
  * `idx_recurso_obra_formato` sobre `recurso_multimedia(idObraFK, idFormatoFK)`: optimiza el `JOIN` entre una obra, sus archivos multimedia y sus formatos al cargar la galería o el resumen multimedia.
  * `idx_ubicacion_coordenadas` sobre `ubicacion(latitud, longitud)`: optimiza búsquedas por rango geográfico dentro del perímetro de Cali.
* **Vistas invocadas desde C#:**
  * `vw_catalogo_obras_completo` (une 9 tablas: `obra`, `artista`, `pais`, `categoria`, `ubicacion`, `barrio`, `comuna`, `usuario`, `tipo_usuario`).
  * `vw_rutas_itinerario_detallado` (une 7 tablas para el recorrido ordenado de cada ruta).
  * `vw_obras_resumen_multimedia` y `vw_estadisticas_culturales_comuna`.
* **Procedimientos almacenados invocados desde C# (`CommandType.StoredProcedure`):**
  * `sp_registrar_obra` (invocado en `Obra.InsertarObra`).
  * `sp_asociar_recurso_multimedia` (invocado en `RecursoMultimedia.InsertarRecurso`).
  * `sp_asignar_obra_a_ruta` (invocado en `RutaObra.AgregarObraARuta`).

---

## 7. Declaración de Uso y Verificación de Herramientas de Apoyo (IA)

De acuerdo con los lineamientos de la rúbrica del curso, se declara que se utilizó asistencia de IA como apoyo para:
1. La generación de archivos multimedia sintéticos de prueba (imágenes, clips de audio/video y mallas `.glb` de demostración en `RecursosMultimedia/`) y redacción de descripciones culturales de las obras de prueba en el script de carga inicial (`galeria_arte_urbano_seed.sql`).
2. Revisión de sintaxis en restricciones `CHECK` de MySQL 8.0 y formateo de controles en formularios Windows Forms.

**Verificación realizada por el equipo:** Todas las tablas, llaves primarias/foráneas, reglas `ON DELETE / ON UPDATE`, vistas, procedimientos almacenados y consultas parametrizadas fueron contrastadas manualmente contra el MER oficial de 14 entidades del curso, ejecutadas en MySQL Workbench 8.0 y probadas extremo a extremo desde la aplicación en Visual Studio.
