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
  * [`ConexionMysql.cs`](./Etapa_6_Aplicacion/GaleriaArteUrbanoCali/models/ConexionMysql.cs): Administra la conexión a MySQL con `MySqlConnectionStringBuilder`, ejecución de consultas parametrizadas (`@param`) y resolución de rutas relativas hacia `RecursosMultimedia/`.
  * [`SeguridadHash.cs`](./Etapa_6_Aplicacion/GaleriaArteUrbanoCali/models/SeguridadHash.cs): Cifrado y verificación de contraseñas con **SHA-256** (`System.Security.Cryptography`).
  * Clases de las **14 tablas del modelo en 3FN**: `Pais.cs`, `Artista.cs`, `TipoUsuario.cs`, `Usuario.cs`, `Comuna.cs`, `Barrio.cs`, `Ubicacion.cs`, `Categoria.cs`, `Obra.cs`, `Ruta.cs`, `RutaObra.cs`, `TipoRecurso.cs`, `FormatoRecurso.cs`, `RecursoMultimedia.cs`.
  * [`ConsultasReportes.cs`](./Etapa_6_Aplicacion/GaleriaArteUrbanoCali/models/ConsultasReportes.cs): Consultas multitabla con `INNER JOIN` y `LEFT JOIN`, vistas y agregaciones (`GROUP BY`, `COUNT`, `SUM`, `HAVING`).
* **`controllers/` (Reglas de Negocio y Validación):**
  * `ControllerUsuario.cs`, `ControllerArtista.cs`, `ControllerCategoria.cs`, `ControllerUbicacion.cs`, `ControllerObra.cs`, `ControllerRuta.cs`, `ControllerRecursoMultimedia.cs`, `ControllerConsultas.cs`.
* **`views/` (Interfaz Gráfica Windows Forms):**
  * `UI_Login.cs`, `FormPrincipal.cs`, `UI_GestionArtistas.cs`, `UI_GestionCategorias.cs`, `UI_GestionUbicaciones.cs`, `UI_GestionObras.cs`, `UI_GestionMultimedia.cs`, `UI_GestionRutas.cs`, `UI_ConsultasYEstadisticas.cs`.

---

## 3. Orden de Ejecución del Script SQL

En **MySQL Workbench** (conectado a una instancia local de MySQL 8.0):

1. **Opción recomendada (Script unificado):**
   * Abrir y ejecutar completo **`Galeria_Arte_Urbano_Cali.sql`** (ubicado en `Etapa_5_MySQL/Galeria_Arte_Urbano_Cali.sql` y en `Entrega_Final/Galeria_Arte_Urbano_Cali.sql`).
2. **Opción por scripts separados (en `Etapa_5_MySQL/`):**
   * **Primero:** `01_estructura.sql` (crea la base de datos `galeria_arte_urbano_db`, las 14 tablas en singular en 3FN, restricciones `PRIMARY KEY`, `FOREIGN KEY`, `UNIQUE`, `CHECK`, `DEFAULT`, los 3 índices justificados, las 4 vistas y los 4 procedimientos almacenados).
   * **Segundo:** `02_datos_prueba.sql` (inserta los catálogos base, 6 artistas, 5 categorías, 12 obras, 3 rutas con 12 paradas, 30 recursos multimedia, ejemplos de `INSERT`, `UPDATE` y `DELETE` y consultas de verificación).

---

## 4. Configuración de la Cadena de Conexión y Ejecución en C#

La cadena de conexión se configura en `models/ConexionMysql.cs` con los parámetros por defecto de MySQL local:
* **Servidor:** `localhost`
* **Puerto:** `3306`
* **Base de datos:** `galeria_arte_urbano_db`
* **Usuario:** `root`
* **Contraseña:** `root`

Para ejecutar la aplicación:
1. **Ejecución directa sin compilar:** Abrir la carpeta `Ejecutable/` y ejecutar **`GaleriaArteUrbanoCali.exe`**.
2. **Desde Visual Studio 2022:** Abrir `GaleriaArteUrbanoCali.sln` (o `GaleriaArteUrbanoCali.csproj`) y presionar **F5**.
3. Iniciar sesión con cualquiera de las cuentas registradas:
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

## 7. Distribución del Desarrollo en el Equipo

* **Juan Pablo Campo Bertin (`2235544`):**
  * Diseño del Modelo Relacional de Datos (`Galeria_Arte_Urbano_MRD.xlsx`) y verificación de normalización hasta 3FN sobre las 14 entidades del modelo.
  * Implementación del script SQL en MySQL 8.0 (`Galeria_Arte_Urbano_Cali.sql`), incluyendo restricciones de integridad, vistas, procedimientos almacenados, índices y carga de datos de prueba.
  * Construcción de la capa `models/` en C# (`ConexionMysql.cs`, `SeguridadHash.cs`, clases de entidades y `ConsultasReportes.cs`).
* **Jean Deiby García:**
  * Implementación de la capa de negocio y validaciones en `controllers/` (`ControllerUsuario`, `ControllerArtista`, `ControllerCategoria`, `ControllerUbicacion`, `ControllerObra`, `ControllerRuta`, `ControllerRecursoMultimedia`, `ControllerConsultas`).
  * Diseño y programación de los formularios en `views/` (inicio de sesión por roles, módulos CRUD, visor y reproductor multimedia con rutas relativas y módulo de consultas y estadísticas).
  * Pruebas funcionales e integración con los recursos de la carpeta `RecursosMultimedia/`.