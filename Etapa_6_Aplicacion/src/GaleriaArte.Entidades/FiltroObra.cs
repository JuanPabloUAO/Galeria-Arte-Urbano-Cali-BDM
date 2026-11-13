namespace GaleriaArte.Entidades;

/// <summary>Filtros opcionales para buscar obras (null = no filtrar por ese campo).</summary>
public class FiltroObra
{
    public string? NombreEmpiezaPor { get; set; }
    public string? IdArtista { get; set; }
    public string? IdCategoria { get; set; }
    public int? IdBarrio { get; set; }
    public int? Comuna { get; set; }
}
