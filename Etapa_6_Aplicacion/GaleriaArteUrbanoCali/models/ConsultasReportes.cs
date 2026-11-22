using System;
using System.Collections.Generic;
using System.Data;

namespace GaleriaArteUrbanoCali.models
{
    public class ConsultasReportes
    {
        // 1. Consulta completa de obra (une las vistas vw_catalogo_obras_completo y vw_obras_resumen_multimedia)
        public DataTable ConsultarFichaCompletaObras()
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT v.titulo_obra AS 'Obra',
                       v.artista AS 'Artista',
                       v.artista_nombre_real AS 'Nombre Real',
                       v.pais_artista AS 'País',
                       v.categoria_tecnica AS 'Categoría',
                       v.nombreLugar AS 'Lugar Físico',
                       v.direccion AS 'Dirección',
                       v.barrio AS 'Barrio',
                       v.comuna AS 'Comuna',
                       v.estadoConservacion AS 'Conservación',
                       IFNULL(m.total_recursos, 0) AS 'Total Multimedia',
                       IFNULL(m.tamano_total_mb, 0) AS 'Peso Total (MB)',
                       v.registrado_por AS 'Registrado Por'
                FROM vw_catalogo_obras_completo v
                LEFT JOIN vw_obras_resumen_multimedia m ON v.idObra = m.idObra
                ORDER BY v.numero_comuna, v.titulo_obra;";
            return conexion.ExecuteDataTable(sql);
        }

        // 2. Obras por artista determinado
        public DataTable ConsultarObrasPorArtista(string idArtista)
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT o.nombreObra AS 'Título de la Obra',
                       a.nombreArtistico AS 'Artista',
                       p.nombrePais AS 'País',
                       c.nombreCategoria AS 'Categoría',
                       u.nombreLugar AS 'Lugar',
                       b.nombreBarrio AS 'Barrio',
                       CONCAT('Comuna ', com.numeroComuna) AS 'Comuna',
                       o.estadoConservacion AS 'Estado Conservación',
                       DATE_FORMAT(o.fechaCreacion, '%Y-%m-%d') AS 'Fecha Creación'
                FROM obra o
                INNER JOIN artista a ON o.idArtistaFK = a.idArtista
                INNER JOIN pais p ON a.idPaisFK = p.idPais
                INNER JOIN categoria c ON o.idCategoriaFK = c.idCategoria
                INNER JOIN ubicacion u ON o.idUbicacionFK = u.idUbicacion
                INNER JOIN barrio b ON u.idBarrioFK = b.idBarrio
                INNER JOIN comuna com ON b.idComunaFK = com.idComuna
                WHERE o.idArtistaFK = @idArtista
                ORDER BY o.fechaCreacion DESC;";

