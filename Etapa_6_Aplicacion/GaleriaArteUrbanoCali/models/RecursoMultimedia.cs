using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace GaleriaArteUrbanoCali.models
{
    public class RecursoMultimedia
    {
        public string IdRecurso { get; set; } = string.Empty;
        public string NombreRecurso { get; set; } = string.Empty;
        public string? DescripcionRecurso { get; set; }
        public string RutaArchivo { get; set; } = string.Empty;
        public decimal Tamanio { get; set; }
        public int? Duracion { get; set; }
        public DateTime FechaRegistroRecurso { get; set; } = DateTime.Today;
        public string IdFormatoFK { get; set; } = string.Empty;
        public string IdObraFK { get; set; } = string.Empty;

        public string Extension { get; set; } = string.Empty;
        public string NombreTipoRecurso { get; set; } = string.Empty;
        public string CarpetaBase { get; set; } = string.Empty;
        public string NombreObra { get; set; } = string.Empty;

        public string RutaRelativaCompleta => $"{CarpetaBase}{RutaArchivo}";

        public DataTable ConsultarRecursosPorObra(string? idObra = null)
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT rm.idRecurso AS 'ID',
                       o.nombreObra AS 'Obra Asociada',
                       rm.nombreRecurso AS 'Nombre del Recurso',
                       tr.nombreTipoRecurso AS 'Tipo',
                       fr.extension AS 'Formato',
                       CONCAT(tr.carpetaBase, rm.rutaArchivo) AS 'Ruta Relativa',
                       rm.tamanio AS 'Tamaño (MB)',
                       IFNULL(CAST(rm.duracion AS CHAR), 'N/A') AS 'Duración (s)',
                       DATE_FORMAT(rm.fechaRegistroRecurso, '%Y-%m-%d') AS 'Fecha Registro',
                       tr.carpetaBase AS 'CarpetaBase',
                       rm.rutaArchivo AS 'ArchivoRelativo'
                FROM recurso_multimedia rm
                INNER JOIN obra o ON rm.idObraFK = o.idObra
                INNER JOIN formato_recurso fr ON rm.idFormatoFK = fr.idFormato
                INNER JOIN tipo_recurso tr ON fr.idTipoRecursoFK = tr.idTipoRecurso ";

            Dictionary<string, object?>? p = null;
            if (!string.IsNullOrWhiteSpace(idObra))
            {
                sql += "WHERE rm.idObraFK = @idObra ";
                p = new Dictionary<string, object?> { { "@idObra", idObra.Trim() } };
            }

            sql += "ORDER BY o.nombreObra, tr.nombreTipoRecurso, rm.nombreRecurso;";
            return conexion.ExecuteDataTable(sql, p);
        }

        // Invoca el procedimiento almacenado sp_asociar_recurso_multimedia desde C#
        public bool InsertarRecurso(RecursoMultimedia rm, out string mensajeError)
        {
            mensajeError = string.Empty;
            using var conexion = new ConexionMysql();

            try
            {
                using MySqlCommand cmd = new MySqlCommand("sp_asociar_recurso_multimedia", conexion.Abrir());
                cmd.CommandType = CommandType.StoredProcedure;

                string idEntrada = string.IsNullOrWhiteSpace(rm.IdRecurso) ? Guid.NewGuid().ToString() : rm.IdRecurso.Trim();

                cmd.Parameters.AddWithValue("@p_id_recurso", idEntrada);
                cmd.Parameters.AddWithValue("@p_id_obra", rm.IdObraFK.Trim());
                cmd.Parameters.AddWithValue("@p_id_formato", rm.IdFormatoFK.Trim());
                cmd.Parameters.AddWithValue("@p_nombre_recurso", rm.NombreRecurso.Trim());
                cmd.Parameters.AddWithValue("@p_descripcion", string.IsNullOrWhiteSpace(rm.DescripcionRecurso) ? DBNull.Value : rm.DescripcionRecurso.Trim());
                cmd.Parameters.AddWithValue("@p_ruta_archivo", rm.RutaArchivo.Trim());
                cmd.Parameters.AddWithValue("@p_tamanio_mb", rm.Tamanio);
                cmd.Parameters.AddWithValue("@p_duracion_seg", rm.Duracion.HasValue ? rm.Duracion.Value : DBNull.Value);

                var outParam = new MySqlParameter("@p_nuevo_id_recurso", MySqlDbType.String, 36)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outParam);

                cmd.ExecuteNonQuery();
                rm.IdRecurso = outParam.Value?.ToString() ?? idEntrada;
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

        public bool EliminarRecurso(string idRecurso, out string mensajeError)
        {
            using var conexion = new ConexionMysql();
            string sql = "DELETE FROM recurso_multimedia WHERE idRecurso = @id;";
            var p = new Dictionary<string, object?> { { "@id", idRecurso.Trim() } };
            return conexion.ExecuteQuery(sql, p, out mensajeError);
        }
    }
}
