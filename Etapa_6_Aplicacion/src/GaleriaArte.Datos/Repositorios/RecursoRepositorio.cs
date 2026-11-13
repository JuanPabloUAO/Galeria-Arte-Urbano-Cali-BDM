using System.Collections.Generic;
using System.Data;
using GaleriaArte.Entidades;

namespace GaleriaArte.Datos.Repositorios;

public class RecursoRepositorio : RepositorioBase
{
    private static RecursoMultimedia Mapear(IDataRecord r) => new()
    {
        IdRecurso = Texto(r, "id_recurso"),
        Nombre = Texto(r, "nombre"),
        Descripcion = TextoNulo(r, "descripcion"),
        RutaArchivo = Texto(r, "ruta_archivo"),
        CodigoFormato = Texto(r, "formato"),
        TipoRecurso = Texto(r, "tipo_recurso"),
        TamanoBytes = Largo(r, "tamano_bytes"),
        DuracionSegundos = EnteroNulo(r, "duracion_segundos"),
        FechaRegistro = Fecha(r, "fecha_registro"),
        IdObra = Texto(r, "id_obra"),
        Obra = Texto(r, "obra"),
    };

    /// <summary>Consulta requerida: todos los recursos multimedia de una obra.</summary>
    public List<RecursoMultimedia> ListarPorObra(string idObra) => Consultar(
        @"SELECT id_recurso, nombre, descripcion, ruta_archivo, formato, tipo_recurso, tamano_bytes,
                 duracion_segundos, fecha_registro, id_obra, obra
          FROM vw_recurso_detalle
          WHERE id_obra = @obra
          ORDER BY FIELD(tipo_recurso, 'IMAGEN', 'VIDEO', 'AUDIO', 'MODELO_3D', 'AR'), nombre",
        Mapear, P("@obra", idObra));

    public void Insertar(RecursoMultimedia rm) => EjecutarComando(
        @"INSERT INTO recurso_multimedia (id_recurso, nombre, descripcion, ruta_archivo, codigo_formatoFK,
                                          tamano_bytes, duracion_segundos, id_obraFK)
          VALUES (@id, @nombre, @desc, @ruta, @formato, @tamano, @duracion, @obra)",
        P("@id", rm.IdRecurso), P("@nombre", rm.Nombre), P("@desc", rm.Descripcion), P("@ruta", rm.RutaArchivo),
        P("@formato", rm.CodigoFormato), P("@tamano", rm.TamanoBytes), P("@duracion", rm.DuracionSegundos), P("@obra", rm.IdObra));

    /// <summary>Solo se editan los metadatos; para cambiar el archivo se elimina y se registra de nuevo.</summary>
    public void Actualizar(RecursoMultimedia rm) => EjecutarComando(
        "UPDATE recurso_multimedia SET nombre = @nombre, descripcion = @desc, duracion_segundos = @duracion WHERE id_recurso = @id",
        P("@nombre", rm.Nombre), P("@desc", rm.Descripcion), P("@duracion", rm.DuracionSegundos), P("@id", rm.IdRecurso));

    public void Eliminar(string idRecurso) =>
        EjecutarComando("DELETE FROM recurso_multimedia WHERE id_recurso = @id", P("@id", idRecurso));

    // ---------- Formatos permitidos (tabla formato_multimedia) ----------

    public List<FormatoMultimedia> ListarFormatos() => Consultar(
        "SELECT codigo_formato, tipo_recurso, mime_type, descripcion FROM formato_multimedia ORDER BY tipo_recurso, codigo_formato",
        r => new FormatoMultimedia
        {
            CodigoFormato = Texto(r, "codigo_formato"),
            TipoRecurso = Texto(r, "tipo_recurso"),
            MimeType = Texto(r, "mime_type"),
            Descripcion = TextoNulo(r, "descripcion"),
        });
}
