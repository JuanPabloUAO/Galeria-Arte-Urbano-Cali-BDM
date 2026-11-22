using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace GaleriaArteUrbanoCali.models
{
    public class Comuna
    {
        public string IdComuna { get; set; } = string.Empty;
        public int NumeroComuna { get; set; }

        public string EtiquetaComuna => NumeroComuna > 0 ? $"Comuna {NumeroComuna}" : "-- Todas las comunas --";

        public Comuna() { }

        public Comuna(string idComuna, int numeroComuna)
        {
            IdComuna = idComuna;
            NumeroComuna = numeroComuna;
        }

        public List<Comuna> GetComunas()
        {
            List<Comuna> lista = new List<Comuna>();
            using var conexion = new ConexionMysql();
            string sql = "SELECT idComuna, numeroComuna FROM comuna ORDER BY numeroComuna;";

            try
            {
                using MySqlCommand cmd = new MySqlCommand(sql, conexion.Abrir());
                using MySqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    lista.Add(new Comuna(r["idComuna"].ToString()!, r.GetInt32("numeroComuna")));
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
