using System;
using MySql.Data.MySqlClient;

namespace GaleriaArte.Datos;

/// <summary>
/// Guarda la cadena de conexión (leída de appsettings.json por la interfaz al iniciar)
/// y crea conexiones abiertas. Cada operación abre y cierra su propia conexión;
/// MySql.Data reutiliza las conexiones internamente (connection pooling).
/// </summary>
public static class Conexion
{
    private static string? _cadena;

    public static void Configurar(string cadenaConexion)
    {
        if (string.IsNullOrWhiteSpace(cadenaConexion))
            throw new ArgumentException("La cadena de conexión está vacía. Revise appsettings.json.");
        _cadena = cadenaConexion;
    }

    internal static MySqlConnection Abrir()
    {
        if (_cadena is null)
            throw new InvalidOperationException("La conexión no ha sido configurada. Llame a Conexion.Configurar al iniciar.");
        var conexion = new MySqlConnection(_cadena);
        conexion.Open();
        return conexion;
    }

    /// <summary>Verifica al iniciar la aplicación que MySQL responde y que la base existe.</summary>
    public static void Probar()
    {
        try
        {
            using var conexion = Abrir();
            using var comando = new MySqlCommand("SELECT COUNT(*) FROM usuario", conexion);
            comando.ExecuteScalar();
        }
        catch (MySqlException ex)
        {
            throw new DatosException(TraductorErrores.Traducir(ex), ex);
        }
    }
}
