using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace GaleriaArteUrbanoCali.models
{
    public class Ubicacion
    {
        public string IdUbicacion { get; set; } = string.Empty;
        public string NombreLugar { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public string IdBarrioFK { get; set; } = string.Empty;
        public string NombreBarrio { get; set; } = string.Empty;
        public int NumeroComuna { get; set; }

        public string EtiquetaCompleta => $"{NombreLugar} — {NombreBarrio} (Comuna {NumeroComuna})";

        public List<Ubicacion> GetUbicaciones()
        {
            List<Ubicacion> lista = new List<Ubicacion>();
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT u.idUbicacion, u.nombreLugar, u.direccion, u.latitud, u.longitud,
                       u.idBarrioFK, b.nombreBarrio, c.numeroComuna
                FROM ubicacion u
                INNER JOIN barrio b ON u.idBarrioFK = b.idBarrio
                INNER JOIN comuna c ON b.idComunaFK = c.idComuna
                ORDER BY c.numeroComuna, u.nombreLugar;";

            try
            {
                using MySqlCommand cmd = new MySqlCommand(sql, conexion.Abrir());
                using MySqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    lista.Add(MapearUbicacion(r));
                }
            }
            finally
            {
                conexion.Cerrar();
            }

            return lista;
        }

        public DataTable ConsultarTablaUbicaciones()
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT u.idUbicacion AS 'ID',
                       u.nombreLugar AS 'Nombre del Lugar',
                       u.direccion AS 'Dirección Física',
                       b.nombreBarrio AS 'Barrio',
                       CONCAT('Comuna ', c.numeroComuna) AS 'Comuna',
                       u.latitud AS 'Latitud',
                       u.longitud AS 'Longitud',
                       COUNT(o.idObra) AS 'Obras en el Lugar'
                FROM ubicacion u
                INNER JOIN barrio b ON u.idBarrioFK = b.idBarrio
                INNER JOIN comuna c ON b.idComunaFK = c.idComuna
                LEFT JOIN obra o ON u.idUbicacion = o.idUbicacionFK
                GROUP BY u.idUbicacion, u.nombreLugar, u.direccion, b.nombreBarrio,
                         c.numeroComuna, u.latitud, u.longitud
                ORDER BY c.numeroComuna, u.nombreLugar;";
            return conexion.ExecuteDataTable(sql);
        }

        public Ubicacion? BuscarPorId(string idUbicacion)
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT u.*, b.nombreBarrio, c.numeroComuna
                FROM ubicacion u
                INNER JOIN barrio b ON u.idBarrioFK = b.idBarrio
                INNER JOIN comuna c ON b.idComunaFK = c.idComuna
                WHERE u.idUbicacion = @id LIMIT 1;";

            try
            {
                using MySqlCommand cmd = new MySqlCommand(sql, conexion.Abrir());
                cmd.Parameters.AddWithValue("@id", idUbicacion.Trim());
                using MySqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    return MapearUbicacion(r);
                }
            }
            finally
            {
                conexion.Cerrar();
            }

            return null;
        }

        public bool InsertarUbicacion(Ubicacion u, out string mensajeError)
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                INSERT INTO ubicacion
                (idUbicacion, nombreLugar, direccion, latitud, longitud, idBarrioFK)
                VALUES
                (@id, @lugar, @dir, @lat, @lon, @idBarrio);";

            var p = new Dictionary<string, object?>
            {
                { "@id", string.IsNullOrWhiteSpace(u.IdUbicacion) ? Guid.NewGuid().ToString() : u.IdUbicacion.Trim() },
                { "@lugar", u.NombreLugar.Trim() },
                { "@dir", u.Direccion.Trim() },
                { "@lat", u.Latitud },
                { "@lon", u.Longitud },
                { "@idBarrio", u.IdBarrioFK.Trim() }
            };

            return conexion.ExecuteQuery(sql, p, out mensajeError);
        }

        public bool ActualizarUbicacion(Ubicacion u, out string mensajeError)
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                UPDATE ubicacion SET
                    nombreLugar = @lugar,
                    direccion = @dir,
                    latitud = @lat,
                    longitud = @lon,
                    idBarrioFK = @idBarrio
                WHERE idUbicacion = @id;";

            var p = new Dictionary<string, object?>
            {
                { "@id", u.IdUbicacion.Trim() },
                { "@lugar", u.NombreLugar.Trim() },
                { "@dir", u.Direccion.Trim() },
                { "@lat", u.Latitud },
                { "@lon", u.Longitud },
                { "@idBarrio", u.IdBarrioFK.Trim() }
            };

            return conexion.ExecuteQuery(sql, p, out mensajeError);
        }

        private static Ubicacion MapearUbicacion(MySqlDataReader r)
        {
            return new Ubicacion
            {
                IdUbicacion = r["idUbicacion"].ToString()!,
                NombreLugar = r.GetString("nombreLugar"),
                Direccion = r.GetString("direccion"),
                Latitud = r.GetDecimal("latitud"),
                Longitud = r.GetDecimal("longitud"),
                IdBarrioFK = r["idBarrioFK"].ToString()!,
                NombreBarrio = r.GetString("nombreBarrio"),
                NumeroComuna = r.GetInt32("numeroComuna")
            };
        }
    }
}
