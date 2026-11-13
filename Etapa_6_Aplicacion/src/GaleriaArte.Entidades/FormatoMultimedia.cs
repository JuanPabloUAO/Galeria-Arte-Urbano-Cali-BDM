namespace GaleriaArte.Entidades;

/// <summary>Tabla formato_multimedia (creada en la 3FN: formato → tipo_recurso).</summary>
public class FormatoMultimedia
{
    public string CodigoFormato { get; set; } = string.Empty;
    public string TipoRecurso { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    public override string ToString() => $"{CodigoFormato} ({TipoRecurso})";
}
