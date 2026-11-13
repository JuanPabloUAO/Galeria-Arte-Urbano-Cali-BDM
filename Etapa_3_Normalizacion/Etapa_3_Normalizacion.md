# Galería de Arte Urbano de Cali
## Documento Técnico — Etapa 3: Normalización hasta la Tercera Forma Normal (3FN)

**Asignatura:** Bases de Datos Multimedia (BDM)  
**Institución:** Universidad Autónoma de Occidente (UAO) — Facultad de Ingeniería  
**Docente:** David Alejandro Castro Benavides  
**Integrantes del Grupo:**
* Juan Pablo Campo Bertin
* Jean Deiby Garcia  
**Versión:** 1.0  
**Fecha:** Noviembre de 2026  
**Documentos previos:** `Etapa_1_Analisis_del_Problema.md`, `Etapa_2_Diseno_UML.md`

---

## 1. Objetivo y método

La normalización verifica, con reglas formales, que el diseño conceptual de la Etapa 2 no guarde datos repetidos ni permita **anomalías** de inserción, actualización o borrado.

Para demostrarlo no partimos del diagrama terminado (eso sería "normalizar lo ya normalizado"). Partimos de cómo se vería la información si alguien la llevara en **una sola planilla de Excel** (forma no normalizada, FNN) y la descomponemos paso a paso:

| Paso | Regla | Pregunta que nos hacemos |
|---|---|---|
| FNN → 1FN | Atributos atómicos, sin grupos repetitivos, con clave que identifique cada fila | ¿Hay celdas con listas o columnas que se repiten (recurso1, recurso2...)? |
| 1FN → 2FN | Todo atributo no clave depende de **toda** la clave (no de una parte) | Si la clave es compuesta, ¿algún atributo depende solo de una parte? |
| 2FN → 3FN | Ningún atributo no clave depende de **otro atributo no clave** (sin dependencias transitivas) | ¿Hay un atributo que "arrastra" a otros? (A → B → C) |

**Notación:** `X → Y` se lee "X determina funcionalmente a Y": si conozco X, conozco un único valor de Y. Las claves primarias se marcan con <u>subrayado</u> o con **PK**.

---

## 2. Forma no normalizada (FNN): la "planilla única"

Supongamos que la galería registraba todo en una hoja llamada `REGISTRO_OBRAS`, una fila por obra:

```
REGISTRO_OBRAS (
  id_obra, nombre_obra, descripcion_obra, fecha_creacion, estado_conservacion, fecha_registro,
  nombre_artistico, nombre_real, biografia, pais_origen, red_social, estado_artista,
  nombre_categoria, descripcion_categoria, estado_categoria,
  nombre_lugar, direccion, barrio, comuna, latitud, longitud,
  correo_usuario, nombre_usuario, apellido_usuario, tipo_usuario,
  { recurso_nombre, recurso_tipo, recurso_formato, recurso_ruta, recurso_tamano, recurso_duracion }*,   ← grupo repetitivo
  { ruta_nombre, ruta_descripcion, ruta_duracion, ruta_distancia, ruta_estado,
    orden_visita, tiempo_permanencia, recomendacion }*                                                  ← grupo repetitivo
)
```

Ejemplo de una fila (abreviado):

| id_obra | nombre_obra | nombre_artistico | barrio | comuna | recursos | rutas |
|---|---|---|---|---|---|---|
| O-01 | Memoria del Río | Kromo Salsa | Centro | 3 | "Foto frontal (JPG), Video proceso (MP4), Audio guía (MP3)" | "Ruta Centro (orden 2), Ruta Salsa (orden 1)" |

### Problemas (anomalías) de esta planilla

* **Actualización:** si el artista *Kromo Salsa* cambia su red social, hay que corregirla en cada fila de cada una de sus obras; si se olvida una, la información queda contradictoria.
* **Inserción:** no se puede registrar un barrio, una categoría o una ruta nueva si todavía no hay una obra que los use.
* **Borrado:** si se elimina la única obra de una categoría, se pierde también la descripción de la categoría.
* **Celdas con listas:** "recursos" y "rutas" guardan varios valores en una celda; no se pueden filtrar, contar ni validar.

---

## 3. Primera Forma Normal (1FN)

### 3.1 Reglas que verificamos

