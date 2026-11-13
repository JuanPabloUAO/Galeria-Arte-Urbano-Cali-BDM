namespace GaleriaArte.Entidades;

/// <summary>Tabla obra. Los nombres (Artista, Categoria...) se llenan con JOIN para la grilla.</summary>
public class Obra
{
    public string IdObra { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime? FechaCreacion { get; set; }
    public string EstadoConservacion { get; set; } = "BUENO";
    public DateTime FechaRegistro { get; set; }
    public string IdArtista { get; set; } = string.Empty;
    public string IdCategoria { get; set; } = string.Empty;
    public string IdUbicacion { get; set; } = string.Empty;
    public string IdUsuario { get; set; } = string.Empty;

    // Datos de solo lectura traídos con JOIN (vista vw_obra_detalle)
    public string Artista { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string Lugar { get; set; } = string.Empty;
    public string Barrio { get; set; } = string.Empty;
    public int Comuna { get; set; }
    public string RegistradoPor { get; set; } = string.Empty;
    public int TotalRecursos { get; set; }

    public override string ToString() => Nombre;
}
