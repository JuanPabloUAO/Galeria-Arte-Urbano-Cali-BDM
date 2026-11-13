# Galería de Arte Urbano de Cali
## Documento Técnico — Etapa 1: Análisis del Problema y Modelado Conceptual

**Asignatura:** Bases de Datos Multimedia (BDM)  
**Institución:** Universidad Autónoma de Occidente (UAO) — Facultad de Ingeniería  
**Docente:** David Alejandro Castro Benavides  
**Integrantes del Grupo:**
* Juan Pablo Campo Bertin
* Jean Deiby Garcia  
**Versión:** 1.0  
**Fecha:** Septiembre de 2026  

---

## 1. Introducción y Descripción del Escenario

La ciudad de Santiago de Cali se distingue a nivel nacional e internacional por su rica manifestación de arte urbano y cultura gráfica, reflejada en murales de gran formato, grafitis, intervenciones con técnica stencil, cartelería y arte tipográfico distribuidos a lo largo de sus 22 comunas y corredores emblemáticos (como la Calle de la Escopeta, el Bulevar del Río, San Antonio y la Avenida Colombia).

A pesar de su valor cultural y turístico, estas expresiones artísticas carecen de una plataforma centralizada que preserve su memoria visual y facilite su difusión. Muchas obras sufren deterioro por intemperie, vandalismo o intervenciones urbanísticas, perdiéndose el registro de sus autores y su significado.

### Propósito del Sistema
El presente proyecto tiene como objetivo diseñar e implementar un sistema de información de escritorio desarrollado en **C# (.NET)** conectado a una base de datos relacional en **MySQL 8**, que permita:
1. **Catalogar y registrar** obras de arte urbano con información detallada de sus autores (artistas), georreferenciación precisa en Cali y clasificación técnica/temática.
2. **Gestionar un repositorio multimedia enriquecido** que acompañe a cada obra mediante fotografías en alta definición, videos de proceso, audios descriptivos o testimoniales, modelos 3D (`.glb`) y enlaces a experiencias de realidad aumentada (AR).
3. **Organizar rutas turísticas y culturales**, permitiendo diseñar trayectos guiados por sectores de la ciudad con un orden de visita lógico y recomendaciones de recorrido.
4. **Garantizar la persistencia eficiente de archivos multimedia**, separando los metadatos relacionales (almacenados en MySQL) de los archivos binarios pesados (almacenados en una estructura de carpetas relativas del sistema de archivos).

---

## 2. Catálogo Detallado de Entidades y Atributos

De acuerdo con los requerimientos establecidos por el docente, el sistema se estructura a partir de ocho (8) entidades nucleares. A continuación se desglosa la definición conceptual de cada una:

### 2.1. Entidad `USUARIO`
Representa a las personas con credenciales de acceso a la aplicación de escritorio. Define los niveles de autorización y permisos operativos.

* **Consideraciones de diseño:** El correo electrónico es único en el sistema. Por seguridad estricta, la contraseña **nunca se almacena en texto plano**, sino mediante un algoritmo de hash criptográfico unidireccional (ej. SHA-256 o BCrypt con sal).

| Atributo | Tipo Conceptual | Obligatoriedad | Clave / Restricción | Descripción |
|---|---|---|---|---|
| `id_usuario` | UUID / String(36) | Requerido | **PK** | Identificador universal único del usuario. |
| `nombre` | Varchar(50) | Requerido | Ninguna | Primer y segundo nombre del usuario. |
| `apellido` | Varchar(50) | Requerido | Ninguna | Apellidos del usuario. |
| `correo` | Varchar(100) | Requerido | **UNIQUE** | Correo institucional o personal válido para autenticación. |
| `password_hash`| Varchar(255) | Requerido | Ninguna | Contraseña cifrada mediante función hash. |
| `tipo_usuario` | Enum / Varchar(20) | Requerido | **CHECK** | Rol del usuario: `'ADMINISTRADOR'` (gestión total) o `'VISITANTE'` (solo consulta). |
| `fecha_registro`| Datetime | Requerido | Default Current | Marca de tiempo del registro en el sistema. |
| `estado` | Boolean / Enum | Requerido | Default Activo | Estado de la cuenta (`'ACTIVO'`, `'INACTIVO'`). |

---

