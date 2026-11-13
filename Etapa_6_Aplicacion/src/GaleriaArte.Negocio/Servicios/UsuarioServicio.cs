using System.Collections.Generic;
using System.Linq;
using GaleriaArte.Datos.Repositorios;
using GaleriaArte.Entidades;

namespace GaleriaArte.Negocio.Servicios;

public class UsuarioServicio
{
    private readonly UsuarioRepositorio _repo = new();

    // ---------- Cifrado (regla 7: contraseñas nunca en texto plano) ----------

    /// <summary>
    /// Genera el hash BCrypt. BCrypt agrega una "sal" aleatoria (dos usuarios con la misma clave
    /// tienen hashes distintos) y es lento a propósito (factor 11) para dificultar ataques de fuerza bruta.
    /// </summary>
    public static string CifrarContrasena(string contrasena) => BCrypt.Net.BCrypt.HashPassword(contrasena, workFactor: 11);

    public static bool VerificarContrasena(string contrasena, string hash)
    {
        try { return BCrypt.Net.BCrypt.Verify(contrasena, hash); }
        catch (BCrypt.Net.SaltParseException) { return false; } // hash corrupto o no BCrypt
    }

    private static void ValidarFortaleza(string? contrasena)
    {
        if (string.IsNullOrEmpty(contrasena) || contrasena.Length < 8
            || !contrasena.Any(char.IsLetter) || !contrasena.Any(char.IsDigit))
            throw new ValidacionException("La contraseña debe tener mínimo 8 caracteres e incluir letras y números.");
    }

    // ---------- Operaciones ----------

    public List<Usuario> Listar()
    {
        Sesion.ExigirAdministrador();
        return _repo.Listar();
    }

    /// <summary>
    /// Crea o actualiza un usuario. En edición, si <paramref name="nuevaContrasena"/> viene vacía, se conserva la actual.
    /// </summary>
    public void Guardar(Usuario u, string? nuevaContrasena)
    {
        Sesion.ExigirAdministrador();
        u.Nombre = Validar.Requerido(u.Nombre, "Nombre", 50);
        u.Apellido = Validar.Requerido(u.Apellido, "Apellido", 50);
        u.Correo = Validar.Correo(u.Correo);
        Validar.EnLista(u.TipoUsuario, Constantes.TiposUsuario, "Tipo de usuario");
        Validar.EnLista(u.Estado, Constantes.EstadosBasicos, "Estado");

        bool esNuevo = string.IsNullOrEmpty(u.IdUsuario);
        if (esNuevo)
        {
            ValidarFortaleza(nuevaContrasena);
            u.IdUsuario = Validar.NuevoId();
            u.PasswordHash = CifrarContrasena(nuevaContrasena!);
            _repo.Insertar(u);
            return;
        }

        // No permitir quedarse sin administradores activos
        if (u.IdUsuario == Sesion.UsuarioActual?.IdUsuario && (u.TipoUsuario != Constantes.Administrador || u.Estado != Constantes.Activo))
            throw new ValidacionException("No puede quitarse a sí mismo el rol de Administrador ni desactivarse.");

        _repo.Actualizar(u);
        if (!string.IsNullOrEmpty(nuevaContrasena))
        {
            ValidarFortaleza(nuevaContrasena);
            _repo.CambiarPassword(u.IdUsuario, CifrarContrasena(nuevaContrasena));
        }
    }
}
