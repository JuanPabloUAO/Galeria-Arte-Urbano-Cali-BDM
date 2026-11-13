# Galería de Arte Urbano de Cali — Aplicación de escritorio

Aplicación **C# WinForms (.NET 8)** conectada a **MySQL 8** para registrar, organizar y consultar obras de arte urbano de Cali, sus artistas, ubicaciones, categorías, rutas y recursos multimedia.

**Integrantes:** Juan Pablo Campo Bertin · Jean Deiby Garcia — Bases de Datos Multimedia, UAO 2026-2

---

## 1. Requisitos

| Software | Versión | Nota |
|---|---|---|
| Windows | 10 u 11 | WinForms solo corre en Windows |
| Visual Studio 2022 | 17.8 o superior | Carga de trabajo **"Desarrollo de escritorio de .NET"** |
| .NET SDK | 8.0 | Se instala con Visual Studio |
| MySQL Server | 8.0.16 o superior | Con MySQL Workbench |
| Conexión a internet | solo la primera vez | Visual Studio descarga los paquetes NuGet `MySql.Data` y `BCrypt.Net-Next` |

## 2. Orden de ejecución

### Paso 1 — Crear la base de datos

En MySQL Workbench, abrir y ejecutar (botón del rayo ⚡) **en este orden** los scripts de la carpeta `../Etapa_5_MySQL` (al lado de esta carpeta):

1. `01_estructura.sql` — base de datos y tablas
2. `02_datos_prueba.sql` — datos de prueba y usuarios
3. `03_vistas_procedimientos_indices.sql` — vistas, procedimientos e índices
4. `04_consultas.sql` — (opcional) consultas de demostración

> O ejecutar solo `Galeria_Arte_Urbano_COMPLETO.sql`, que contiene los cuatro.

### Paso 2 — Configurar la cadena de conexión

Editar `src/GaleriaArte.UI/appsettings.json` y poner la contraseña de **su** usuario de MySQL:

```json
{
  "ConnectionStrings": {
    "GaleriaDB": "Server=localhost;Port=3306;Database=galeria_arte_urbano_cali;Uid=root;Pwd=SU_CLAVE;CharSet=utf8mb4;"
  },
  "RutaRecursosMultimedia": ""
}
```

| Parte | Significado |
|---|---|
| `Server=localhost;Port=3306` | Servidor MySQL local, puerto por defecto |
| `Database=galeria_arte_urbano_cali` | Base creada por el script 01 |
| `Uid=root;Pwd=...` | Usuario y contraseña de MySQL |
| `CharSet=utf8mb4` | Para que tildes y ñ lleguen correctamente |
| `RutaRecursosMultimedia` | Vacío = la app busca automáticamente la carpeta `RecursosMultimedia` subiendo desde el `.exe`. También acepta una ruta relativa o absoluta |

### Paso 3 — Ejecutar

1. Abrir `GaleriaArteUrbano.sln` en Visual Studio.
2. Clic derecho en **GaleriaArte.UI** → *Establecer como proyecto de inicio*.
3. **F5**. La primera vez se restauran los paquetes NuGet automáticamente.

### Usuarios de prueba

| Correo | Contraseña | Rol |
|---|---|---|
| `admin@galeriacali.co` | `Admin123*` | ADMINISTRADOR (gestiona todo) |
| `editora@galeriacali.co` | `Editor123*` | ADMINISTRADOR |
| `visitante@galeriacali.co` | `Visitante123*` | VISITANTE (solo consulta) |

## 3. Estructura del proyecto (por capas)

