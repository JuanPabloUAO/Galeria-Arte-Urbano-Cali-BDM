using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace GaleriaArteUrbanoCali.models
{
    public class Ruta
    {
        public string IdRuta { get; set; } = string.Empty;
        public string NombreRuta { get; set; } = string.Empty;
        public string? DescripcionRuta { get; set; }
        public int DuracionEstimada { get; set; } = 120;
        public decimal DistanciaEstimada { get; set; } = 2.5m;
        public bool EstadoRuta { get; set; } = true;

        public string EtiquetaRuta => $"{NombreRuta} ({DuracionEstimada} min | {DistanciaEstimada:0.0} km)";

        public List<Ruta> GetRutas()
        {
            List<Ruta> lista = new List<Ruta>();
            using var conexion = new ConexionMysql();
            string sql = "SELECT * FROM ruta ORDER BY nombreRuta;";

            try
            {
                using MySqlCommand cmd = new MySqlCommand(sql, conexion.Abrir());
                using MySqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    lista.Add(new Ruta
                    {
                        IdRuta = r["idRuta"].ToString()!,
                        NombreRuta = r.GetString("nombreRuta"),
                        DescripcionRuta = r.IsDBNull(r.GetOrdinal("descripcionRuta")) ? null : r.GetString("descripcionRuta"),
                        DuracionEstimada = r.GetInt32("duracionEstimada"),
                        DistanciaEstimada = r.GetDecimal("distanciaEstimada"),
                        EstadoRuta = r.GetBoolean("estadoRuta")
                    });
                }
            }
            finally
            {
                conexion.Cerrar();
            }

            return lista;
        }

        public DataTable ConsultarTablaRutas()
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT r.idRuta AS 'ID',
                       r.nombreRuta AS 'Nombre de la Ruta',
                       r.duracionEstimada AS 'Duración (min)',
                       r.distanciaEstimada AS 'Distancia (km)',
                       CASE WHEN r.estadoRuta = 1 THEN 'Activa' ELSE 'Inactiva' END AS 'Estado',
                       COUNT(ro.idRutaObra) AS 'Total Paradas'
                FROM ruta r
                LEFT JOIN ruta_obra ro ON r.idRuta = ro.idRutaFK
                GROUP BY r.idRuta, r.nombreRuta, r.duracionEstimada, r.distanciaEstimada, r.estadoRuta
                ORDER BY r.nombreRuta;";
            return conexion.ExecuteDataTable(sql);
        }

        public bool InsertarRuta(Ruta r, out string mensajeError)
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                INSERT INTO ruta (idRuta, nombreRuta, descripcionRuta, duracionEstimada, distanciaEstimada, estadoRuta)
                VALUES (@id, @nombre, @desc, @dur, @dist, @estado);";

            var p = new Dictionary<string, object?>
            {
                { "@id", string.IsNullOrWhiteSpace(r.IdRuta) ? Guid.NewGuid().ToString() : r.IdRuta.Trim() },
                { "@nombre", r.NombreRuta.Trim() },
                { "@desc", string.IsNullOrWhiteSpace(r.DescripcionRuta) ? null : r.DescripcionRuta.Trim() },
                { "@dur", r.DuracionEstimada },
                { "@dist", r.DistanciaEstimada },
                { "@estado", r.EstadoRuta }
            };

            return conexion.ExecuteQuery(sql, p, out mensajeError);
        }
    }
}
