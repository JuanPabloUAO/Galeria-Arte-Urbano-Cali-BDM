using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace GaleriaArte.Datos;

/// <summary>
/// Clase base de todos los repositorios. Concentra el código repetitivo de ADO.NET:
/// abrir la conexión, crear el comando con PARÁMETROS (nunca concatenando texto → sin inyección SQL),
/// leer resultados y traducir los errores de MySQL a mensajes entendibles.
/// </summary>
public abstract class RepositorioBase
{
    /// <summary>Crea un parámetro. Si el valor es null en C#, se envía NULL a MySQL.</summary>
    protected static MySqlParameter P(string nombre, object? valor) =>
        new MySqlParameter(nombre, valor ?? DBNull.Value);

    /// <summary>Ejecuta un SELECT y convierte cada fila con la función <paramref name="mapear"/>.</summary>
    protected static List<T> Consultar<T>(string sql, Func<IDataRecord, T> mapear, params MySqlParameter[] parametros)
    {
        var lista = new List<T>();
        Ejecutar(() =>
        {
            using var conexion = Conexion.Abrir();
            using var comando = new MySqlCommand(sql, conexion);
            comando.Parameters.AddRange(parametros);
            using var lector = comando.ExecuteReader();
            while (lector.Read())
                lista.Add(mapear(lector));
        });
        return lista;
    }

    /// <summary>Ejecuta un SELECT que debe devolver como máximo una fila.</summary>
    protected static T? ConsultarUno<T>(string sql, Func<IDataRecord, T> mapear, params MySqlParameter[] parametros) where T : class
    {
        var lista = Consultar(sql, mapear, parametros);
        return lista.Count > 0 ? lista[0] : null;
    }

    /// <summary>Ejecuta INSERT / UPDATE / DELETE y devuelve las filas afectadas.</summary>
    protected static int EjecutarComando(string sql, params MySqlParameter[] parametros)
    {
        int filas = 0;
        Ejecutar(() =>
        {
            using var conexion = Conexion.Abrir();
            using var comando = new MySqlCommand(sql, conexion);
            comando.Parameters.AddRange(parametros);
            filas = comando.ExecuteNonQuery();
        });
        return filas;
    }

    /// <summary>Ejecuta una consulta que devuelve un solo valor (COUNT, LAST_INSERT_ID...).</summary>
    protected static object? Escalar(string sql, params MySqlParameter[] parametros)
    {
        object? valor = null;
        Ejecutar(() =>
        {
            using var conexion = Conexion.Abrir();
            using var comando = new MySqlCommand(sql, conexion);
            comando.Parameters.AddRange(parametros);
            valor = comando.ExecuteScalar();
        });
        return valor == DBNull.Value ? null : valor;
    }

    /// <summary>Llama a un procedimiento almacenado. Los parámetros OUT se leen después de la llamada.</summary>
    protected static void EjecutarProcedimiento(string nombre, params MySqlParameter[] parametros)
    {
        Ejecutar(() =>
        {
            using var conexion = Conexion.Abrir();
            using var comando = new MySqlCommand(nombre, conexion) { CommandType = CommandType.StoredProcedure };
            comando.Parameters.AddRange(parametros);
            comando.ExecuteNonQuery();
        });
    }

    /// <summary>Envuelve la operación y traduce MySqlException a DatosException con un mensaje claro.</summary>
    private static void Ejecutar(Action operacion)
    {
        try
        {
            operacion();
        }
        catch (MySqlException ex)
        {
            throw new DatosException(TraductorErrores.Traducir(ex), ex);
        }
    }

    // ---------- Lectura segura de columnas (maneja NULL y tipos de MySQL) ----------

    // Convert.ToString funciona tanto si el conector devuelve CHAR(36) como string o como Guid.
    protected static string Texto(IDataRecord r, string columna) =>
        r[columna] == DBNull.Value ? string.Empty : Convert.ToString(r[columna]) ?? string.Empty;

    protected static string? TextoNulo(IDataRecord r, string columna) =>
        r[columna] == DBNull.Value ? null : Convert.ToString(r[columna]);

    protected static int Entero(IDataRecord r, string columna) =>
        r[columna] == DBNull.Value ? 0 : Convert.ToInt32(r[columna]);

    protected static int? EnteroNulo(IDataRecord r, string columna) =>
        r[columna] == DBNull.Value ? null : Convert.ToInt32(r[columna]);

    protected static long Largo(IDataRecord r, string columna) =>
        r[columna] == DBNull.Value ? 0 : Convert.ToInt64(r[columna]);

    protected static decimal Numero(IDataRecord r, string columna) =>
        r[columna] == DBNull.Value ? 0m : Convert.ToDecimal(r[columna]);

    protected static DateTime Fecha(IDataRecord r, string columna) =>
        r[columna] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(r[columna]);

    protected static DateTime? FechaNula(IDataRecord r, string columna) =>
        r[columna] == DBNull.Value ? null : Convert.ToDateTime(r[columna]);
}
