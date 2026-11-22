using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace GaleriaArteUrbanoCali.models
{
    public class Usuario
    {
        public string IdUsuario { get; set; } = string.Empty;
        public string Nombre1Usuario { get; set; } = string.Empty;
        public string? Nombre2Usuario { get; set; }
        public string Apellido1Usuario { get; set; } = string.Empty;
        public string? Apellido2Usuario { get; set; }
        public string CorreoUsuario { get; set; } = string.Empty;
        public string ContrasenaHash { get; set; } = string.Empty;
        public DateTime FechaRegistroUsuario { get; set; } = DateTime.Today;
        public bool EstadoUsuario { get; set; } = true;
        public string IdTipoUsuarioFK { get; set; } = string.Empty;
        public string NombreTipoUsuario { get; set; } = string.Empty;

        public string NombreCompleto =>
            $"{Nombre1Usuario} {Nombre2Usuario}".Trim() + " " + $"{Apellido1Usuario} {Apellido2Usuario}".Trim();

        public bool EsAdministrador =>
            string.Equals(NombreTipoUsuario, "Administrador", StringComparison.OrdinalIgnoreCase);

        public Usuario? AutenticarPorCorreo(string correo, string contrasenaPlano, out string mensaje)
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT u.idUsuario, u.nombre1Usuario, u.nombre2Usuario, u.apellido1Usuario, u.apellido2Usuario,
                       u.correoUsuario, u.contrasenaHash, u.fechaRegistroUsuario, u.estadoUsuario,
                       u.idTipoUsuarioFK, t.nombreTipoUsuario
                FROM usuario u
                INNER JOIN tipo_usuario t ON u.idTipoUsuarioFK = t.idTipoUsuario
                WHERE u.correoUsuario = @correo
                LIMIT 1;";

            try
            {
                using MySqlCommand cmd = new MySqlCommand(sql, conexion.Abrir());
                cmd.Parameters.AddWithValue("@correo", correo.Trim());
                using MySqlDataReader r = cmd.ExecuteReader();

                if (!r.Read())
                {
                    mensaje = "No existe ningún usuario registrado con ese correo electrónico.";
                    return null;
                }

                bool activo = r.GetBoolean("estadoUsuario");
                if (!activo)
                {
                    mensaje = "La cuenta de usuario se encuentra inactiva.";
                    return null;
                }

                string hash = r.GetString("contrasenaHash");
                if (!SeguridadHash.VerificarContrasena(contrasenaPlano, hash))
                {
                    mensaje = "Contraseña incorrecta.";
                    return null;
                }

                var usr = new Usuario
                {
                    IdUsuario = r["idUsuario"].ToString()!,
                    Nombre1Usuario = r.GetString("nombre1Usuario"),
                    Nombre2Usuario = r.IsDBNull(r.GetOrdinal("nombre2Usuario")) ? null : r.GetString("nombre2Usuario"),
                    Apellido1Usuario = r.GetString("apellido1Usuario"),
                    Apellido2Usuario = r.IsDBNull(r.GetOrdinal("apellido2Usuario")) ? null : r.GetString("apellido2Usuario"),
                    CorreoUsuario = r.GetString("correoUsuario"),
                    ContrasenaHash = hash,
                    FechaRegistroUsuario = r.GetDateTime("fechaRegistroUsuario"),
                    EstadoUsuario = activo,
                    IdTipoUsuarioFK = r["idTipoUsuarioFK"].ToString()!,
                    NombreTipoUsuario = r.GetString("nombreTipoUsuario")
                };

                mensaje = $"Bienvenido(a) {usr.NombreCompleto} ({usr.NombreTipoUsuario}).";
                return usr;
            }
            catch (Exception ex)
            {
                mensaje = "Error al autenticar usuario: " + ex.Message;
                return null;
            }
            finally
            {
                conexion.Cerrar();
            }
        }

        public bool InsertarUsuario(Usuario u, string contrasenaPlano, out string mensajeError)
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                INSERT INTO usuario
                (idUsuario, nombre1Usuario, nombre2Usuario, apellido1Usuario, apellido2Usuario,
                 correoUsuario, contrasenaHash, fechaRegistroUsuario, estadoUsuario, idTipoUsuarioFK)
                VALUES
                (@id, @n1, @n2, @a1, @a2, @correo, @hash, @fecha, @estado, @idTipo);";

            var p = new Dictionary<string, object?>
            {
                { "@id", string.IsNullOrWhiteSpace(u.IdUsuario) ? Guid.NewGuid().ToString() : u.IdUsuario.Trim() },
                { "@n1", u.Nombre1Usuario.Trim() },
                { "@n2", string.IsNullOrWhiteSpace(u.Nombre2Usuario) ? null : u.Nombre2Usuario.Trim() },
                { "@a1", u.Apellido1Usuario.Trim() },
                { "@a2", string.IsNullOrWhiteSpace(u.Apellido2Usuario) ? null : u.Apellido2Usuario.Trim() },
                { "@correo", u.CorreoUsuario.Trim() },
                { "@hash", SeguridadHash.GenerarHashSha256(contrasenaPlano) },
                { "@fecha", DateTime.Today.ToString("yyyy-MM-dd") },
                { "@estado", u.EstadoUsuario },
                { "@idTipo", u.IdTipoUsuarioFK.Trim() }
            };

            return conexion.ExecuteQuery(sql, p, out mensajeError);
        }

        public DataTable ConsultarUsuarios()
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT u.idUsuario AS 'ID',
                       CONCAT_WS(' ', u.nombre1Usuario, u.nombre2Usuario, u.apellido1Usuario, u.apellido2Usuario) AS 'Nombre Completo',
                       u.correoUsuario AS 'Correo Electrónico',
                       t.nombreTipoUsuario AS 'Rol',
                       DATE_FORMAT(u.fechaRegistroUsuario, '%Y-%m-%d') AS 'Fecha Registro',
                       CASE WHEN u.estadoUsuario = 1 THEN 'Activo' ELSE 'Inactivo' END AS 'Estado'
                FROM usuario u
                INNER JOIN tipo_usuario t ON u.idTipoUsuarioFK = t.idTipoUsuario
                ORDER BY t.nombreTipoUsuario, u.apellido1Usuario;";
            return conexion.ExecuteDataTable(sql);
        }
    }
}
