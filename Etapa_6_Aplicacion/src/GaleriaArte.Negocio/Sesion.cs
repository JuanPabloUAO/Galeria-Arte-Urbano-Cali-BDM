using GaleriaArte.Entidades;

namespace GaleriaArte.Negocio;

/// <summary>
/// Usuario que inició sesión. La interfaz consulta <see cref="EsAdministrador"/> para
/// habilitar o deshabilitar botones, y los servicios lo verifican de nuevo antes de
/// modificar datos (así un Visitante no puede escribir aunque se habilite un botón por error).
/// </summary>
public static class Sesion
{
    public static Usuario? UsuarioActual { get; private set; }

    public static bool EsAdministrador => UsuarioActual?.EsAdministrador == true;

    internal static void Iniciar(Usuario usuario) => UsuarioActual = usuario;

    public static void Cerrar() => UsuarioActual = null;

    /// <summary>Lanza una excepción si el usuario actual no es Administrador.</summary>
    internal static void ExigirAdministrador()
    {
        if (!EsAdministrador)
            throw new ValidacionException("Esta operación solo está permitida para usuarios Administradores.");
    }
}