1. **Atomicidad:** cada celda tiene un solo valor indivisible para el uso del sistema.
2. **Sin grupos repetitivos:** no hay listas ni columnas tipo `recurso1, recurso2, recurso3`.
3. **Identificación:** cada fila tiene una clave primaria que la distingue.

### 3.2 Transformaciones aplicadas

| # | Problema detectado | Acción | Resultado |
|---|---|---|---|
| 1FN-1 | El grupo `{recurso...}*` repite datos dentro de la fila de la obra | Se saca a su propia relación; cada recurso es una fila que lleva el `id_obra` al que pertenece | `RECURSO_1FN` |
| 1FN-2 | El grupo `{ruta...}*` repite datos dentro de la fila de la obra | Se saca a su propia relación; cada fila es "esta obra en esta ruta" | `PARTICIPACION_RUTA_1FN` |
| 1FN-3 | `nombre_usuario` podría guardarse como "Ana María Pérez Gómez" | Se mantienen **separados** `nombre` y `apellido` (ya lo estaban desde la Etapa 1) para poder ordenar y buscar por apellido | Atributos atómicos |
| 1FN-4 | Coordenadas como texto "3.4516, -76.5320" | Se separan en `latitud` y `longitud` numéricas, para validar rangos con `CHECK` y poder calcular distancias | Atributos atómicos |
| 1FN-5 | `red_social` podría contener varios enlaces separados por comas | **Decisión:** se guarda un único enlace principal (portafolio o red más usada). Si el negocio exigiera varias redes por artista, habría que crear una tabla `RED_SOCIAL_ARTISTA` | Atributo atómico |
| 1FN-6 | `direccion` ("Carrera 1 # 11-05") podría descomponerse en tipo de vía, número, etc. | **Decisión:** se deja como un solo valor. El sistema nunca busca ni ordena por "número de vía"; las búsquedas geográficas se hacen por barrio y comuna. Atomicidad depende del uso, y para este uso el valor es atómico | Atributo atómico |

### 3.3 Relaciones en 1FN

```
OBRA_1FN (
  id_obra PK, nombre_obra, descripcion_obra, fecha_creacion, estado_conservacion, fecha_registro,
  nombre_artistico, nombre_real, biografia, pais_origen, red_social, estado_artista,
  nombre_categoria, descripcion_categoria, estado_categoria,
  nombre_lugar, direccion, barrio, comuna, latitud, longitud,
  correo_usuario, nombre_usuario, apellido_usuario, tipo_usuario, estado_usuario
)

RECURSO_1FN (
  id_recurso PK, id_obra, nombre, tipo_recurso, formato, descripcion,
  ruta_archivo, tamano_bytes, duracion_segundos, fecha_registro
)

PARTICIPACION_RUTA_1FN (
  id_ruta PK, id_obra PK,                      ← clave compuesta
  nombre_ruta, descripcion_ruta, duracion_estimada_min, distancia_estimada_km, estado_ruta,
  orden_visita, tiempo_estimado_min, recomendacion
)
```

---

## 4. Segunda Forma Normal (2FN)

### 4.1 Regla

Una relación está en 2FN si está en 1FN y **cada atributo no clave depende de la clave completa**. Solo puede violarse cuando la clave es **compuesta**; si la clave es de un solo atributo, la relación ya está en 2FN.

### 4.2 Análisis de dependencias

**`OBRA_1FN`** — clave simple `id_obra`. **Ya está en 2FN.**

**`RECURSO_1FN`** — clave simple `id_recurso`. **Ya está en 2FN.**

**`PARTICIPACION_RUTA_1FN`** — clave compuesta `(id_ruta, id_obra)`. Analizamos cada atributo:

| Atributo | ¿De qué depende? | ¿Dependencia parcial? |
|---|---|---|
| `nombre_ruta` | `id_ruta → nombre_ruta` (el nombre de la ruta no cambia según la obra) | **Sí** |
| `descripcion_ruta` | `id_ruta` | **Sí** |
| `duracion_estimada_min` | `id_ruta` | **Sí** |
| `distancia_estimada_km` | `id_ruta` | **Sí** |
| `estado_ruta` | `id_ruta` | **Sí** |
| `orden_visita` | `(id_ruta, id_obra)`: la misma obra puede ser la 1.ª parada en una ruta y la 4.ª en otra | No — depende de toda la clave |
| `tiempo_estimado_min` | `(id_ruta, id_obra)`: en una ruta rápida se sugieren 5 min y en una ruta fotográfica 20 min | No |
| `recomendacion` | `(id_ruta, id_obra)`: el consejo depende del contexto del recorrido | No |

