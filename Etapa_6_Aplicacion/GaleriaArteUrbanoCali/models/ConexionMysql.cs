using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using MySql.Data.MySqlClient;

namespace GaleriaArteUrbanoCali.models
{
    public class ConexionMysql : IDisposable
    {
        private readonly string _cadenaConexion;
        private MySqlConnection? _conexion;

        public string UltimoError { get; private set; } = string.Empty;

        public ConexionMysql(
            string servidor = "localhost",
            string baseDatos = "galeria_arte_urbano_db",
            string usuario = "root",
            string clave = "root",
            uint puerto = 3306)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = servidor,
                Database = baseDatos,
                UserID = usuario,
                Password = clave,
                Port = puerto,
                CharacterSet = "utf8mb4"
            };

            _cadenaConexion = builder.ConnectionString;
        }

        public MySqlConnection Abrir()
        {
            _conexion ??= new MySqlConnection(_cadenaConexion);
            if (_conexion.State != ConnectionState.Open)
            {
                _conexion.Open();
            }

            return _conexion;
        }

        public void Cerrar()
        {
            if (_conexion != null && _conexion.State != ConnectionState.Closed)
            {
                _conexion.Close();
            }
        }

        public void Dispose()
        {
            Cerrar();
            _conexion?.Dispose();
            _conexion = null;
        }

        public bool ProbarConexion(out string mensaje)
        {
            try
            {
                using var cn = new MySqlConnection(_cadenaConexion);
                cn.Open();
                mensaje = $"Conectado a MySQL {cn.ServerVersion} (BD: galeria_arte_urbano_db)";
                return true;
            }
            catch (MySqlException ex)
            {
                mensaje = $"Error MySQL ({ex.Number}): {ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                mensaje = $"Error general: {ex.Message}";
                return false;
            }
        }

        public bool ExecuteQuery(string sql, Dictionary<string, object?> parametros, out string mensajeError)
        {
            bool result = false;
            mensajeError = string.Empty;
            UltimoError = string.Empty;

            try
            {
                using MySqlCommand cmd = new MySqlCommand(sql, Abrir());
                if (parametros != null)
                {
                    foreach (var par in parametros)
                    {
                        cmd.Parameters.AddWithValue(par.Key, par.Value ?? DBNull.Value);
                    }
                }
                int filas = cmd.ExecuteNonQuery();
                result = filas >= 0;
            }
            catch (Exception ex)
            {
                UltimoError = ex.Message;
                mensajeError = ex.Message;
            }
            finally
            {
                Cerrar();
            }
            return result;
        }

        public bool ExecuteQuery(string sql, Dictionary<string, object?> parametros)
        {
            return ExecuteQuery(sql, parametros, out _);
        }

        public DataTable ExecuteDataTable(string sql, Dictionary<string, object?>? parametros = null)
        {
            DataTable dt = new DataTable();
            UltimoError = string.Empty;

            try
            {
                using MySqlCommand cmd = new MySqlCommand(sql, Abrir());
                if (parametros != null)
                {
                    foreach (var par in parametros)
                    {
                        cmd.Parameters.AddWithValue(par.Key, par.Value ?? DBNull.Value);
                    }
                }
                using MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                UltimoError = ex.Message;
            }
            finally
            {
                Cerrar();
            }

            return dt;
        }

        public object? ExecuteScalar(string sql, Dictionary<string, object?>? parametros = null)
        {
            object? result = null;
            UltimoError = string.Empty;

            try
            {
                using MySqlCommand cmd = new MySqlCommand(sql, Abrir());
                if (parametros != null)
                {
                    foreach (var par in parametros)
                    {
                        cmd.Parameters.AddWithValue(par.Key, par.Value ?? DBNull.Value);
                    }
                }
                result = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                UltimoError = ex.Message;
            }
            finally
            {
                Cerrar();
            }

            return result;
        }

        public static string ObtenerDirectorioRaizMultimedia()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo? dir = new DirectoryInfo(baseDir);

            while (dir != null)
            {
                string candidata = Path.Combine(dir.FullName, "RecursosMultimedia");
                if (Directory.Exists(candidata))
                {
                    return dir.FullName;
                }
                dir = dir.Parent;
            }

            return baseDir;
        }

        public static string ResolverRutaFisicaMultimedia(string carpetaBase, string rutaArchivo)
        {
            string raiz = ObtenerDirectorioRaizMultimedia();
            string combinada = Path.Combine(
                raiz,
                carpetaBase.Replace('/', Path.DirectorySeparatorChar),
                rutaArchivo.Replace('/', Path.DirectorySeparatorChar));
            return Path.GetFullPath(combinada);
        }
    }
}
