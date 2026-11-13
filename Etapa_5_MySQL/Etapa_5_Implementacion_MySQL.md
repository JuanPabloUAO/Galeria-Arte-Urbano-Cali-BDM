# Galería de Arte Urbano de Cali
## Documento Técnico — Etapa 5: Implementación en MySQL

**Integrantes:** Juan Pablo Campo Bertin · Jean Deiby Garcia  
**Motor:** MySQL 8.0.16 o superior (probado en MySQL 8.0.46)  
**Fecha:** Noviembre de 2026

---

## 1. Archivos de esta carpeta

| Orden | Archivo | Qué hace |
|---|---|---|
| 1 | `01_estructura.sql` | Borra y crea la base `galeria_arte_urbano_cali` y sus 10 tablas con PK, FK, UNIQUE, CHECK, DEFAULT y `ON DELETE/ON UPDATE` |
| 2 | `02_datos_prueba.sql` | Carga 3 usuarios, 7 artistas, 6 categorías, 9 barrios, 10 ubicaciones, 6 formatos, 12 obras, 18 recursos, 3 rutas y 13 paradas |
| 3 | `03_vistas_procedimientos_indices.sql` | 3 vistas, 4 procedimientos almacenados y 2 índices justificados |
| 4 | `04_consultas.sql` | Consultas del enunciado (JOIN), 7 consultas con agregación, INSERT/UPDATE/DELETE, llamadas a procedimientos y EXPLAIN |
| — | `Galeria_Arte_Urbano_COMPLETO.sql` | Los cuatro anteriores unidos, por si el docente pide **un solo** script |

El script `04` deja la base exactamente igual a como estaba (las modificaciones se deshacen con `ROLLBACK` o se revierten), así que se puede ejecutar cuantas veces se quiera.

## 2. Cumplimiento de los requisitos del enunciado (sección 10)

| Requisito | Dónde está |
|---|---|
| Creación de BD y tablas | `01` |
| PK y FK | `01` — 10 PK, 9 FK |
| NOT NULL, UNIQUE, CHECK | `01` — 10 UNIQUE, 29 CHECK |
| Valores por defecto | `01` — UUID, fechas, estados |
| ON DELETE / ON UPDATE definidos | `01` — las 9 FK (justificación en Etapa 4, §4) |
| UNIQUE (id_ruta, id_obra) y (id_ruta, orden) | `01` — `uq_ruta_obra_obra`, `uq_ruta_obra_orden` |
| Mínimo 5 artistas, 5 categorías, 10 obras, 3 rutas, 15 recursos | `02` — 7, 6, 12, 3, 18 |
| INSERT, SELECT, UPDATE, DELETE, JOIN | `04` secciones A y C |
| ≥ 3 consultas con GROUP BY / COUNT / HAVING | `04` sección B (B1–B7) |
| ≥ 1 vista | `03` — `vw_obra_detalle`, `vw_ruta_detalle`, `vw_recurso_detalle` |
| ≥ 1 procedimiento almacenado | `03` — 4 procedimientos |
| ≥ 1 índice justificado | `03` — `idx_obra_nombre`, `idx_barrio_comuna` (§5) |

## 3. Explicación de las vistas

| Vista | Para qué sirve | La usa en C# |
|---|---|---|
| `vw_obra_detalle` | Une obra + artista + categoría + ubicación + barrio + usuario y cuenta los recursos. Responde "consulta de obra", "obras por artista/categoría/ubicación" con un simple `WHERE` | `ConsultaRepository`, `ObraRepository.Listar` |
| `vw_ruta_detalle` | Lista las paradas de cada ruta con su orden, obra, artista y ubicación | `RutaRepository.ObtenerParadas` |
| `vw_recurso_detalle` | Muestra cada recurso con su tipo (que viene de `formato_multimedia` por la 3FN) | `RecursoRepository` |

## 4. Explicación de los procedimientos almacenados

