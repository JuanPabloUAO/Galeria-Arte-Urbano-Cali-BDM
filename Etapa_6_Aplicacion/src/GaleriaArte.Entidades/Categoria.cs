namespace GaleriaArte.Entidades;

/// <summary>Tabla categoria.</summary>
public class Categoria
{
    public string IdCategoria { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Estado { get; set; } = Constantes.Activo;
    public int TotalObras { get; set; }

    public override string ToString() => Estado == Constantes.Activo ? Nombre : $"{Nombre} (inactiva)";
}
