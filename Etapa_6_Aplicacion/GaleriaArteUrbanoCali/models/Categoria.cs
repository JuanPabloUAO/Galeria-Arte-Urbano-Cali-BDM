using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace GaleriaArteUrbanoCali.models
{
    public class Categoria
    {
        public string IdCategoria { get; set; } = string.Empty;
        public string NombreCategoria { get; set; } = string.Empty;
        public string? DescripcionCategoria { get; set; }
        public bool EstadoCategoria { get; set; } = true;

        public List<Categoria> GetCategoriasActivas()
        {
            List<Categoria> lista = new List<Categoria>();
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT idCategoria, nombreCategoria, descripcionCategoria, estadoCategoria
                FROM categoria
                WHERE estadoCategoria = TRUE
                ORDER BY nombreCategoria;";

            try
            {
                using MySqlCommand cmd = new MySqlCommand(sql, conexion.Abrir());
                using MySqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    lista.Add(MapearCategoria(r));
                }
            }
            finally
            {
                conexion.Cerrar();
            }

            return lista;
        }

        public DataTable ConsultarTablaCategorias()
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT c.idCategoria AS 'ID',
                       c.nombreCategoria AS 'Nombre Categoría',
                       IFNULL(c.descripcionCategoria, '') AS 'Descripción',
                       CASE WHEN c.estadoCategoria = 1 THEN 'Activa' ELSE 'Inactiva' END AS 'Estado',
                       COUNT(o.idObra) AS 'Obras Asociadas'
                FROM categoria c
                LEFT JOIN obra o ON c.idCategoria = o.idCategoriaFK
                GROUP BY c.idCategoria, c.nombreCategoria, c.descripcionCategoria, c.estadoCategoria
                ORDER BY c.nombreCategoria;";
            return conexion.ExecuteDataTable(sql);
        }

        public Categoria? BuscarPorId(string idCategoria)
        {
            using var conexion = new ConexionMysql();
            string sql = "SELECT * FROM categoria WHERE idCategoria = @id LIMIT 1;";

            try
            {
                using MySqlCommand cmd = new MySqlCommand(sql, conexion.Abrir());
                cmd.Parameters.AddWithValue("@id", idCategoria.Trim());
                using MySqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    return MapearCategoria(r);
                }
            }
            finally
            {
                conexion.Cerrar();
            }

            return null;
        }

        public bool InsertarCategoria(Categoria c, out string mensajeError)
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                INSERT INTO categoria (idCategoria, nombreCategoria, descripcionCategoria, estadoCategoria)
                VALUES (@id, @nombre, @desc, @estado);";

            var p = new Dictionary<string, object?>
            {
                { "@id", string.IsNullOrWhiteSpace(c.IdCategoria) ? Guid.NewGuid().ToString() : c.IdCategoria.Trim() },
                { "@nombre", c.NombreCategoria.Trim() },
                { "@desc", string.IsNullOrWhiteSpace(c.DescripcionCategoria) ? null : c.DescripcionCategoria.Trim() },
                { "@estado", c.EstadoCategoria }
            };

            return conexion.ExecuteQuery(sql, p, out mensajeError);
        }

        public bool ActualizarCategoria(Categoria c, out string mensajeError)
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                UPDATE categoria SET
                    nombreCategoria = @nombre,
                    descripcionCategoria = @desc,
                    estadoCategoria = @estado
                WHERE idCategoria = @id;";

            var p = new Dictionary<string, object?>
            {
                { "@id", c.IdCategoria.Trim() },
                { "@nombre", c.NombreCategoria.Trim() },
                { "@desc", string.IsNullOrWhiteSpace(c.DescripcionCategoria) ? null : c.DescripcionCategoria.Trim() },
                { "@estado", c.EstadoCategoria }
            };

            return conexion.ExecuteQuery(sql, p, out mensajeError);
        }

        public bool CambiarEstadoCategoria(string idCategoria, bool nuevoEstado, out string mensajeError)
        {
            using var conexion = new ConexionMysql();
            string sql = "UPDATE categoria SET estadoCategoria = @estado WHERE idCategoria = @id;";
            var p = new Dictionary<string, object?>
            {
                { "@id", idCategoria.Trim() },
                { "@estado", nuevoEstado }
            };
            return conexion.ExecuteQuery(sql, p, out mensajeError);
        }

        private static Categoria MapearCategoria(MySqlDataReader r)
        {
            return new Categoria
            {
                IdCategoria = r["idCategoria"].ToString()!,
                NombreCategoria = r.GetString("nombreCategoria"),
                DescripcionCategoria = r.IsDBNull(r.GetOrdinal("descripcionCategoria")) ? null : r.GetString("descripcionCategoria"),
                EstadoCategoria = r.GetBoolean("estadoCategoria")
            };
        }
    }
}
