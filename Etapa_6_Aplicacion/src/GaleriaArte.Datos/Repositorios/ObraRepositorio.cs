using System.Collections.Generic;
using System.Data;
using GaleriaArte.Entidades;

namespace GaleriaArte.Datos.Repositorios;

public class ObraRepositorio : RepositorioBase
{
    // Se lee de la vista vw_obra_detalle, que ya trae los JOIN con artista, categoría, ubicación y barrio
    private const string SelectVista =
        @"SELECT id_obra, obra, descripcion, fecha_creacion, estado_conservacion, fecha_registro,
                 id_artista, artista, id_categoria, categoria, id_ubicacion, lugar, barrio, comuna,
                 id_usuario, registrado_por, total_recursos
          FROM vw_obra_detalle ";

    private static Obra Mapear(IDataRecord r) => new()
    {
        IdObra = Texto(r, "id_obra"),
        Nombre = Texto(r, "obra"),
        Descripcion = Texto(r, "descripcion"),
        FechaCreacion = FechaNula(r, "fecha_creacion"),
        EstadoConservacion = Texto(r, "estado_conservacion"),
        FechaRegistro = Fecha(r, "fecha_registro"),
        IdArtista = Texto(r, "id_artista"),
        Artista = Texto(r, "artista"),
        IdCategoria = Texto(r, "id_categoria"),
        Categoria = Texto(r, "categoria"),
        IdUbicacion = Texto(r, "id_ubicacion"),
        Lugar = Texto(r, "lugar"),
        Barrio = Texto(r, "barrio"),
        Comuna = Entero(r, "comuna"),
        IdUsuario = Texto(r, "id_usuario"),
        RegistradoPor = Texto(r, "registrado_por"),
        TotalRecursos = Entero(r, "total_recursos"),
    };

    /// <summary>
    /// Búsqueda con filtros combinables. Responde a las consultas "obras por artista",
    /// "por categoría" y "por ubicación (barrio o comuna)". El filtro por nombre usa
    /// LIKE 'texto%' para aprovechar el índice idx_obra_nombre.
    /// </summary>
    public List<Obra> Buscar(FiltroObra? f = null)
    {
        f ??= new FiltroObra();
        return Consultar(SelectVista +
            @"WHERE (@nombre IS NULL OR obra LIKE CONCAT(@nombre, '%'))
                AND (@artista IS NULL OR id_artista = @artista)
                AND (@categoria IS NULL OR id_categoria = @categoria)
                AND (@barrio IS NULL OR id_barrio = @barrio)
                AND (@comuna IS NULL OR comuna = @comuna)
              ORDER BY obra",
            Mapear,
            P("@nombre", string.IsNullOrWhiteSpace(f.NombreEmpiezaPor) ? null : f.NombreEmpiezaPor.Trim()),
            P("@artista", f.IdArtista), P("@categoria", f.IdCategoria), P("@barrio", f.IdBarrio), P("@comuna", f.Comuna));
    }

    public Obra? ObtenerPorId(string idObra) =>
        ConsultarUno(SelectVista + "WHERE id_obra = @id", Mapear, P("@id", idObra));

    public void Insertar(Obra o) => EjecutarComando(
        @"INSERT INTO obra (id_obra, nombre, descripcion, fecha_creacion, estado_conservacion,
                            id_artistaFK, id_categoriaFK, id_ubicacionFK, id_usuarioFK)
          VALUES (@id, @nombre, @desc, @fecha, @estado, @artista, @categoria, @ubicacion, @usuario)",
        P("@id", o.IdObra), P("@nombre", o.Nombre), P("@desc", o.Descripcion), P("@fecha", o.FechaCreacion),
        P("@estado", o.EstadoConservacion), P("@artista", o.IdArtista), P("@categoria", o.IdCategoria),
        P("@ubicacion", o.IdUbicacion), P("@usuario", o.IdUsuario));

    public void Actualizar(Obra o) => EjecutarComando(
        @"UPDATE obra SET nombre = @nombre, descripcion = @desc, fecha_creacion = @fecha, estado_conservacion = @estado,
                 id_artistaFK = @artista, id_categoriaFK = @categoria, id_ubicacionFK = @ubicacion
          WHERE id_obra = @id",
        P("@nombre", o.Nombre), P("@desc", o.Descripcion), P("@fecha", o.FechaCreacion), P("@estado", o.EstadoConservacion),
        P("@artista", o.IdArtista), P("@categoria", o.IdCategoria), P("@ubicacion", o.IdUbicacion), P("@id", o.IdObra));

    /// <summary>Elimina la obra. Sus recursos se borran por ON DELETE CASCADE; si está en una ruta, MySQL lo impide (RESTRICT).</summary>
    public void Eliminar(string idObra) =>
        EjecutarComando("DELETE FROM obra WHERE id_obra = @id", P("@id", idObra));

    public List<string> RutasQueContienen(string idObra) => Consultar(
        @"SELECT r.nombre FROM ruta r INNER JOIN ruta_obra ro ON ro.id_rutaFK = r.id_ruta
          WHERE ro.id_obraFK = @id ORDER BY r.nombre",
        r => Texto(r, "nombre"), P("@id", idObra));
}