            var p = new Dictionary<string, object?> { { "@idArtista", idArtista.Trim() } };
            return conexion.ExecuteDataTable(sql, p);
        }

        // 3. Obras por categoría seleccionada
        public DataTable ConsultarObrasPorCategoria(string idCategoria)
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT o.nombreObra AS 'Título de la Obra',
                       c.nombreCategoria AS 'Categoría',
                       a.nombreArtistico AS 'Artista',
                       u.nombreLugar AS 'Ubicación Física',
                       b.nombreBarrio AS 'Barrio',
                       CONCAT('Comuna ', com.numeroComuna) AS 'Comuna',
                       o.estadoConservacion AS 'Estado Conservación'
                FROM obra o
                INNER JOIN categoria c ON o.idCategoriaFK = c.idCategoria
                INNER JOIN artista a ON o.idArtistaFK = a.idArtista
                INNER JOIN ubicacion u ON o.idUbicacionFK = u.idUbicacion
                INNER JOIN barrio b ON u.idBarrioFK = b.idBarrio
                INNER JOIN comuna com ON b.idComunaFK = com.idComuna
                WHERE o.idCategoriaFK = @idCat
                ORDER BY o.nombreObra;";

            var p = new Dictionary<string, object?> { { "@idCat", idCategoria.Trim() } };
            return conexion.ExecuteDataTable(sql, p);
        }

        // 4. Obras por ubicación (sector, barrio o comuna, uniendo UBICACION - BARRIO - COMUNA)
        public DataTable ConsultarObrasPorComunaOBarrio(string? idComuna, string? idBarrio)
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT CONCAT('Comuna ', com.numeroComuna) AS 'Comuna',
                       b.nombreBarrio AS 'Barrio',
                       u.nombreLugar AS 'Sector / Lugar',
                       u.direccion AS 'Dirección',
                       o.nombreObra AS 'Obra de Arte Urbano',
                       a.nombreArtistico AS 'Artista',
                       c.nombreCategoria AS 'Categoría',
                       o.estadoConservacion AS 'Estado'
                FROM obra o
                INNER JOIN ubicacion u ON o.idUbicacionFK = u.idUbicacion
                INNER JOIN barrio b ON u.idBarrioFK = b.idBarrio
                INNER JOIN comuna com ON b.idComunaFK = com.idComuna
                INNER JOIN artista a ON o.idArtistaFK = a.idArtista
                INNER JOIN categoria c ON o.idCategoriaFK = c.idCategoria
                WHERE (@idComuna IS NULL OR com.idComuna = @idComuna)
                  AND (@idBarrio IS NULL OR b.idBarrio = @idBarrio)
                ORDER BY com.numeroComuna, b.nombreBarrio, o.nombreObra;";

            var p = new Dictionary<string, object?>
            {
                { "@idComuna", string.IsNullOrWhiteSpace(idComuna) ? null : idComuna.Trim() },
                { "@idBarrio", string.IsNullOrWhiteSpace(idBarrio) ? null : idBarrio.Trim() }
            };
            return conexion.ExecuteDataTable(sql, p);
        }

        // 5. Consulta de ruta: obras que la componen, orden de recorrido y ubicación de cada obra
        public DataTable ConsultarItinerarioRuta(string? idRuta = null)
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT nombre_ruta AS 'Ruta Turística',
                       orden_visita AS 'Orden #',
                       titulo_obra AS 'Obra de Arte Urbano',
                       artista AS 'Artista',
                       nombreLugar AS 'Lugar / Sector',
                       direccion AS 'Dirección',
                       barrio AS 'Barrio',
                       comuna AS 'Comuna',
                       tiempo_permanencia_min AS 'Permanencia (min)',
                       recomendacion_parada AS 'Recomendación'
                FROM vw_rutas_itinerario_detallado ";

            Dictionary<string, object?>? p = null;
            if (!string.IsNullOrWhiteSpace(idRuta))
            {
                sql += "WHERE idRuta = @idRuta ";
                p = new Dictionary<string, object?> { { "@idRuta", idRuta.Trim() } };
            }

            sql += "ORDER BY nombre_ruta, orden_visita ASC;";
            return conexion.ExecuteDataTable(sql, p);
        }

        // 6. Estadística 1: Cantidad de obras y estado de conservación por comuna (GROUP BY, COUNT, SUM)
        public DataTable ConsultarEstadisticasPorComuna()
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT comuna AS 'Comuna de Cali',
                       barrios_con_obras AS 'Barrios con Obras',
                       total_obras AS 'Total Obras',
                       total_artistas_intervinientes AS 'Artistas Distintos',
                       total_tecnicas_presentes AS 'Categorías Presentes',
                       obras_buen_estado AS 'Obras en Buen Estado',
                       obras_requieren_intervencion AS 'Requieren Restauración'
                FROM vw_estadisticas_culturales_comuna
                ORDER BY total_obras DESC;";
            return conexion.ExecuteDataTable(sql);
        }

        // 7. Estadística 2: Obras y recursos multimedia por categoría (GROUP BY, COUNT, SUM)
        public DataTable ConsultarEstadisticasPorCategoria()
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT c.nombreCategoria AS 'Categoría',
                       COUNT(DISTINCT o.idObra) AS 'Cantidad de Obras',
                       COUNT(DISTINCT o.idArtistaFK) AS 'Artistas en la Categoría',
                       COUNT(rm.idRecurso) AS 'Total Recursos Multimedia',
                       ROUND(COALESCE(SUM(rm.tamanio), 0), 2) AS 'Volumen Multimedia (MB)'
                FROM categoria c
                LEFT JOIN obra o ON c.idCategoria = o.idCategoriaFK
                LEFT JOIN recurso_multimedia rm ON o.idObra = rm.idObraFK
                GROUP BY c.idCategoria, c.nombreCategoria
                ORDER BY COUNT(DISTINCT o.idObra) DESC;";
            return conexion.ExecuteDataTable(sql);
        }

        // 8. Estadística 3: Artistas con más de N obras (GROUP BY, COUNT, HAVING)
        public DataTable ConsultarArtistasMasProlificos(int minimoObras = 2)
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT a.nombreArtistico AS 'Artista',
                       CONCAT_WS(' ', a.nombre1Real, a.nombre2Real, a.apellido1Real, a.apellido2Real) AS 'Nombre Real',
                       p.nombrePais AS 'País Origen',
                       COUNT(DISTINCT o.idObra) AS 'Total Obras Registradas',
                       COUNT(rm.idRecurso) AS 'Total Archivos Multimedia',
                       ROUND(COALESCE(SUM(rm.tamanio), 0), 2) AS 'Almacenamiento (MB)'
                FROM artista a
                INNER JOIN pais p ON a.idPaisFK = p.idPais
                INNER JOIN obra o ON a.idArtista = o.idArtistaFK
                LEFT JOIN recurso_multimedia rm ON o.idObra = rm.idObraFK
                GROUP BY a.idArtista, a.nombreArtistico, a.nombre1Real, a.nombre2Real,
                         a.apellido1Real, a.apellido2Real, p.nombrePais
                HAVING COUNT(DISTINCT o.idObra) >= @minObras
                ORDER BY COUNT(DISTINCT o.idObra) DESC, SUM(rm.tamanio) DESC;";

            var p = new Dictionary<string, object?> { { "@minObras", minimoObras } };
            return conexion.ExecuteDataTable(sql, p);
        }
    }
}
