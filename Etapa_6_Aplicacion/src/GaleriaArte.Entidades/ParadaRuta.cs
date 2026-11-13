namespace GaleriaArte.Entidades;

/// <summary>
/// Tabla asociativa ruta_obra, enriquecida con datos de la obra y su ubicación
/// (vista vw_ruta_detalle) para mostrar el recorrido.
/// </summary>
public class ParadaRuta
{
    public string IdRutaObra { get; set; } = string.Empty;
    public string IdRuta { get; set; } = string.Empty;
    public string IdObra { get; set; } = string.Empty;
    public int OrdenVisita { get; set; }
    public int TiempoEstimadoMin { get; set; }
    public string? Recomendacion { get; set; }

    // Traídos con JOIN
    public string Ruta { get; set; } = string.Empty;
    public string Obra { get; set; } = string.Empty;
    public string Artista { get; set; } = string.Empty;
    public string Lugar { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Barrio { get; set; } = string.Empty;
    public int Comuna { get; set; }
}
