using System;
using System.Collections.Generic;
using System.Data;
using GaleriaArteUrbanoCali.models;

namespace GaleriaArteUrbanoCali.controllers
{
    public class ControllerUsuario
    {
        private readonly Usuario _modeloUsuario = new Usuario();
        private readonly TipoUsuario _modeloTipoUsuario = new TipoUsuario();

        public static Usuario? SesionActual { get; set; }

        public Usuario? IniciarSesion(string correo, string contrasena, out string mensaje)
        {
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contrasena))
            {
                mensaje = "Debe ingresar correo electrónico y contraseña.";
                return null;
            }

            Usuario? usuario = _modeloUsuario.AutenticarPorCorreo(correo, contrasena, out mensaje);
            if (usuario != null)
            {
                SesionActual = usuario;
            }
            return usuario;
        }

        public List<TipoUsuario> ListarTiposUsuario()
        {
            return _modeloTipoUsuario.GetTiposUsuario();
        }

        public bool RegistrarUsuario(Usuario u, string contrasenaPlano, out string mensaje)
        {
            if (string.IsNullOrWhiteSpace(u.Nombre1Usuario) ||
                string.IsNullOrWhiteSpace(u.Apellido1Usuario) ||
                string.IsNullOrWhiteSpace(u.CorreoUsuario) ||
                string.IsNullOrWhiteSpace(contrasenaPlano) ||
                string.IsNullOrWhiteSpace(u.IdTipoUsuarioFK))
            {
                mensaje = "Complete los campos obligatorios: Primer Nombre, Primer Apellido, Correo, Contraseña y Tipo de Usuario.";
                return false;
            }

            if (!u.CorreoUsuario.Contains('@') || !u.CorreoUsuario.Contains('.'))
            {
                mensaje = "El formato del correo electrónico no es válido.";
                return false;
            }

            return _modeloUsuario.InsertarUsuario(u, contrasenaPlano, out mensaje);
        }

        public DataTable ConsultarUsuarios()
        {
            return _modeloUsuario.ConsultarUsuarios();
        }

        public bool ProbarConexionBD(out string mensaje)
        {
            using var conexion = new ConexionMysql();
            return conexion.ProbarConexion(out mensaje);
        }

        public string ObtenerDirectorioRaizMultimedia()
        {
            return ConexionMysql.ObtenerDirectorioRaizMultimedia();
        }
    }
}
