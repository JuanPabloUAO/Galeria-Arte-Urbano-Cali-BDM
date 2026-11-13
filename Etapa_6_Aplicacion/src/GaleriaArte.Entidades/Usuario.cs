namespace GaleriaArte.Entidades;

/// <summary>Tabla usuario. La contraseña solo existe como hash BCrypt.</summary>
public class Usuario
{
    public string IdUsuario { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string TipoUsuario { get; set; } = Constantes.Visitante;
    public DateTime FechaRegistro { get; set; }
    public string Estado { get; set; } = Constantes.Activo;

    public string NombreCompleto => $"{Nombre} {Apellido}";
    public bool EsAdministrador => TipoUsuario == Constantes.Administrador;
}