### 2.2. Entidad `ARTISTA`
Representa a los creadores individuales o colectivos artísticos responsables de las intervenciones urbanas.

* **Consideraciones de diseño:** Un artista puede ser autor de múltiples obras. Si un artista tiene obras registradas en el sistema, **no se permite su eliminación física** para evitar la orfandad de registros históricos; en su lugar, se implementa borrado lógico mediante el atributo `estado`.

| Atributo | Tipo Conceptual | Obligatoriedad | Clave / Restricción | Descripción |
|---|---|---|---|---|
| `id_artista` | UUID / String(36) | Requerido | **PK** | Identificador universal único del artista. |
| `nombre_artistico` | Varchar(100) | Requerido | **UNIQUE** | Tag, seudónimo o nombre público del artista/colectivo. |
| `nombre_real` | Varchar(100) | Opcional | Ninguna | Nombre y apellidos reales del creador (si aplica/desea revelarlo). |
| `biografia` | Text | Opcional | Ninguna | Reseña biográfica, trayectoria y estilo artístico. |
| `pais_origen` | Varchar(50) | Requerido | Ninguna | País de procedencia del artista (fomenta análisis de artistas locales e invitados). |
| `red_social` | Varchar(150) | Opcional | Ninguna | Enlace o usuario a su portafolio o perfil (Instagram, Behance, etc.). |
| `estado` | Enum / Varchar(20) | Requerido | Default Activo | Control de borrado lógico: `'ACTIVO'`, `'INACTIVO'`. |

---

### 2.3. Entidad `UBICACION`
Representa el punto físico y geográfico exacto de la ciudad de Cali donde se localiza una obra de arte.

* **Consideraciones de diseño:** Se incluyen coordenadas geográficas (`latitud` y `longitud`) con validación de rangos mediante restricciones `CHECK`, garantizando que las obras se ubiquen dentro de los límites del perímetro urbano y rural de Cali (Latitud aproximada entre `3.25` y `3.55` Norte; Longitud entre `-76.60` y `-76.45` Oeste).

| Atributo | Tipo Conceptual | Obligatoriedad | Clave / Restricción | Descripción |
|---|---|---|---|---|
| `id_ubicacion` | UUID / String(36) | Requerido | **PK** | Identificador único de la ubicación. |
| `nombre_lugar` | Varchar(100) | Requerido | Ninguna | Nombre representativo del sector o hito urbano (ej. "Muro Bulevar del Río"). |
| `direccion` | Varchar(120) | Requerido | Ninguna | Nomenclatura urbana (ej. "Carrera 1 # 11-05"). |
| `barrio` | Varchar(60) | Requerido | Ninguna | Nombre del barrio (ej. "San Antonio", "Centenario", "El Peñón"). |
| `comuna` | Varchar(20) | Requerido | **CHECK** | Comuna a la que pertenece (Comuna 1 a Comuna 22, o Corregimiento). |
| `latitud` | Decimal(10, 8) | Requerido | **CHECK** | Coordenada de latitud en grados decimales. |
| `longitud` | Decimal(11, 8) | Requerido | **CHECK** | Coordenada de longitud en grados decimales. |

---

### 2.4. Entidad `CATEGORIA`
Define la clasificación temática o técnica de las manifestaciones artísticas registradas.

* **Consideraciones de diseño:** Nombre único para evitar duplicidad de criterios taxonómicos (ejemplos: *Muralismo*, *Grafiti Wildstyle*, *Stencil*, *Retrato*, *Arte Tipográfico / Lettering*, *Instalación urbana*).

| Atributo | Tipo Conceptual | Obligatoriedad | Clave / Restricción | Descripción |
|---|---|---|---|---|
| `id_categoria` | UUID / String(36) | Requerido | **PK** | Identificador único de la categoría. |
| `nombre` | Varchar(50) | Requerido | **UNIQUE** | Denominación técnica de la categoría. |
| `descripcion` | Text | Opcional | Ninguna | Explicación de las características de la técnica o temática. |
| `estado` | Enum / Varchar(20) | Requerido | Default Activo | Permite habilitar o inhabilitar la categoría en formularios. |

---

### 2.5. Entidad `OBRA`
Es la entidad central del sistema. Representa cada intervención artística tangible ubicada en el espacio público.

