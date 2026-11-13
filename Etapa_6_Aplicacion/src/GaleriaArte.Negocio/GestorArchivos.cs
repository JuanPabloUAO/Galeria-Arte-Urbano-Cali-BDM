using System;
using System.Collections.Generic;
using System.IO;
using GaleriaArte.Entidades;

namespace GaleriaArte.Negocio;

/// <summary>
/// Maneja la carpeta RecursosMultimedia. Los archivos NO se guardan en MySQL:
/// se copian a esta carpeta y en la base solo se registra la ruta RELATIVA
/// (ej. "Imagenes/obra_50000000_20261109153000.jpg"), para que el proyecto funcione en otro equipo.
/// </summary>
public static class GestorArchivos
{
    public const string NombreCarpetaRaiz = "RecursosMultimedia";

    private static readonly Dictionary<string, string> CarpetaPorTipo = new()
    {
        [Constantes.TipoImagen] = "Imagenes",
        [Constantes.TipoVideo] = "Videos",
        [Constantes.TipoAudio] = "Audios",
        [Constantes.TipoModelo3D] = "Modelos3D",
        [Constantes.TipoAR] = "AR",
    };

    /// <summary>Extensión del archivo → código de formato (PK de formato_multimedia).</summary>
    private static readonly Dictionary<string, string> FormatoPorExtension = new(StringComparer.OrdinalIgnoreCase)
    {
        [".jpg"] = "JPG",
        [".jpeg"] = "JPG",
        [".png"] = "PNG",
        [".mp4"] = "MP4",
        [".mp3"] = "MP3",
        [".glb"] = "GLB",
    };

    public static string CarpetaRaiz { get; private set; } = string.Empty;

    /// <summary>
    /// Define la carpeta raíz. Si la configuración viene vacía, la busca subiendo desde la carpeta
    /// del ejecutable (así funciona igual desde Visual Studio, bin\Debug\..., o desde una copia publicada).
    /// Si no existe, la crea junto al ejecutable.
    /// </summary>
    public static void Configurar(string? rutaConfigurada)
    {
        string raiz;
        if (!string.IsNullOrWhiteSpace(rutaConfigurada))
        {
            raiz = Path.IsPathRooted(rutaConfigurada)
                ? rutaConfigurada
                : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, rutaConfigurada));
        }
        else
        {
            raiz = BuscarHaciaArriba(AppContext.BaseDirectory) ?? Path.Combine(AppContext.BaseDirectory, NombreCarpetaRaiz);
        }

        CarpetaRaiz = Path.GetFullPath(raiz);
        Directory.CreateDirectory(CarpetaRaiz);
        foreach (var sub in CarpetaPorTipo.Values)
            Directory.CreateDirectory(Path.Combine(CarpetaRaiz, sub));
    }

    private static string? BuscarHaciaArriba(string desde)
    {
        var dir = new DirectoryInfo(desde);
        while (dir != null)
        {
            var candidata = Path.Combine(dir.FullName, NombreCarpetaRaiz);
            if (Directory.Exists(candidata)) return candidata;
            dir = dir.Parent;
        }
        return null;
    }

    public static string? FormatoDesdeExtension(string rutaArchivo) =>
        FormatoPorExtension.TryGetValue(Path.GetExtension(rutaArchivo), out var formato) ? formato : null;

    public static string FiltroDialogo =>
        "Archivos multimedia (*.jpg;*.jpeg;*.png;*.mp4;*.mp3;*.glb)|*.jpg;*.jpeg;*.png;*.mp4;*.mp3;*.glb";

    /// <summary>Convierte la ruta relativa guardada en MySQL en la ruta completa del equipo actual.</summary>
    public static string RutaAbsoluta(string rutaRelativa)
    {
        var relativa = rutaRelativa.Replace('/', Path.DirectorySeparatorChar).TrimEnd(Path.DirectorySeparatorChar);
        var completa = Path.GetFullPath(Path.Combine(CarpetaRaiz, relativa));
        // Seguridad: impedir que una ruta con ".." salga de la carpeta de recursos
        if (!completa.StartsWith(CarpetaRaiz, StringComparison.OrdinalIgnoreCase))
            throw new ValidacionException("La ruta del recurso no es válida.");
        return completa;
    }

    public static bool Existe(string rutaRelativa)
    {
        var ruta = RutaAbsoluta(rutaRelativa);
        return File.Exists(ruta) || Directory.Exists(ruta);
    }

    /// <summary>
    /// Copia un archivo del computador a la subcarpeta de su tipo con un nombre único.
    /// Devuelve la ruta RELATIVA que se guarda en la base de datos (siempre con '/').
    /// </summary>
    public static string CopiarArchivo(string rutaOrigen, string tipoRecurso, string idObra)
    {
        var carpeta = CarpetaPorTipo[tipoRecurso];
        var nombre = $"obra_{idObra.Substring(0, 8)}_{DateTime.Now:yyyyMMddHHmmssfff}{Path.GetExtension(rutaOrigen).ToLowerInvariant()}";
        File.Copy(rutaOrigen, Path.Combine(CarpetaRaiz, carpeta, nombre), overwrite: false);
        return carpeta + "/" + nombre;
    }

    /// <summary>Copia una carpeta de contenido AR completa. Devuelve "AR/nombre/".</summary>
    public static string CopiarCarpetaAR(string carpetaOrigen, string idObra)
    {
        var nombre = $"obra_{idObra.Substring(0, 8)}_{DateTime.Now:yyyyMMddHHmmssfff}";
        var destino = Path.Combine(CarpetaRaiz, CarpetaPorTipo[Constantes.TipoAR], nombre);
        CopiarDirectorio(carpetaOrigen, destino);
        return CarpetaPorTipo[Constantes.TipoAR] + "/" + nombre + "/";
    }

    private static void CopiarDirectorio(string origen, string destino)
    {
        Directory.CreateDirectory(destino);
        foreach (var archivo in Directory.GetFiles(origen))
            File.Copy(archivo, Path.Combine(destino, Path.GetFileName(archivo)));
        foreach (var sub in Directory.GetDirectories(origen))
            CopiarDirectorio(sub, Path.Combine(destino, Path.GetFileName(sub)));
    }

    public static long TamanoCarpeta(string carpeta)
    {
        long total = 0;
        foreach (var archivo in Directory.GetFiles(carpeta, "*", SearchOption.AllDirectories))
            total += new FileInfo(archivo).Length;
        return total;
    }

    /// <summary>Borra el archivo o carpeta del disco. No falla si ya no existe.</summary>
    public static void Eliminar(string rutaRelativa)
    {
        var ruta = RutaAbsoluta(rutaRelativa);
        if (File.Exists(ruta)) File.Delete(ruta);
        else if (Directory.Exists(ruta)) Directory.Delete(ruta, recursive: true);
    }
}
