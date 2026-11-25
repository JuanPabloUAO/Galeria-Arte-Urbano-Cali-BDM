using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace GaleriaArteUrbanoCali.models
{
    public class Pais
    {
        public string IdPais { get; set; } = string.Empty;
        public string NombrePais { get; set; } = string.Empty;

        public Pais() { }

        public Pais(string idPais, string nombrePais)
        {
            IdPais = idPais;
            NombrePais = nombrePais;
        }

        public List<Pais> GetPaises()
        {
            List<Pais> lista = new List<Pais>();
            using var conexion = new ConexionMysql();
            string sql = "SELECT idPais, nombrePais FROM pais ORDER BY nombrePais;";

            try
            {
                using MySqlCommand cmd = new MySqlCommand(sql, conexion.Abrir());
                using MySqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    lista.Add(new Pais(r["idPais"].ToString()!, r.GetString("nombrePais")));
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