### 4.3 Transformación aplicada

| # | Tabla que cambia | Cambio | Justificación |
|---|---|---|---|
| 2FN-1 | `PARTICIPACION_RUTA_1FN` | Se extraen `nombre_ruta`, `descripcion_ruta`, `duracion_estimada_min`, `distancia_estimada_km` y `estado_ruta` a una nueva relación **`RUTA`** con clave `id_ruta` | Dependían solo de `id_ruta` (dependencia parcial). Sin el cambio, el nombre de una ruta con 8 obras se repetiría 8 veces y podría quedar escrito de formas distintas |
| 2FN-2 | `PARTICIPACION_RUTA_1FN` → **`RUTA_OBRA`** | Conserva solo `id_ruta`, `id_obra`, `orden_visita`, `tiempo_estimado_min`, `recomendacion` | Estos atributos describen la **relación** entre una ruta y una obra, no a ninguna de las dos por separado. Por eso `RUTA_OBRA` es entidad asociativa y no se reduce a una FK (condición de diseño del enunciado) |

### 4.4 Decisión sobre la clave de `RUTA_OBRA`

La clave natural de `RUTA_OBRA` es `(id_ruta, id_obra)`. En el diseño de la Etapa 2 definimos además un identificador propio `id_ruta_obra` (UUID). Mantenemos esa decisión:

* **PK:** `id_ruta_obra` (sustituta). Facilita que la aplicación C# modifique o elimine una parada con un solo valor.
* **Clave candidata 1:** `UNIQUE (id_rutaFK, id_obraFK)` → una obra no se repite en la misma ruta.
* **Clave candidata 2:** `UNIQUE (id_rutaFK, orden_visita)` → el orden no se repite en la misma ruta.

Agregar una PK sustituta **no rompe la 2FN**: con PK simple no hay dependencias parciales posibles, y las dos restricciones `UNIQUE` siguen protegiendo las claves naturales (ambas exigidas en la sección 10 del enunciado).

### 4.5 Relaciones en 2FN

```
OBRA_2FN    (id_obra PK, ... igual que OBRA_1FN ...)
RECURSO_2FN (id_recurso PK, ... igual que RECURSO_1FN ...)
RUTA        (id_ruta PK, nombre, descripcion, duracion_estimada_min, distancia_estimada_km, estado)
RUTA_OBRA   (id_ruta_obra PK, id_rutaFK, id_obraFK, orden_visita, tiempo_estimado_min, recomendacion)
```

---

## 5. Tercera Forma Normal (3FN)

### 5.1 Regla

Una relación está en 3FN si está en 2FN y **ningún atributo no clave depende transitivamente de la clave**; es decir, no existe `PK → A → B` donde `A` no es clave.

### 5.2 Dependencias transitivas en `OBRA_2FN`

En la planilla, la obra "arrastra" los datos de otras cosas:

| Dependencia transitiva | Explicación | Acción |
|---|---|---|
| `id_obra → nombre_artistico → (nombre_real, biografia, pais_origen, red_social, estado_artista)` | La biografía es del artista, no de la obra | 3FN-1: se crea **`ARTISTA`** con `id_artista` (UUID) y la obra conserva `id_artistaFK` |
| `id_obra → nombre_categoria → (descripcion_categoria, estado_categoria)` | La descripción es de la categoría | 3FN-2: se crea **`CATEGORIA`**; la obra conserva `id_categoriaFK` |
| `id_obra → (nombre_lugar, direccion) → (barrio, comuna, latitud, longitud)` | Los datos del sitio son del lugar; varias obras pueden estar en el mismo muro | 3FN-3: se crea **`UBICACION`**; la obra conserva `id_ubicacionFK` |
| `id_obra → correo_usuario → (nombre_usuario, apellido_usuario, tipo_usuario, estado_usuario)` | Los datos de la persona que registra no son de la obra | 3FN-4: se crea **`USUARIO`**; la obra conserva `id_usuarioFK` |

Con 3FN-1 a 3FN-4 llegamos exactamente a las 8 entidades de la Etapa 2. **Pero al revisar cada una de ellas con la misma regla encontramos dos dependencias transitivas más** que el diagrama de la Etapa 2 todavía tenía:

### 5.3 Hallazgo 1 — `UBICACION`: `barrio → comuna`

