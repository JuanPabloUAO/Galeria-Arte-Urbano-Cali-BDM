using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace GaleriaArteUrbanoCali.models
{
    public class RutaObra
    {
        public string IdRutaObra { get; set; } = string.Empty;
        public int OrdenVisita { get; set; } = 1;
        public string? Recomendacion { get; set; }
        public int TiempoPermanencia { get; set; } = 20;
        public string IdRutaFK { get; set; } = string.Empty;
        public string IdObraFK { get; set; } = string.Empty;

        public DataTable ConsultarParadasPorRuta(string idRuta)
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT ro.idRutaObra AS 'ID Parada',
                       ro.ordenVisita AS 'Orden Visita',
                       o.nombreObra AS 'Obra de Arte Urbano',
                       a.nombreArtistico AS 'Artista',
                       u.nombreLugar AS 'Lugar Físico',
                       b.nombreBarrio AS 'Barrio',
                       CONCAT('Comuna ', c.numeroComuna) AS 'Comuna',
                       ro.tiempoPermanencia AS 'Permanencia (min)',
                       IFNULL(ro.recomendacion, '') AS 'Recomendación de Visita'
                FROM ruta_obra ro
                INNER JOIN obra o ON ro.idObraFK = o.idObra
                INNER JOIN artista a ON o.idArtistaFK = a.idArtista
                INNER JOIN ubicacion u ON o.idUbicacionFK = u.idUbicacion
                INNER JOIN barrio b ON u.idBarrioFK = b.idBarrio
                INNER JOIN comuna c ON b.idComunaFK = c.idComuna
                WHERE ro.idRutaFK = @idRuta
                ORDER BY ro.ordenVisita ASC;";

            var p = new Dictionary<string, object?> { { "@idRuta", idRuta.Trim() } };
            return conexion.ExecuteDataTable(sql, p);
        }

        // Invoca sp_asignar_obra_a_ruta tras verificar que la obra y el orden no se repitan en la ruta
        public bool AgregarObraARuta(RutaObra ro, out string mensajeError)
        {
            mensajeError = string.Empty;
            using var conexion = new ConexionMysql();

            string sqlValObra = "SELECT COUNT(*) FROM ruta_obra WHERE idRutaFK = @idRuta AND idObraFK = @idObra;";
            var pValObra = new Dictionary<string, object?>
            {
                { "@idRuta", ro.IdRutaFK.Trim() },
                { "@idObra", ro.IdObraFK.Trim() }
            };
            if (Convert.ToInt32(conexion.ExecuteScalar(sqlValObra, pValObra) ?? 0) > 0)
            {
                mensajeError = "La obra seleccionada ya hace parte de esta ruta.";
                return false;
            }

            string sqlValOrden = "SELECT COUNT(*) FROM ruta_obra WHERE idRutaFK = @idRuta AND ordenVisita = @orden;";
            var pValOrden = new Dictionary<string, object?>
            {
                { "@idRuta", ro.IdRutaFK.Trim() },
                { "@orden", ro.OrdenVisita }
            };
            if (Convert.ToInt32(conexion.ExecuteScalar(sqlValOrden, pValOrden) ?? 0) > 0)
            {
                mensajeError = $"El orden de visita #{ro.OrdenVisita} ya está ocupado en esta ruta.";
                return false;
            }

            try
            {
                using MySqlCommand cmd = new MySqlCommand("sp_asignar_obra_a_ruta", conexion.Abrir());
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@p_id_ruta", ro.IdRutaFK.Trim());
                cmd.Parameters.AddWithValue("@p_id_obra", ro.IdObraFK.Trim());
                cmd.Parameters.AddWithValue("@p_orden_visita", ro.OrdenVisita);
                cmd.Parameters.AddWithValue("@p_tiempo_min", ro.TiempoPermanencia);
                cmd.Parameters.AddWithValue("@p_recomendacion", string.IsNullOrWhiteSpace(ro.Recomendacion) ? DBNull.Value : ro.Recomendacion.Trim());

                var outParam = new MySqlParameter("@p_nuevo_id_hito", MySqlDbType.String, 36)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outParam);

                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                mensajeError = ex.Message;
                return false;
            }
            finally
            {
                conexion.Cerrar();
            }
        }

        public bool ActualizarOrdenParada(string idRutaObra, string idRuta, int nuevoOrden, int nuevoTiempo, string? recomendacion, out string mensajeError)
        {
            using var conexion = new ConexionMysql();

            string sqlValOrden = "SELECT COUNT(*) FROM ruta_obra WHERE idRutaFK = @idRuta AND ordenVisita = @orden AND idRutaObra <> @idParada;";
            var pValOrden = new Dictionary<string, object?>
            {
                { "@idRuta", idRuta.Trim() },
                { "@orden", nuevoOrden },
                { "@idParada", idRutaObra.Trim() }
            };
            if (Convert.ToInt32(conexion.ExecuteScalar(sqlValOrden, pValOrden) ?? 0) > 0)
            {
                mensajeError = $"El orden de visita #{nuevoOrden} ya está asignado a otra parada en esta ruta.";
                return false;
            }

            string sql = @"
                UPDATE ruta_obra
                SET ordenVisita = @orden,
                    tiempoPermanencia = @tiempo,
                    recomendacion = @rec
                WHERE idRutaObra = @id;";

            var p = new Dictionary<string, object?>
            {
                { "@id", idRutaObra.Trim() },
                { "@orden", nuevoOrden },
                { "@tiempo", nuevoTiempo },
                { "@rec", string.IsNullOrWhiteSpace(recomendacion) ? null : recomendacion.Trim() }
            };

            return conexion.ExecuteQuery(sql, p, out mensajeError);
        }

        public bool RetirarParadaDeRuta(string idRutaObra, out string mensajeError)
        {
            using var conexion = new ConexionMysql();
            string sql = "DELETE FROM ruta_obra WHERE idRutaObra = @id;";
            var p = new Dictionary<string, object?> { { "@id", idRutaObra.Trim() } };
            return conexion.ExecuteQuery(sql, p, out mensajeError);
        }
    }
}
