using System.Collections.Generic;
using System.Data;
using GaleriaArte.Entidades;

namespace GaleriaArte.Datos.Repositorios;

public class UsuarioRepositorio : RepositorioBase
{
    private const string Columnas = "id_usuario, nombre, apellido, correo, password_hash, tipo_usuario, fecha_registro, estado";

    private static Usuario Mapear(IDataRecord r) => new()
    {
        IdUsuario = Texto(r, "id_usuario"),
        Nombre = Texto(r, "nombre"),
        Apellido = Texto(r, "apellido"),
        Correo = Texto(r, "correo"),
        PasswordHash = Texto(r, "password_hash"),
        TipoUsuario = Texto(r, "tipo_usuario"),
        FechaRegistro = Fecha(r, "fecha_registro"),
        Estado = Texto(r, "estado"),
    };

    public Usuario? ObtenerPorCorreo(string correo) =>
        ConsultarUno("SELECT " + Columnas + " FROM usuario WHERE correo = @correo", Mapear, P("@correo", correo));

    public List<Usuario> Listar() =>
        Consultar("SELECT " + Columnas + " FROM usuario ORDER BY apellido, nombre", Mapear);

    public void Insertar(Usuario u) => EjecutarComando(
        @"INSERT INTO usuario (id_usuario, nombre, apellido, correo, password_hash, tipo_usuario, estado)
          VALUES (@id, @nombre, @apellido, @correo, @hash, @tipo, @estado)",
        P("@id", u.IdUsuario), P("@nombre", u.Nombre), P("@apellido", u.Apellido), P("@correo", u.Correo),
        P("@hash", u.PasswordHash), P("@tipo", u.TipoUsuario), P("@estado", u.Estado));

    /// <summary>Actualiza los datos personales y el rol. La contraseña se cambia aparte.</summary>
    public void Actualizar(Usuario u) => EjecutarComando(
        @"UPDATE usuario SET nombre = @nombre, apellido = @apellido, correo = @correo, tipo_usuario = @tipo, estado = @estado
          WHERE id_usuario = @id",
        P("@nombre", u.Nombre), P("@apellido", u.Apellido), P("@correo", u.Correo),
        P("@tipo", u.TipoUsuario), P("@estado", u.Estado), P("@id", u.IdUsuario));

    public void CambiarPassword(string idUsuario, string hash) => EjecutarComando(
        "UPDATE usuario SET password_hash = @hash WHERE id_usuario = @id", P("@hash", hash), P("@id", idUsuario));

    public int ContarAdministradoresActivos() => System.Convert.ToInt32(Escalar(
        "SELECT COUNT(*) FROM usuario WHERE tipo_usuario = 'ADMINISTRADOR' AND estado = 'ACTIVO'"));
}
