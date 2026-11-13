using System;
using System.Collections.Generic;
using System.Data;
using GaleriaArte.Entidades;

namespace GaleriaArte.Datos.Repositorios;

public class UbicacionRepositorio : RepositorioBase
{
    private static Ubicacion Mapear(IDataRecord r) => new()
    {
        IdUbicacion = Texto(r, "id_ubicacion"),
        NombreLugar = Texto(r, "nombre_lugar"),
        Direccion = Texto(r, "direccion"),
        IdBarrio = Entero(r, "id_barrioFK"),
        Latitud = Numero(r, "latitud"),
        Longitud = Numero(r, "longitud"),
        NombreBarrio = Texto(r, "barrio"),
        Comuna = Entero(r, "comuna"),
    };

    public List<Ubicacion> Listar(string? filtro = null) => Consultar(
        @"SELECT u.id_ubicacion, u.nombre_lugar, u.direccion, u.id_barrioFK, u.latitud, u.longitud,
                 b.nombre AS barrio, b.comuna
          FROM ubicacion u
          INNER JOIN barrio b ON b.id_barrio = u.id_barrioFK
          WHERE (@filtro IS NULL OR u.nombre_lugar LIKE CONCAT('%', @filtro, '%')
                 OR u.direccion LIKE CONCAT('%', @filtro, '%') OR b.nombre LIKE CONCAT('%', @filtro, '%'))
          ORDER BY b.comuna, b.nombre, u.nombre_lugar",
        Mapear, P("@filtro", string.IsNullOrWhiteSpace(filtro) ? null : filtro.Trim()));

    public void Insertar(Ubicacion u) => EjecutarComando(
        @"INSERT INTO ubicacion (id_ubicacion, nombre_lugar, direccion, id_barrioFK, latitud, longitud)
          VALUES (@id, @lugar, @dir, @barrio, @lat, @lon)",
        P("@id", u.IdUbicacion), P("@lugar", u.NombreLugar), P("@dir", u.Direccion),
        P("@barrio", u.IdBarrio), P("@lat", u.Latitud), P("@lon", u.Longitud));

    public void Actualizar(Ubicacion u) => EjecutarComando(
        @"UPDATE ubicacion SET nombre_lugar = @lugar, direccion = @dir, id_barrioFK = @barrio, latitud = @lat, longitud = @lon
          WHERE id_ubicacion = @id",
        P("@lugar", u.NombreLugar), P("@dir", u.Direccion), P("@barrio", u.IdBarrio),
        P("@lat", u.Latitud), P("@lon", u.Longitud), P("@id", u.IdUbicacion));

    // ---------- Barrios (tabla de apoyo) ----------

    private static Barrio MapearBarrio(IDataRecord r) => new()
    {
        IdBarrio = Entero(r, "id_barrio"),
        Nombre = Texto(r, "nombre"),
        Comuna = Entero(r, "comuna"),
    };

    public List<Barrio> ListarBarrios() =>
        Consultar("SELECT id_barrio, nombre, comuna FROM barrio ORDER BY comuna, nombre", MapearBarrio);

    public List<int> ListarComunasConBarrios() =>
        Consultar("SELECT DISTINCT comuna FROM barrio ORDER BY comuna", r => Entero(r, "comuna"));

    /// <summary>Inserta un barrio y devuelve el id AUTO_INCREMENT generado.</summary>
    public int InsertarBarrio(Barrio b) => Convert.ToInt32(Escalar(
        "INSERT INTO barrio (nombre, comuna) VALUES (@nombre, @comuna); SELECT LAST_INSERT_ID();",
        P("@nombre", b.Nombre), P("@comuna", b.Comuna)));
}
