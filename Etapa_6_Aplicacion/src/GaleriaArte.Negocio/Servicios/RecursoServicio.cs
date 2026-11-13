using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GaleriaArte.Datos.Repositorios;
using GaleriaArte.Entidades;

namespace GaleriaArte.Negocio.Servicios;

public class RecursoServicio
{
    private readonly RecursoRepositorio _repo = new();
    private List<FormatoMultimedia>? _formatos;

    /// <summary>Formatos permitidos, leídos de la tabla formato_multimedia (se cargan una sola vez).</summary>
    public List<FormatoMultimedia> Formatos => _formatos ??= _repo.ListarFormatos();

    public List<RecursoMultimedia> ListarPorObra(string idObra) => _repo.ListarPorObra(idObra);

    /// <summary>
    /// Registra un archivo (o una carpeta AR) para una obra:
    /// 1) valida, 2) copia a RecursosMultimedia/&lt;Tipo&gt;/, 3) guarda la ruta relativa en MySQL.
    /// Si el INSERT falla, se borra la copia para no dejar archivos huérfanos.
    /// </summary>
    public RecursoMultimedia Registrar(string idObra, string rutaOrigen, string? nombre, string? descripcion, int? duracionSegundos, bool esCarpetaAR)
    {
        Sesion.ExigirAdministrador();
        Validar.IdSeleccionado(idObra, "una obra");
        if (string.IsNullOrWhiteSpace(rutaOrigen))
            throw new ValidacionException("Seleccione el archivo o la carpeta del recurso.");

        string codigoFormato;
        long tamano;
        if (esCarpetaAR)
        {
            if (!Directory.Exists(rutaOrigen)) throw new ValidacionException("La carpeta seleccionada no existe.");
            codigoFormato = "AR";
            tamano = GestorArchivos.TamanoCarpeta(rutaOrigen);
        }
        else
        {
            if (!File.Exists(rutaOrigen)) throw new ValidacionException("El archivo seleccionado no existe.");
            codigoFormato = GestorArchivos.FormatoDesdeExtension(rutaOrigen)
                ?? throw new ValidacionException("Formato no permitido. Use JPG, PNG, MP4, MP3 o GLB.");
            tamano = new FileInfo(rutaOrigen).Length;
        }

        var formato = Formatos.FirstOrDefault(f => f.CodigoFormato == codigoFormato)
            ?? throw new ValidacionException($"El formato {codigoFormato} no está habilitado en la base de datos.");
        if (tamano <= 0) throw new ValidacionException("El archivo está vacío.");

        bool admiteDuracion = formato.TipoRecurso == Constantes.TipoVideo || formato.TipoRecurso == Constantes.TipoAudio;
        if (!admiteDuracion) duracionSegundos = null;
        else if (duracionSegundos.HasValue && duracionSegundos <= 0)
            throw new ValidacionException("La duración debe ser mayor que cero.");

        var recurso = new RecursoMultimedia
        {
            IdRecurso = Validar.NuevoId(),
            IdObra = idObra,
            Nombre = Validar.Requerido(string.IsNullOrWhiteSpace(nombre) ? Path.GetFileNameWithoutExtension(rutaOrigen.TrimEnd('\\', '/')) : nombre, "Nombre", 100),
            Descripcion = Validar.Opcional(descripcion, "Descripción", 2000),
            CodigoFormato = codigoFormato,
            TipoRecurso = formato.TipoRecurso,
            TamanoBytes = tamano,
            DuracionSegundos = duracionSegundos,
        };

        recurso.RutaArchivo = esCarpetaAR
            ? GestorArchivos.CopiarCarpetaAR(rutaOrigen, idObra)
            : GestorArchivos.CopiarArchivo(rutaOrigen, formato.TipoRecurso, idObra);

        try
        {
            _repo.Insertar(recurso);
        }
        catch
        {
            GestorArchivos.Eliminar(recurso.RutaArchivo); // deshacer la copia
            throw;
        }
        return recurso;
    }

    public void ActualizarDatos(RecursoMultimedia r)
    {
        Sesion.ExigirAdministrador();
        r.Nombre = Validar.Requerido(r.Nombre, "Nombre", 100);
        r.Descripcion = Validar.Opcional(r.Descripcion, "Descripción", 2000);
        if (r.TipoRecurso != Constantes.TipoVideo && r.TipoRecurso != Constantes.TipoAudio) r.DuracionSegundos = null;
        _repo.Actualizar(r);
    }

    /// <summary>Borra el registro y luego el archivo físico.</summary>
    public void Eliminar(RecursoMultimedia r)
    {
        Sesion.ExigirAdministrador();
        _repo.Eliminar(r.IdRecurso);
        GestorArchivos.Eliminar(r.RutaArchivo);
    }

    /// <summary>Ruta completa para abrir el recurso. Lanza un error claro si el archivo ya no está en disco.</summary>
    public string ObtenerRutaParaAbrir(RecursoMultimedia r)
    {
        if (!GestorArchivos.Existe(r.RutaArchivo))
            throw new ValidacionException($"No se encontró el archivo '{r.RutaArchivo}' dentro de la carpeta RecursosMultimedia.\n" +
                                          "Verifique que la carpeta se copió completa con el proyecto.");
        return GestorArchivos.RutaAbsoluta(r.RutaArchivo);
    }
}