* **Consideraciones de diseño:**
  - Relación con `ARTISTA`: De acuerdo con la directriz del docente, se modela como **1:N** (cada obra tiene un artista principal responsable).
  - Relaciones foráneas: Depende directamente de `ARTISTA`, `CATEGORIA`, `UBICACION` y del `USUARIO` que realizó el registro en la plataforma para trazabilidad y auditoría.

| Atributo | Tipo Conceptual | Obligatoriedad | Clave / Restricción | Descripción |
|---|---|---|---|---|
| `id_obra` | UUID / String(36) | Requerido | **PK** | Identificador universal único de la obra. |
| `nombre` | Varchar(120) | Requerido | Ninguna | Título o nombre de la obra (o "Sin título" si no posee). |
| `descripcion` | Text | Requerido | Ninguna | Reseña conceptual, mensaje cultural o temática representada. |
| `fecha_creacion`| Date | Opcional | Ninguna | Fecha estimada de elaboración de la obra por el artista. |
| `estado_conservacion` | Enum / Varchar(30) | Requerido | **CHECK** | Estado físico: `'EXCELENTE'`, `'BUENO'`, `'DETERIORADO'`, `'DESTRUIDO'`. |
| `fecha_registro`| Datetime | Requerido | Default Current | Fecha y hora en la que fue ingresada al sistema. |
| `id_artistaFK` | UUID / String(36) | Requerido | **FK** | Referencia al artista creador principal. |
| `id_categoriaFK`| UUID / String(36) | Requerido | **FK** | Referencia a la técnica o categoría artística. |
| `id_ubicacionFK`| UUID / String(36) | Requerido | **FK** | Referencia al lugar físico donde está emplazada. |
| `id_usuarioFK` | UUID / String(36) | Requerido | **FK** | Referencia al usuario que ingresó el registro. |

---

### 2.6. Entidad `RECURSO_MULTIMEDIA`
Almacena la información descriptiva y la ubicación en disco de los activos digitales asociados a una obra.

* **Consideraciones de diseño:**
  - **No almacena BLOBs:** La columna `ruta_archivo` guarda una ruta **relativa** al directorio raíz de la aplicación (ej. `RecursosMultimedia/Imagenes/obra_001.jpg`).
  - Formatos y tipos controlados mediante validación y dominios restringidos.

| Atributo | Tipo Conceptual | Obligatoriedad | Clave / Restricción | Descripción |
|---|---|---|---|---|
| `id_recurso` | UUID / String(36) | Requerido | **PK** | Identificador único del recurso multimedia. |
| `nombre` | Varchar(100) | Requerido | Ninguna | Nombre descriptivo del archivo (ej. "Fotografía frontal diurna"). |
| `tipo_recurso` | Enum / Varchar(20) | Requerido | **CHECK** | Naturaleza: `'IMAGEN'`, `'VIDEO'`, `'AUDIO'`, `'MODELO_3D'`, `'AR'`. |
| `descripcion` | Text | Opcional | Ninguna | Notas adicionales sobre la captura o perspectiva. |
| `ruta_archivo` | Varchar(255) | Requerido | Ninguna | Ruta relativa dentro de la carpeta `RecursosMultimedia/`. |
| `formato` | Varchar(10) | Requerido | **CHECK** | Extensión estandarizada: `'JPG'`, `'PNG'`, `'MP4'`, `'MP3'`, `'GLB'`. |
| `tamano_bytes` | BigInt | Requerido | **CHECK > 0** | Tamaño del archivo en bytes para control de almacenamiento. |
| `duracion_segundos`| Int | Opcional | Ninguna | Duración en segundos (aplica para audio y video; NULL en imágenes/3D). |
| `fecha_registro`| Datetime | Requerido | Default Current | Momento de carga del archivo. |
| `id_obraFK` | UUID / String(36) | Requerido | **FK** | Referencia a la obra a la que pertenece el recurso. |

---

### 2.7. Entidad `RUTA`
Representa un circuito o recorrido temático predefinido que conecta diferentes obras de arte urbano en la ciudad.

* **Consideraciones de diseño:** Permite a los visitantes realizar tours autoguiados con estimaciones de tiempo y distancia.

