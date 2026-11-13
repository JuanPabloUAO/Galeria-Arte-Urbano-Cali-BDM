# Galería de Arte Urbano de Cali

Proyecto de curso — **Bases de Datos Multimedia (BDM)**  
**Universidad Autónoma de Occidente (UAO)** — Facultad de Ingeniería  
**Docente:** David Alejandro Castro Benavides  

### Integrantes
* **Juan Pablo Campo Bertin** (Cód. 2235544)
* **Jean Deiby Garcia**

---

## Descripción del Proyecto
Aplicación de escritorio en **C# WinForms (.NET 8)** conectada a **MySQL 8** para la gestión, catalogación y consulta de obras de arte urbano en Santiago de Cali, sus artistas, ubicaciones georreferenciadas, rutas culturales y recursos multimedia asociados (imágenes, audios, videos, modelos 3D y realidad aumentada).

Siguiendo las pautas de clase, los metadatos y rutas relativas se administran en MySQL y los archivos multimedia físicos se almacenan en el sistema de archivos local (`RecursosMultimedia/`).

---

## Estructura del Repositorio

* `Etapa_2_Diseno_UML/`: Diagrama conceptual MER en UMLet (`.uxf`) y vector (`.svg`).
* `Etapa_3_Normalizacion/`: Diagrama MER normalizado hasta Tercera Forma Normal 3FN (`Galeria_Arte_Urbano_Cali_MER_v2.uxf`).
* `Etapa_4_Modelo_Relacional/`: Modelo Relacional de Datos en formato oficial UAO (`Galeria_Arte_Urbano_MRD.xlsx`) y diagrama relacional (`.png`).
* `Etapa_5_MySQL/`: Scripts de base de datos para MySQL 8:
  * `01_estructura.sql` (creación de base de datos, 10 tablas, llaves UUID y restricciones CHECK).
  * `02_datos_prueba.sql` (datos de prueba para usuarios, artistas, obras, rutas y recursos).
  * `03_vistas_procedimientos_indices.sql` (vistas analíticas e índices).
  * `04_consultas.sql` (consultas con JOINs de 3 o más tablas).
  * `Galeria_Arte_Urbano_COMPLETO.sql` (script completo unificado).
* `Etapa_6_Aplicacion/`: Solución en C# (.NET 8 Windows Forms):
  * `GaleriaArteUrbano.sln` (arquitectura por capas: Datos, Entidades, Negocio y UI).
  * `RecursosMultimedia/` (archivos de audio, video, imagen, 3D y marcadores AR).
  * `LEEME.txt` (instrucciones rápidas de configuración y ejecución).

---

## Cómo Ejecutar el Proyecto

1. **Base de Datos:**
   En MySQL Workbench ejecutar en orden los scripts `01` -> `02` -> `03` (o ejecutar directamente `Galeria_Arte_Urbano_COMPLETO.sql`).

2. **Configuración de Conexión:**
   En `Etapa_6_Aplicacion/src/GaleriaArte.UI/appsettings.json`, verificar los parámetros de MySQL local.

3. **Ejecución:**
   Abrir `GaleriaArteUrbano.sln` en Visual Studio 2022 y presionar **F5**.

### Usuarios Registrados para Pruebas
* **Administrador:** `admin@galeriacali.co` | Clave: `Admin123*`
* **Visitante:** `visitante@galeriacali.co` | Clave: `Visitante123*`
