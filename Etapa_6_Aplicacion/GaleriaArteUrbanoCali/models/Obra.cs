using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace GaleriaArteUrbanoCali.models
{
    public class Obra
    {
        public string IdObra { get; set; } = string.Empty;
        public string NombreObra { get; set; } = string.Empty;
        public string? DescripcionObra { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Today;
        public string EstadoConservacion { get; set; } = "Excelente";
        public DateTime FechaRegistroObra { get; set; } = DateTime.Today;
        public string IdArtistaFK { get; set; } = string.Empty;
        public string IdCategoriaFK { get; set; } = string.Empty;
        public string IdUbicacionFK { get; set; } = string.Empty;
        public string IdUsuarioFK { get; set; } = string.Empty;

        public string NombreArtistico { get; set; } = string.Empty;
        public string EtiquetaObra => $"{NombreObra} ({NombreArtistico})";

        public List<Obra> GetListaObras()
        {
            List<Obra> lista = new List<Obra>();
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT o.*, a.nombreArtistico
                FROM obra o
                INNER JOIN artista a ON o.idArtistaFK = a.idArtista
                ORDER BY o.nombreObra;";

            try
            {
                using MySqlCommand cmd = new MySqlCommand(sql, conexion.Abrir());
                using MySqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    lista.Add(MapearObra(r));
                }
            }
            finally
            {
                conexion.Cerrar();
            }

            return lista;
        }

        // Consulta la vista vw_catalogo_obras_completo desde C#
        public DataTable ConsultarCatalogoObras()
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT idObra AS 'ID',
                       titulo_obra AS 'Título de la Obra',
                       artista AS 'Artista',
                       pais_artista AS 'País',
                       categoria_tecnica AS 'Categoría',
                       nombreLugar AS 'Lugar',
                       barrio AS 'Barrio',
                       comuna AS 'Comuna',
                       estadoConservacion AS 'Conservación',
                       DATE_FORMAT(fechaCreacion, '%Y-%m-%d') AS 'Creación',
                       registrado_por AS 'Registrado Por'
                FROM vw_catalogo_obras_completo
                ORDER BY titulo_obra;";
            return conexion.ExecuteDataTable(sql);
        }

        public Obra? BuscarPorId(string idObra)
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT o.*, a.nombreArtistico
                FROM obra o
                INNER JOIN artista a ON o.idArtistaFK = a.idArtista
                WHERE o.idObra = @id LIMIT 1;";

            try
            {
                using MySqlCommand cmd = new MySqlCommand(sql, conexion.Abrir());
                cmd.Parameters.AddWithValue("@id", idObra.Trim());
                using MySqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    return MapearObra(r);
                }
            }
            finally
            {
                conexion.Cerrar();
            }

            return null;
        }

        // Invoca el procedimiento almacenado sp_registrar_obra desde C#
        public bool InsertarObra(Obra o, out string mensajeError)
        {
            mensajeError = string.Empty;
            using var conexion = new ConexionMysql();

            try
            {
                using MySqlCommand cmd = new MySqlCommand("sp_registrar_obra", conexion.Abrir());
                cmd.CommandType = CommandType.StoredProcedure;

                string idEntrada = string.IsNullOrWhiteSpace(o.IdObra) ? Guid.NewGuid().ToString() : o.IdObra.Trim();

                cmd.Parameters.AddWithValue("@p_id_obra", idEntrada);
                cmd.Parameters.AddWithValue("@p_nombre_obra", o.NombreObra.Trim());
                cmd.Parameters.AddWithValue("@p_descripcion_obra", string.IsNullOrWhiteSpace(o.DescripcionObra) ? DBNull.Value : o.DescripcionObra.Trim());
                cmd.Parameters.AddWithValue("@p_fecha_creacion", o.FechaCreacion.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@p_estado_conservacion", o.EstadoConservacion.Trim());
                cmd.Parameters.AddWithValue("@p_id_artista", o.IdArtistaFK.Trim());
                cmd.Parameters.AddWithValue("@p_id_categoria", o.IdCategoriaFK.Trim());
                cmd.Parameters.AddWithValue("@p_id_ubicacion", o.IdUbicacionFK.Trim());
                cmd.Parameters.AddWithValue("@p_id_usuario", o.IdUsuarioFK.Trim());

                var outParam = new MySqlParameter("@p_nuevo_id_obra", MySqlDbType.String, 36)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outParam);

                cmd.ExecuteNonQuery();
                o.IdObra = outParam.Value?.ToString() ?? idEntrada;
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

        public bool ActualizarObra(Obra o, out string mensajeError)
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                UPDATE obra SET
                    nombreObra = @nombre,
                    descripcionObra = @desc,
                    fechaCreacion = @fechaCrea,
                    estadoConservacion = @estado,
                    idArtistaFK = @idArt,
                    idCategoriaFK = @idCat,
                    idUbicacionFK = @idUbi
                WHERE idObra = @id;";

            var p = new Dictionary<string, object?>
            {
                { "@id", o.IdObra.Trim() },
                { "@nombre", o.NombreObra.Trim() },
                { "@desc", string.IsNullOrWhiteSpace(o.DescripcionObra) ? null : o.DescripcionObra.Trim() },
                { "@fechaCrea", o.FechaCreacion.ToString("yyyy-MM-dd") },
                { "@estado", o.EstadoConservacion.Trim() },
                { "@idArt", o.IdArtistaFK.Trim() },
                { "@idCat", o.IdCategoriaFK.Trim() },
                { "@idUbi", o.IdUbicacionFK.Trim() }
            };

            return conexion.ExecuteQuery(sql, p, out mensajeError);
        }

        public bool RegistrarObraConRecursoPrincipalSP(
            Obra o,
            string idFormato,
            string nombreRecurso,
            string rutaArchivoRelativa,
            decimal tamanioMb,
            out string mensajeError)
        {
            mensajeError = string.Empty;
            using var conexion = new ConexionMysql();

            try
            {
                using MySqlCommand cmd = new MySqlCommand("sp_registrar_obra_con_recurso_principal", conexion.Abrir());
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@p_nombre_obra", o.NombreObra.Trim());
                cmd.Parameters.AddWithValue("@p_descripcion_obra", string.IsNullOrWhiteSpace(o.DescripcionObra) ? DBNull.Value : o.DescripcionObra.Trim());
                cmd.Parameters.AddWithValue("@p_fecha_creacion", o.FechaCreacion.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@p_estado_conservacion", o.EstadoConservacion.Trim());
                cmd.Parameters.AddWithValue("@p_id_artista", o.IdArtistaFK.Trim());
                cmd.Parameters.AddWithValue("@p_id_categoria", o.IdCategoriaFK.Trim());
                cmd.Parameters.AddWithValue("@p_id_ubicacion", o.IdUbicacionFK.Trim());
                cmd.Parameters.AddWithValue("@p_id_usuario", o.IdUsuarioFK.Trim());
                cmd.Parameters.AddWithValue("@p_id_formato", idFormato.Trim());
                cmd.Parameters.AddWithValue("@p_nombre_recurso", nombreRecurso.Trim());
                cmd.Parameters.AddWithValue("@p_ruta_archivo", rutaArchivoRelativa.Trim());
                cmd.Parameters.AddWithValue("@p_tamanio_mb", tamanioMb);

                var outObra = new MySqlParameter("@p_id_obra_creada", MySqlDbType.String, 36) { Direction = ParameterDirection.Output };
                var outRec = new MySqlParameter("@p_id_recurso_creado", MySqlDbType.String, 36) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outObra);
                cmd.Parameters.Add(outRec);

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

        private static Obra MapearObra(MySqlDataReader r)
        {
            return new Obra
            {
                IdObra = r["idObra"].ToString()!,
                NombreObra = r.GetString("nombreObra"),
                DescripcionObra = r.IsDBNull(r.GetOrdinal("descripcionObra")) ? null : r.GetString("descripcionObra"),
                FechaCreacion = r.GetDateTime("fechaCreacion"),
                EstadoConservacion = r.GetString("estadoConservacion"),
                FechaRegistroObra = r.GetDateTime("fechaRegistroObra"),
                IdArtistaFK = r["idArtistaFK"].ToString()!,
                IdCategoriaFK = r["idCategoriaFK"].ToString()!,
                IdUbicacionFK = r["idUbicacionFK"].ToString()!,
                IdUsuarioFK = r["idUsuarioFK"].ToString()!,
                NombreArtistico = r.GetString("nombreArtistico")
            };
        }
    }
}
