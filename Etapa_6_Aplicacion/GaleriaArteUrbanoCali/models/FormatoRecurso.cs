using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace GaleriaArteUrbanoCali.models
{
    public class FormatoRecurso
    {
        public string IdFormato { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public string IdTipoRecursoFK { get; set; } = string.Empty;
        public string NombreTipoRecurso { get; set; } = string.Empty;
        public string CarpetaBase { get; set; } = string.Empty;

        public string EtiquetaFormato => $"{Extension} — {NombreTipoRecurso}";

        public List<FormatoRecurso> GetFormatos()
        {
            List<FormatoRecurso> lista = new List<FormatoRecurso>();
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT f.idFormato, f.extension, f.idTipoRecursoFK,
                       t.nombreTipoRecurso, t.carpetaBase
                FROM formato_recurso f
                INNER JOIN tipo_recurso t ON f.idTipoRecursoFK = t.idTipoRecurso
                ORDER BY t.nombreTipoRecurso, f.extension;";

            try
            {
                using MySqlCommand cmd = new MySqlCommand(sql, conexion.Abrir());
                using MySqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    lista.Add(new FormatoRecurso
                    {
                        IdFormato = r["idFormato"].ToString()!,
                        Extension = r.GetString("extension"),
                        IdTipoRecursoFK = r["idTipoRecursoFK"].ToString()!,
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

        public List<FormatoRecurso> GetFormatosPorTipo(string idTipoRecurso)
        {
            List<FormatoRecurso> lista = new List<FormatoRecurso>();
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT f.idFormato, f.extension, f.idTipoRecursoFK,
                       t.nombreTipoRecurso, t.carpetaBase
                FROM formato_recurso f
                INNER JOIN tipo_recurso t ON f.idTipoRecursoFK = t.idTipoRecurso
                WHERE f.idTipoRecursoFK = @idTipo
                ORDER BY f.extension;";

            try
            {
                using MySqlCommand cmd = new MySqlCommand(sql, conexion.Abrir());
                cmd.Parameters.AddWithValue("@idTipo", idTipoRecurso.Trim());
                using MySqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    lista.Add(new FormatoRecurso
                    {
                        IdFormato = r["idFormato"].ToString()!,
                        Extension = r.GetString("extension"),
                        IdTipoRecursoFK = r["idTipoRecursoFK"].ToString()!,
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