```
UBICACION (id_ubicacion PK, nombre_lugar, direccion, barrio, comuna, latitud, longitud)

id_ubicacion → barrio
barrio       → comuna        ← cada barrio de Cali pertenece a una sola comuna
∴ id_ubicacion → comuna       (transitiva)
```

**Por qué es un problema:** si hay 6 ubicaciones en San Antonio, la comuna 3 se escribe 6 veces. Nada impide guardar "San Antonio – comuna 3" en una fila y "San Antonio – comuna 19" en otra, y entonces la consulta *"obras por comuna"* (requerida en la sección 12 del enunciado) daría resultados falsos.

| # | Tabla que cambia | Cambio | Justificación |
|---|---|---|---|
| **3FN-5** | `UBICACION` | Se eliminan `barrio` y `comuna`; se agrega `id_barrioFK` | `comuna` dependía de `barrio`, que no es clave |
| **3FN-6** | **Nueva tabla `BARRIO`** | `BARRIO (id_barrio PK, nombre UNIQUE, comuna)` | La comuna se escribe una sola vez por barrio. Además, la aplicación ofrece una lista de barrios (sin errores de digitación) y la estadística por comuna queda garantizada |

> **¿Por qué no una tabla `COMUNA`?** La comuna solo tiene un número (1 a 22) y ningún otro atributo que dependa de ella. Una tabla `COMUNA(numero)` no eliminaría ninguna redundancia. Se controla con `CHECK (comuna BETWEEN 1 AND 22)`. Si en el futuro se guardaran datos propios de la comuna (nombre, alcaldía local, población), entonces sí se crearía.

### 5.4 Hallazgo 2 — `RECURSO_MULTIMEDIA`: `formato → tipo_recurso`

```
RECURSO_MULTIMEDIA (id_recurso PK, nombre, tipo_recurso, descripcion, ruta_archivo,
                    formato, tamano_bytes, duracion_segundos, fecha_registro, id_obraFK)

id_recurso → formato
formato    → tipo_recurso      ← un JPG siempre es IMAGEN, un MP4 siempre es VIDEO
∴ id_recurso → tipo_recurso      (transitiva)
```

**Por qué es un problema:** nada impide registrar `formato = 'MP3'` con `tipo_recurso = 'IMAGEN'`. Con dos `CHECK` independientes ambos valores son "válidos", pero la combinación es incoherente y la aplicación intentaría mostrar un audio como imagen.

| # | Tabla que cambia | Cambio | Justificación |
|---|---|---|---|
| **3FN-7** | `RECURSO_MULTIMEDIA` | Se elimina `tipo_recurso`; `formato` pasa a ser `codigo_formatoFK` | `tipo_recurso` dependía de `formato`, que no es clave |
| **3FN-8** | **Nueva tabla `FORMATO_MULTIMEDIA`** | `FORMATO_MULTIMEDIA (codigo_formato PK, tipo_recurso, mime_type, descripcion)` | Cada formato permitido se registra una vez con su tipo. Además cumple el requisito "tipo y formatos controlados": un recurso solo puede usar un formato que exista en esta tabla (integridad referencial en lugar de listas repetidas en el código) |

> **Beneficio adicional:** para admitir un formato nuevo (por ejemplo `WEBP`) basta con insertar una fila en `FORMATO_MULTIMEDIA`; no hay que modificar ningún `CHECK` ni recompilar la aplicación.

### 5.5 Dependencias revisadas que **no** generan cambios

Revisar y documentar lo que **no** se cambia también es parte de la justificación:

| Tabla | Dependencia analizada | Conclusión |
|---|---|---|
| `RECURSO_MULTIMEDIA` | `ruta_archivo → codigo_formatoFK` (la extensión del archivo indica el formato) | `ruta_archivo` es **clave candidata** (`UNIQUE`: dos recursos no pueden apuntar al mismo archivo). Una dependencia desde una clave candidata no es transitiva. Se mantiene |
| `RECURSO_MULTIMEDIA` | `duracion_segundos` solo aplica a audio y video | No es dependencia funcional, es una regla de negocio. Se permite `NULL` y la capa de negocio en C# valida que imágenes y modelos 3D no lleven duración |
| `RUTA` | ¿`duracion_estimada_min` se calcula sumando `tiempo_estimado_min` de sus paradas? | **No.** La duración total incluye los desplazamientos a pie entre obras, que no están en la base de datos. No es un atributo derivado; se mantiene |
| `RUTA` | ¿`distancia_estimada_km` se deriva de las coordenadas? | Solo en línea recta; la distancia real depende de calles y senderos. Se mantiene como dato del recorrido |
| `ARTISTA` | `pais_origen` | Ningún otro atributo depende del país. Una tabla `PAIS` sería útil solo si se guardaran más datos del país. Se mantiene |
| `USUARIO` | `tipo_usuario → permisos` | Los permisos no se guardan como columnas; los aplica la aplicación según el rol. No hay atributo transitivo almacenado |
| `UBICACION` | `(latitud, longitud) → direccion` | En la práctica casi lo es, pero dos registros pueden compartir coordenadas aproximadas (mismo muro, distinta cara). No se declara como clave |
| `OBRA` | `estado_conservacion` | Depende solo de la obra. Se controla con `CHECK` |

