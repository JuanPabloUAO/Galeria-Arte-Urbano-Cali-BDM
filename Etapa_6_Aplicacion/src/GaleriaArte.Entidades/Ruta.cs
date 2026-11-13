namespace GaleriaArte.Entidades;

/// <summary>Tabla ruta.</summary>
public class Ruta
{
    public string IdRuta { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int DuracionEstimadaMin { get; set; }
    public decimal DistanciaEstimadaKm { get; set; }
    public string Estado { get; set; } = Constantes.Activo;
    public int TotalObras { get; set; }

    public override string ToString() => Nombre;
}