| Atributo | Tipo Conceptual | Obligatoriedad | Clave / Restricción | Descripción |
|---|---|---|---|---|
| `id_ruta` | UUID / String(36) | Requerido | **PK** | Identificador universal único de la ruta. |
| `nombre` | Varchar(100) | Requerido | **UNIQUE** | Nombre comercial o cultural (ej. "Ruta Histórica de San Antonio"). |
| `descripcion` | Text | Requerido | Ninguna | Detalle de los atractivos del recorrido y público objetivo. |
| `duracion_estimada_min` | Int | Requerido | **CHECK > 0** | Tiempo sugerido en minutos para completar el trayecto. |
| `distancia_estimada_km` | Decimal(5, 2) | Requerido | **CHECK > 0** | Longitud total del circuito en kilómetros. |
| `estado` | Enum / Varchar(20) | Requerido | Default Activo | Control de habilitación: `'ACTIVO'`, `'INACTIVO'`, `'EN_MANTENIMIENTO'`. |

---

### 2.8. Entidad Asociativa `RUTA_OBRA`
Resuelve la relación muchos a muchos (**N:M**) entre `RUTA` y `OBRA`. Una ruta contiene múltiples obras y una obra puede pertenecer a más de una ruta.

* **Consideraciones de diseño:**
  - **Condición obligatoria del docente:** No puede reducirse a una llave foránea simple porque almacena información inherente a la relación.
  - **Restricciones de unicidad:**
    1. Una obra no puede repetirse dentro de la misma ruta: `UNIQUE (id_rutaFK, id_obraFK)`.
    2. El orden de visita debe ser estrictamente secuencial y sin duplicados en la misma ruta: `UNIQUE (id_rutaFK, orden_visita)`.

| Atributo | Tipo Conceptual | Obligatoriedad | Clave / Restricción | Descripción |
|---|---|---|---|---|
| `id_ruta_obra` | UUID / String(36) | Requerido | **PK** | Identificador único del hito o parada en la ruta. |
| `id_rutaFK` | UUID / String(36) | Requerido | **FK** | Referencia a la ruta a la que pertenece el punto. |
| `id_obraFK` | UUID / String(36) | Requerido | **FK** | Referencia a la obra que se visita en este punto. |
| `orden_visita` | Int | Requerido | **CHECK > 0** | Posición numérica en el recorrido (1 = punto de partida, 2, 3...). |
| `tiempo_estimado_min` | Int | Requerido | **CHECK > 0** | Tiempo sugerido de permanencia frente a la obra (en minutos). |
| `recomendacion` | Text | Opcional | Ninguna | Consejos para el visitante (ej. "Mejor iluminación por la tarde", "Acceso peatonal"). |

---

## 3. Matriz de Relaciones y Cardinalidades

A continuación se detalla la semántica de cada vínculo del modelo, especificando dirección de lectura, verbo formal y cardinalidad en notación `(mínimo, máximo)`:

```
[USUARIO] (1,1) -------- registra --------> (0,N) [OBRA]
[ARTISTA] (1,1) -------- realiza ---------> (0,N) [OBRA]
[CATEGORIA] (1,1) ------ clasifica -------> (0,N) [OBRA]
[UBICACION] (1,1) ------ contiene --------> (0,N) [OBRA]
[OBRA] (1,1) ----------- posee -----------> (0,N) [RECURSO_MULTIMEDIA]
[RUTA] (1,1) ----------- compone ---------> (1,N) [RUTA_OBRA]
[OBRA] (1,1) ----------- participa en ----> (0,N) [RUTA_OBRA]
```

### Tabla Resumen de Cardinalidades

