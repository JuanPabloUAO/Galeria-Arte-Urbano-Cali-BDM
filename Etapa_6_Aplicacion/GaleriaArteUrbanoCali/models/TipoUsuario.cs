using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace GaleriaArteUrbanoCali.models
{
    public class TipoUsuario
    {
        public string IdTipoUsuario { get; set; } = string.Empty;
        public string NombreTipoUsuario { get; set; } = string.Empty;

        public TipoUsuario() { }

        public TipoUsuario(string idTipoUsuario, string nombreTipoUsuario)
        {
            IdTipoUsuario = idTipoUsuario;
            NombreTipoUsuario = nombreTipoUsuario;
        }

        public List<TipoUsuario> GetTiposUsuario()
        {
            List<TipoUsuario> lista = new List<TipoUsuario>();
            using var conexion = new ConexionMysql();
            string sql = "SELECT idTipoUsuario, nombreTipoUsuario FROM tipo_usuario ORDER BY nombreTipoUsuario;";

            try
            {
                using MySqlCommand cmd = new MySqlCommand(sql, conexion.Abrir());
                using MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new TipoUsuario(
                        reader["idTipoUsuario"].ToString()!,
                        reader.GetString("nombreTipoUsuario")));
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