| Procedimiento | Problema que resuelve |
|---|---|
| `sp_agregar_obra_a_ruta(ruta, obra, tiempo, recomendación)` | Calcula el siguiente `orden_visita` (máximo + 1) dentro de una transacción con `FOR UPDATE`, para que dos administradores no obtengan el mismo número |
| `sp_retirar_obra_de_ruta(ruta, obra)` | Borra la parada y corre las siguientes una posición hacia atrás para que no queden huecos (1, 2, 4 → 1, 2, 3) |
| `sp_mover_obra_en_ruta(ruta, obra, nuevo_orden)` | Cambia la posición de una parada y desplaza las demás. Primero "estaciona" la obra en la posición 60000 para no chocar con el `UNIQUE (id_rutaFK, orden_visita)` |
| `sp_eliminar_o_desactivar_artista(artista, OUT resultado)` | Aplica RN-02: si el artista tiene obras lo marca `INACTIVO`; si no tiene, lo borra. Devuelve `'DESACTIVADO'` o `'ELIMINADO'` |

**Detalle técnico importante (para la sustentación):** cuando se actualizan varios `orden_visita` a la vez, MySQL verifica el `UNIQUE` fila por fila. Si se resta 1 empezando por la parada 5, la 5 pasaría a 4 mientras la 4 todavía existe → error de duplicado. Por eso los `UPDATE` llevan `ORDER BY orden_visita ASC` (al restar) o `DESC` (al sumar).

**Otro detalle:** un `START TRANSACTION` dentro de un procedimiento confirma (*commit*) cualquier transacción que estuviera abierta afuera. Por eso en el script `04` los procedimientos no se prueban dentro de `START TRANSACTION ... ROLLBACK`, sino que sus efectos se revierten con la operación contraria.

## 5. Justificación de los índices (con evidencia EXPLAIN)

MySQL crea índices automáticamente para PK, UNIQUE y FK. Los dos índices adicionales responden a consultas concretas de la aplicación:

**`idx_obra_nombre ON obra(nombre)`** — búsqueda de obras por nombre en el módulo de consultas.

```sql
EXPLAIN SELECT id_obra, nombre FROM obra WHERE nombre LIKE 'Mem%' ORDER BY nombre;
```

| | type | key | rows | Extra |
|---|---|---|---|---|
| Sin el índice | `index` (recorre todo el índice de la ubicación) | `uq_obra_ubicacion_nombre` | 12 | `Using where; Using filesort` |
| Con el índice | `range` | `idx_obra_nombre` | 1 | `Using where; Using index` |

Con el índice MySQL lee solo las filas que empiezan por "Mem" y ya vienen ordenadas (desaparece el `filesort`). Con 12 obras la diferencia es pequeña, pero crece con el catálogo. El `UNIQUE (id_ubicacionFK, nombre)` no sirve porque su primera columna es la ubicación (un índice compuesto solo se aprovecha de izquierda a derecha).

**`idx_barrio_comuna ON barrio(comuna)`** — consulta "obras por comuna" y la estadística por comuna.

```sql
EXPLAIN SELECT b.comuna, COUNT(o.id_obra) FROM barrio b
JOIN ubicacion u ON u.id_barrioFK = b.id_barrio
JOIN obra o ON o.id_ubicacionFK = u.id_ubicacion
WHERE b.comuna = 3 GROUP BY b.comuna;
```

Resultado: la tabla `barrio` se accede con `type = ref` usando `idx_barrio_comuna` (va directo a los barrios de la comuna 3) y las siguientes tablas usan los índices de las FK (`Using index` en las tres), es decir, la consulta se resuelve completa sin leer las tablas.

## 6. Pruebas de integridad realizadas

Se ejecutaron estas sentencias inválidas para comprobar que la base las rechaza:

| Prueba | Resultado de MySQL |
|---|---|
| Usuario con contraseña `'clave123'` (texto plano) | `Check constraint 'ck_usuario_hash' is violated` |
| Ubicación en Bogotá (lat 4.6) | `Check constraint 'ck_ubicacion_latitud' is violated` |
| Recurso con ruta `C:/fotos/a.jpg` | `Check constraint 'ck_recurso_ruta_rel' is violated` |
| Recurso con formato `BMP` (no permitido) | `Error 1452: a foreign key constraint fails` |
| Misma obra dos veces en una ruta | `Duplicate entry ... for key 'uq_ruta_obra_obra'` |
| Dos obras con orden 1 en la misma ruta | `Duplicate entry ... for key 'uq_ruta_obra_orden'` |
| Borrar el artista "Kromo Salsa" (tiene obras) | `Error 1451: Cannot delete or update a parent row` |
| Categoría "Grafíti" cuando ya existe "Grafiti" | `Duplicate entry 'Grafíti' for key 'uq_categoria_nombre'` |
