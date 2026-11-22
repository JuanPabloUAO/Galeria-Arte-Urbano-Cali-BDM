using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace GaleriaArteUrbanoCali.models
{
    public class TipoRecurso
    {
        public string IdTipoRecurso { get; set; } = string.Empty;
        public string NombreTipoRecurso { get; set; } = string.Empty;
        public string CarpetaBase { get; set; } = string.Empty;

        public string EtiquetaTipo => $"{NombreTipoRecurso} ({CarpetaBase})";

        public List<TipoRecurso> GetTiposRecurso()
        {
            List<TipoRecurso> lista = new List<TipoRecurso>();
            using var conexion = new ConexionMysql();
            string sql = "SELECT idTipoRecurso, nombreTipoRecurso, carpetaBase FROM tipo_recurso ORDER BY nombreTipoRecurso;";

            try
            {
                using MySqlCommand cmd = new MySqlCommand(sql, conexion.Abrir());
                using MySqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    lista.Add(new TipoRecurso
                    {
                        IdTipoRecurso = r["idTipoRecurso"].ToString()!,
                        NombreTipoRecurso = r.GetString("nombreTipoRecurso"),
                        CarpetaBase = r.GetString("carpetaBase")
                    });
                }
            }
            finally
            {
                conexion.Cerrar();
            }

            return lista;
        }
    }
}