---

## 6. Resultado final en 3FN (10 relaciones)

```
USUARIO            (id_usuario PK, nombre, apellido, correo UQ, password_hash, tipo_usuario,
                    fecha_registro, estado)
ARTISTA            (id_artista PK, nombre_artistico UQ, nombre_real, biografia, pais_origen,
                    red_social, estado)
CATEGORIA          (id_categoria PK, nombre UQ, descripcion, estado)
BARRIO             (id_barrio PK, nombre UQ, comuna)                                   ← NUEVA
UBICACION          (id_ubicacion PK, nombre_lugar, direccion, id_barrioFK, latitud, longitud)
OBRA               (id_obra PK, nombre, descripcion, fecha_creacion, estado_conservacion,
                    fecha_registro, id_artistaFK, id_categoriaFK, id_ubicacionFK, id_usuarioFK)
FORMATO_MULTIMEDIA (codigo_formato PK, tipo_recurso, mime_type, descripcion)          ← NUEVA
RECURSO_MULTIMEDIA (id_recurso PK, nombre, descripcion, ruta_archivo UQ, codigo_formatoFK,
                    tamano_bytes, duracion_segundos, fecha_registro, id_obraFK)
RUTA               (id_ruta PK, nombre UQ, descripcion, duracion_estimada_min,
                    distancia_estimada_km, estado)
RUTA_OBRA          (id_ruta_obra PK, id_rutaFK, id_obraFK, orden_visita, tiempo_estimado_min,
                    recomendacion)   UQ(id_rutaFK, id_obraFK)  UQ(id_rutaFK, orden_visita)
```

### 6.1 Resumen de cambios respecto a la Etapa 2

| Código | Tabla afectada | Qué cambió | Forma normal |
|---|---|---|---|
| 1FN-1, 1FN-2 | Planilla → `RECURSO`, `RUTA_OBRA` | Grupos repetitivos separados | 1FN |
| 2FN-1, 2FN-2 | `RUTA_OBRA` / `RUTA` | Datos de la ruta separados de la participación | 2FN |
| 3FN-1 a 3FN-4 | `OBRA` | Artista, categoría, ubicación y usuario separados (coincide con Etapa 2) | 3FN |
| **3FN-5, 3FN-6** | **`UBICACION`, nueva `BARRIO`** | `barrio` y `comuna` → tabla `BARRIO`; `UBICACION` guarda `id_barrioFK` | 3FN |
| **3FN-7, 3FN-8** | **`RECURSO_MULTIMEDIA`, nueva `FORMATO_MULTIMEDIA`** | `tipo_recurso` → tabla `FORMATO_MULTIMEDIA`; el recurso guarda `codigo_formatoFK` | 3FN |

### 6.2 Nuevas relaciones para actualizar el diagrama UML

| Relación | Verbo | Cardinalidad | Lectura |
|---|---|---|---|
| `BARRIO — UBICACION` | agrupa | `BARRIO 1..1` — `0..*` `UBICACION` | Un barrio agrupa cero o muchas ubicaciones; cada ubicación está en exactamente un barrio |
| `FORMATO_MULTIMEDIA — RECURSO_MULTIMEDIA` | describe | `FORMATO 1..1` — `0..*` `RECURSO` | Un formato describe cero o muchos recursos; cada recurso tiene exactamente un formato |

El archivo `Galeria_Arte_Urbano_Cali_MER_v2.uxf` (en esta misma carpeta) incluye estas dos entidades y relaciones para mantener la coherencia UML ↔ MRD ↔ SQL que pide el enunciado (sección 17).