| Entidad Origen | Cardinalidad Origen | Verbo Formal | Entidad Destino | Cardinalidad Destino | Justificación Semántica |
|---|:---:|:---:|---|:---:|---|
| **ARTISTA** | `(1,1)` | Realiza | **OBRA** | `(0,N)` | Un artista puede registrar cero o muchas obras; cada obra tiene asignado obligatoriamente un único artista principal (restricción simplificadora del curso). |
| **CATEGORIA** | `(1,1)` | Clasifica | **OBRA** | `(0,N)` | Una categoría puede clasificar múltiples obras; cada obra pertenece obligatoriamente a una única categoría técnica. |
| **UBICACION** | `(1,1)` | Contiene | **OBRA** | `(0,N)` | Una ubicación física puede albergar una o más obras (ej. un muro extenso con varios grafitis); cada obra se localiza en una sola ubicación. |
| **USUARIO** | `(1,1)` | Registra | **OBRA** | `(0,N)` | Un usuario (administrador) puede ingresar muchas obras; cada obra guarda la traza de quién la dio de alta en el sistema. |
| **OBRA** | `(1,1)` | Posee | **RECURSO_MULTIMEDIA** | `(0,N)` | Una obra puede tener asociados múltiples archivos multimedia (fotos, audios, 3D); cada recurso pertenece exclusivamente a una obra. |
| **RUTA** | `(1,1)` | Estructura | **RUTA_OBRA** | `(1,N)` | Una ruta debe contener al menos una obra (habitualmente varias) en una secuencia ordenada. |
| **OBRA** | `(1,1)` | Se incluye en | **RUTA_OBRA** | `(0,N)` | Una obra puede no pertenecer a ninguna ruta, o estar incluida en múltiples circuitos turísticos de la ciudad. |

---

## 4. Reglas del Negocio (Business Rules)

Para garantizar la integridad y coherencia del sistema, se formulan las siguientes reglas de negocio obligatorias:

* **RN-01 (Integridad de la Obra):** Toda obra que ingrese al sistema debe tener asignado obligatoriamente un Artista, una Categoría, una Ubicación y el Usuario registrador. Ninguno de estos campos puede ser nulo.
* **RN-02 (Eliminación Lógica de Artistas):** Un artista que tenga al menos una obra registrada no puede ser eliminado de la base de datos (se prohíbe `ON DELETE CASCADE` en la clave foránea hacia artista). En su lugar, el sistema desactivará al artista (`estado = 'INACTIVO'`).
* **RN-03 (Almacenamiento Desacoplado de Multimedia):** Está estrictamente prohibido almacenar archivos binarios (`BLOB`/`VARBINARY`) en la base de datos MySQL. MySQL almacena la ruta relativa, formato y tamaño, mientras que el archivo físico reside en la estructura de carpetas `RecursosMultimedia/`.
* **RN-04 (Formatos Multimedia Admitidos):** La aplicación únicamente permitirá cargar archivos con extensiones controladas: `.jpg`, `.jpeg`, `.png` (Imágenes), `.mp4` (Video), `.mp3` (Audio), `.glb` (Modelos 3D).
* **RN-05 (Integridad de Rutas de Arte):**
  - Dentro de una misma ruta no puede figurar dos veces la misma obra.
  - El orden de visita (`orden_visita`) dentro de una ruta debe ser único y positivo consecutivo ($1, 2, 3 \dots$).
* **RN-06 (Georreferenciación Válida en Cali):** Toda ubicación debe tener coordenadas válidas dentro del área de cobertura metropolitana de Cali (Latitud: $[3.25, 3.55]$, Longitud: $[-76.60, -76.45]$).
* **RN-07 (Seguridad de Credenciales):** Las contraseñas de los usuarios deben someterse a hashing unidireccional antes de ser persistidas en la base de datos. Se prohíbe el almacenamiento en texto claro.
* **RN-08 (Control de Acceso Basado en Roles):**
  - **Administrador:** Tiene privilegios de creación, edición, borrado lógico y consulta en todos los módulos (Artistas, Categorías, Ubicaciones, Obras, Multimedia y Rutas).
  - **Visitante:** Acceso en modo de solo lectura (búsquedas, filtros, consultas de rutas y reproducción de multimedia).

---

## 5. Próximos Pasos (Transición a Etapa 2 y Etapa 3)

1. **Construcción del Diagrama Conceptual UML en UMLet (`.uxf`):** Modelar las 8 entidades con su notación estándar, atributos, estereotipos `<<PK>>` y cardinalidades en ambos extremos.
2. **Análisis de Normalización formal hasta 3FN:**
   - Demostrar la ausencia de grupos repetitivos (1FN).
   - Validar que no existan dependencias parciales sobre claves compuestas (2FN en `RUTA_OBRA`).
   - Justificar que no existen dependencias transitivas entre atributos no clave (3FN).
3. **Mapeo al Modelo Relacional (MRD en Excel):** Llenar la plantilla oficial del profesor (`EjemploMRD_Universidad.xlsx`) con las tablas normalizadas.
