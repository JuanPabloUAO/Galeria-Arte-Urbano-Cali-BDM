namespace GaleriaArte.Entidades;

/// <summary>Tabla artista.</summary>
public class Artista
{
    public string IdArtista { get; set; } = string.Empty;
    public string NombreArtistico { get; set; } = string.Empty;
    public string? NombreReal { get; set; }
    public string? Biografia { get; set; }
    public string PaisOrigen { get; set; } = "Colombia";
    public string? RedSocial { get; set; }
    public string Estado { get; set; } = Constantes.Activo;

    /// <summary>Calculado en la consulta (no es columna): número de obras del artista.</summary>
    public int TotalObras { get; set; }

    public override string ToString() => Estado == Constantes.Activo ? NombreArtistico : $"{NombreArtistico} (inactivo)";
}