```
Etapa_6_Aplicacion/
├── GaleriaArteUrbano.sln
├── README.md
├── RecursosMultimedia/            ← archivos multimedia (NO están en MySQL)
└── src/
    ├── GaleriaArte.Entidades/     ← CAPA 1: clases = tablas (Obra, Artista, Ruta...)
    ├── GaleriaArte.Datos/         ← CAPA 2: acceso a datos, ÚNICO lugar con SQL (parametrizado)
    │   ├── Conexion.cs, RepositorioBase.cs, TraductorErrores.cs
    │   └── Repositorios/          ← un repositorio por tabla + estadísticas
    ├── GaleriaArte.Negocio/       ← CAPA 3: validaciones, permisos, BCrypt, manejo de archivos
    │   ├── Sesion.cs, GestorArchivos.cs, Validar.cs
    │   └── Servicios/
    └── GaleriaArte.UI/            ← CAPA 4: formularios WinForms (sin lógica de negocio ni SQL)
        ├── appsettings.json
        ├── Formularios/           ← Login, Principal, Obras, Artistas, ..., Consultas
        └── Utilidades/
```

Dependencias: `UI → Negocio → Datos → Entidades`. La interfaz **no** referencia la capa de datos.

## 4. Carpeta de recursos multimedia

```
RecursosMultimedia/
├── Imagenes/     obra_001_frontal.jpg, obra_004_detalle.png, ...   (JPG, PNG)
├── Videos/       obra_001_proceso.mp4, obra_007_dron.mp4             (MP4)
├── Audios/       obra_001_audioguia.mp3, obra_008_testimonio.mp3     (MP3)
├── Modelos3D/    obra_003_escalinata.glb                              (GLB)
└── AR/           obra_001/  (marcador.png, modelo_ar.glb, LEEME.txt)  (carpeta)
```

* MySQL guarda solo la **ruta relativa** (ej. `Imagenes/obra_001_frontal.jpg`), nunca `C:\...`. Un `CHECK` en la tabla lo impide.
* Al registrar un recurso desde la app, el archivo se **copia** a la subcarpeta de su tipo con un nombre único (`obra_<id>_<fecha>.ext`).
* Los formatos permitidos están en la tabla `formato_multimedia`: JPG, PNG, MP4, MP3, GLB y AR (carpeta).
* Los archivos de ejemplo incluidos son **generados** (imágenes abstractas, tonos de audio, un cubo 3D) solo para probar.
* **Al copiar el proyecto a otro equipo, copiar también esta carpeta.**

## 5. Módulos

| Módulo | Operaciones |
|---|---|
| Acceso y usuarios | Login con correo + contraseña verificada con BCrypt. Administrador gestiona; Visitante solo consulta. Gestión de usuarios (solo administrador) |
| Artistas | Registrar, consultar, modificar, eliminar o desactivar (si tiene obras se desactiva) |
| Categorías | Registrar, consultar, modificar, activar/desactivar |
| Ubicaciones | Registrar, consultar, modificar; crear barrios; ver en mapa |
| Obras | Registrar seleccionando artista, categoría y ubicación; consultar, modificar, eliminar |
| Multimedia | Elegir archivo o carpeta AR, registrarlo, ver imágenes, reproducir video/audio/3D con el programa de Windows |
| Rutas | Crear rutas, agregar/retirar obras, subir/bajar el orden, consultar el recorrido |
| Consultas | Ficha de obra con recursos, obras por artista/categoría/barrio/comuna, consulta de ruta, estadísticas con gráfico |

## 6. Problemas frecuentes

| Mensaje | Solución |
|---|---|
| *No se pudo conectar con el servidor MySQL* | Iniciar el servicio MySQL80 (Servicios de Windows) y revisar `Server`/`Port` |
| *Usuario o contraseña de MySQL incorrectos* | Corregir `Uid`/`Pwd` en `appsettings.json` |
| *La base de datos ... no existe* | Ejecutar los scripts SQL (paso 1) |
| *No se encontró el archivo ... dentro de RecursosMultimedia* | Copiar la carpeta `RecursosMultimedia` completa junto a la solución |
| Error NU1101 al compilar (paquete no encontrado) | Conectarse a internet; *Herramientas → Administrador de paquetes NuGet → Restaurar* |
| Video o modelo 3D no abre | Windows no tiene programa asociado a esa extensión; instalar VLC (video) o abrir el `.glb` en https://gltf-viewer.donmccurdy.com |
