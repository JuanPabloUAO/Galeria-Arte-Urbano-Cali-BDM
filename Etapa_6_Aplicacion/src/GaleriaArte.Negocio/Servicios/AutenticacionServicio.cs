using GaleriaArte.Datos.Repositorios;
using GaleriaArte.Entidades;

namespace GaleriaArte.Negocio.Servicios;

/// <summary>Inicio de sesión: valida correo + contraseña contra el hash BCrypt guardado.</summary>
public class AutenticacionServicio
{
    private readonly UsuarioRepositorio _repo = new();

    public Usuario IniciarSesion(string? correo, string? contrasena)
    {
        var correoValido = Validar.Correo(correo);
        if (string.IsNullOrEmpty(contrasena))
            throw new ValidacionException("Ingrese la contraseña.");

        var usuario = _repo.ObtenerPorCorreo(correoValido);

        // Mismo mensaje si el correo no existe o si la clave es incorrecta:
        // así no se le revela a un atacante qué correos están registrados.
        if (usuario is null || !UsuarioServicio.VerificarContrasena(contrasena, usuario.PasswordHash))
            throw new ValidacionException("Correo o contraseña incorrectos.");

        if (usuario.Estado != Constantes.Activo)
            throw new ValidacionException("El usuario está inactivo. Contacte al administrador.");

        Sesion.Iniciar(usuario);
        return usuario;
    }
}
