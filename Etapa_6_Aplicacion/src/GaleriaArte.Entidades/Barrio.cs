namespace GaleriaArte.Entidades;

/// <summary>Tabla barrio (creada en la 3FN para eliminar la dependencia barrio → comuna).</summary>
public class Barrio
{
    public int IdBarrio { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Comuna { get; set; }

    public override string ToString() => $"{Nombre} (comuna {Comuna})";
}
