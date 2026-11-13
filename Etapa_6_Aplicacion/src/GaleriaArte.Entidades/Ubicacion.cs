namespace GaleriaArte.Entidades;

/// <summary>Tabla ubicacion. NombreBarrio y Comuna se llenan con JOIN para mostrarlos.</summary>
public class Ubicacion
{
    public string IdUbicacion { get; set; } = string.Empty;
    public string NombreLugar { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public int IdBarrio { get; set; }
    public decimal Latitud { get; set; }
    public decimal Longitud { get; set; }

    // Datos de solo lectura traídos con JOIN
    public string NombreBarrio { get; set; } = string.Empty;
    public int Comuna { get; set; }

    public override string ToString() => $"{NombreLugar} - {NombreBarrio}";
}
