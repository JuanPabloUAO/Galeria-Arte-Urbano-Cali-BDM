namespace GaleriaArte.Entidades;

/// <summary>Tabla recurso_multimedia. RutaArchivo es RELATIVA a la carpeta RecursosMultimedia.</summary>
public class RecursoMultimedia
{
    public string IdRecurso { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string RutaArchivo { get; set; } = string.Empty;
    public string CodigoFormato { get; set; } = string.Empty;
    public long TamanoBytes { get; set; }
    public int? DuracionSegundos { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string IdObra { get; set; } = string.Empty;

    // Traídos con JOIN
    public string TipoRecurso { get; set; } = string.Empty;
    public string Obra { get; set; } = string.Empty;

    public string TamanoLegible => TamanoBytes >= 1048576
        ? $"{TamanoBytes / 1048576.0:0.0} MB"
        : $"{TamanoBytes / 1024.0:0.0} KB";
}
