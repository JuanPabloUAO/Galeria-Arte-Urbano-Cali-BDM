using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace GaleriaArteUrbanoCali.models
{
    public class Barrio
    {
        public string IdBarrio { get; set; } = string.Empty;
        public string NombreBarrio { get; set; } = string.Empty;
        public string IdComunaFK { get; set; } = string.Empty;
        public int NumeroComuna { get; set; }

        public string EtiquetaBarrioComuna => NumeroComuna > 0 ? $"{NombreBarrio} (Comuna {NumeroComuna})" : NombreBarrio;

        public List<Barrio> GetBarrios()
        {
            List<Barrio> lista = new List<Barrio>();
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT b.idBarrio, b.nombreBarrio, b.idComunaFK, c.numeroComuna
                FROM barrio b
                INNER JOIN comuna c ON b.idComunaFK = c.idComuna
                ORDER BY c.numeroComuna, b.nombreBarrio;";

            try
            {
                using MySqlCommand cmd = new MySqlCommand(sql, conexion.Abrir());
                using MySqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    lista.Add(new Barrio
                    {
                        IdBarrio = r["idBarrio"].ToString()!,
                        NombreBarrio = r.GetString("nombreBarrio"),
                        IdComunaFK = r["idComunaFK"].ToString()!,
                        NumeroComuna = r.GetInt32("numeroComuna")
                    });
                }
            }
            finally
            {
                conexion.Cerrar();
            }

            return lista;
        }

        public List<Barrio> GetBarriosPorComuna(string idComuna)
        {
            List<Barrio> lista = new List<Barrio>();
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT b.idBarrio, b.nombreBarrio, b.idComunaFK, c.numeroComuna
                FROM barrio b
                INNER JOIN comuna c ON b.idComunaFK = c.idComuna
                WHERE b.idComunaFK = @idComuna
                ORDER BY b.nombreBarrio;";

            try
            {
                using MySqlCommand cmd = new MySqlCommand(sql, conexion.Abrir());
                cmd.Parameters.AddWithValue("@idComuna", idComuna.Trim());
                using MySqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    lista.Add(new Barrio
                    {
                        IdBarrio = r["idBarrio"].ToString()!,
                        NombreBarrio = r.GetString("nombreBarrio"),
                        IdComunaFK = r["idComunaFK"].ToString()!,
                        NumeroComuna = r.GetInt32("numeroComuna")
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
