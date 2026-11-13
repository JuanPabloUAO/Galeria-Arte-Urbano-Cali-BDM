namespace GaleriaArte.Entidades;

/// <summary>Fila genérica para consultas de agregación (etiqueta + valor).</summary>
public class Estadistica
{
    public string Etiqueta { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string? Detalle { get; set; }
}
