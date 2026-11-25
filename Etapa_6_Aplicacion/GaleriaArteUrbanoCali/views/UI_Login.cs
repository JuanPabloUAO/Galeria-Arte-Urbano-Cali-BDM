using System;
using System.Drawing;
using System.Windows.Forms;
using GaleriaArteUrbanoCali.controllers;
using GaleriaArteUrbanoCali.models;

namespace GaleriaArteUrbanoCali.views
{
    public class UI_Login : Form
    {
        private readonly ControllerUsuario _ctrlUsuario = new ControllerUsuario();

        private TextBox txtCorreo = new TextBox();
        private TextBox txtContrasena = new TextBox();
        private Label lblEstadoConexion = new Label();

        private TextBox txtRegN1 = new TextBox();
        private TextBox txtRegN2 = new TextBox();
        private TextBox txtRegA1 = new TextBox();
        private TextBox txtRegA2 = new TextBox();
        private TextBox txtRegCorreo = new TextBox();
        private TextBox txtRegClave = new TextBox();
        private ComboBox cboRegTipo = new ComboBox();

        public UI_Login()
        {
            ConfigurarInterfaz();
            CargarDatosIniciales();
        }

        private void ConfigurarInterfaz()
        {
            Text = "Galería de Arte Urbano de Cali — Acceso y Autenticación (BDM UAO)";
            Size = new Size(760, 480);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9.5F);

            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 68,
                BackColor = Color.FromArgb(24, 58, 90)
            };

            Label lblTitulo = new Label
            {
                Text = "GALERÍA DE ARTE URBANO DE SANTIAGO DE CALI",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 12)
            };

            Label lblSub = new Label
            {
                Text = "Universidad Autónoma de Occidente (UAO) — Bases de Datos Multimedia",
                ForeColor = Color.FromArgb(200, 220, 240),
                Font = new Font("Segoe UI", 8.8F),
                AutoSize = true,
                Location = new Point(22, 40)
            };

            pnlHeader.Controls.Add(lblTitulo);
            pnlHeader.Controls.Add(lblSub);
            Controls.Add(pnlHeader);

            GroupBox grpLogin = new GroupBox
            {
                Text = "Iniciar Sesión (Validación con Contraseña Cifrada)",
                Location = new Point(20, 82),
                Size = new Size(345, 315),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };

            Label l1 = new Label { Text = "Correo Electrónico:", Location = new Point(18, 35), AutoSize = true, Font = new Font("Segoe UI", 9F) };
            txtCorreo = new TextBox { Location = new Point(18, 58), Width = 305, Text = "juan.campo@uao.edu.co", Font = new Font("Segoe UI", 9.5F) };

            Label l2 = new Label { Text = "Contraseña:", Location = new Point(18, 95), AutoSize = true, Font = new Font("Segoe UI", 9F) };
            txtContrasena = new TextBox { Location = new Point(18, 118), Width = 305, Text = "Cali2026*", UseSystemPasswordChar = true, Font = new Font("Segoe UI", 9.5F) };

            Button btnIngresar = new Button
            {
                Text = "Iniciar Sesión",
                Location = new Point(18, 160),
                Size = new Size(305, 38),
                BackColor = Color.FromArgb(24, 90, 157),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnIngresar.Click += BtnIngresar_Click;

            Label lblDemo = new Label
            {
                Text = "Acceso rápido para pruebas de sustentación:",
                Location = new Point(18, 212),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
                ForeColor = Color.DimGray
            };

            Button btnDemoAdmin = new Button
            {
                Text = "Perfil Administrador (CRUD)",
                Location = new Point(18, 236),
                Size = new Size(148, 34),
                BackColor = Color.FromArgb(215, 232, 248),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.2F, FontStyle.Bold)
            };
            btnDemoAdmin.Click += (s, e) =>
            {
                txtCorreo.Text = "juan.campo@uao.edu.co";
                txtContrasena.Text = "Cali2026*";
            };

            Button btnDemoVisitante = new Button
            {
                Text = "Perfil Visitante (Consulta)",
                Location = new Point(175, 236),
                Size = new Size(148, 34),
                BackColor = Color.FromArgb(226, 240, 217),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.2F, FontStyle.Bold)
            };
            btnDemoVisitante.Click += (s, e) =>
            {
                txtCorreo.Text = "david.castro@talentum.edu.co";
                txtContrasena.Text = "Cali2026*";
            };

            grpLogin.Controls.AddRange(new Control[] { l1, txtCorreo, l2, txtContrasena, btnIngresar, lblDemo, btnDemoAdmin, btnDemoVisitante });
            Controls.Add(grpLogin);

            GroupBox grpRegistro = new GroupBox
            {
                Text = "Registrar Nuevo Usuario (Nombres Atómicos 1FN + Rol 3FN)",
                Location = new Point(380, 82),
                Size = new Size(345, 315),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };

            grpRegistro.Controls.Add(new Label { Text = "1er Nombre*:", Location = new Point(16, 32), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtRegN1 = new TextBox { Location = new Point(16, 52), Width = 148, Font = new Font("Segoe UI", 9F) };

            grpRegistro.Controls.Add(new Label { Text = "2do Nombre:", Location = new Point(178, 32), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtRegN2 = new TextBox { Location = new Point(178, 52), Width = 148, Font = new Font("Segoe UI", 9F) };

            grpRegistro.Controls.Add(new Label { Text = "1er Apellido*:", Location = new Point(16, 84), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtRegA1 = new TextBox { Location = new Point(16, 104), Width = 148, Font = new Font("Segoe UI", 9F) };

            grpRegistro.Controls.Add(new Label { Text = "2do Apellido:", Location = new Point(178, 84), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtRegA2 = new TextBox { Location = new Point(178, 104), Width = 148, Font = new Font("Segoe UI", 9F) };

            grpRegistro.Controls.Add(new Label { Text = "Correo Electrónico*:", Location = new Point(16, 136), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtRegCorreo = new TextBox { Location = new Point(16, 156), Width = 310, Font = new Font("Segoe UI", 9F) };

            grpRegistro.Controls.Add(new Label { Text = "Contraseña*:", Location = new Point(16, 188), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtRegClave = new TextBox { Location = new Point(16, 208), Width = 148, UseSystemPasswordChar = true, Font = new Font("Segoe UI", 9F) };

            grpRegistro.Controls.Add(new Label { Text = "Tipo Usuario (3FN)*:", Location = new Point(178, 188), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            cboRegTipo = new ComboBox { Location = new Point(178, 208), Width = 148, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F) };

            Button btnRegistrarUsuario = new Button
            {
                Text = "Registrar Usuario con Hash SHA-256",
                Location = new Point(16, 250),
                Size = new Size(310, 36),
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnRegistrarUsuario.Click += BtnRegistrarUsuario_Click;

            grpRegistro.Controls.AddRange(new Control[] { txtRegN1, txtRegN2, txtRegA1, txtRegA2, txtRegCorreo, txtRegClave, cboRegTipo, btnRegistrarUsuario });
            Controls.Add(grpRegistro);

            lblEstadoConexion = new Label
            {
                Text = "Verificando conexión a MySQL 8...",
                Location = new Point(22, 408),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            Controls.Add(lblEstadoConexion);
        }

        private void CargarDatosIniciales()
        {
            if (_ctrlUsuario.ProbarConexionBD(out string msg))
            {
                lblEstadoConexion.Text = "Estado BD: " + msg;
                lblEstadoConexion.ForeColor = Color.DarkGreen;

                cboRegTipo.DataSource = _ctrlUsuario.ListarTiposUsuario();
                cboRegTipo.DisplayMember = "NombreTipoUsuario";
                cboRegTipo.ValueMember = "IdTipoUsuario";
            }
            else
            {
                lblEstadoConexion.Text = "Error BD: " + msg;
                lblEstadoConexion.ForeColor = Color.DarkRed;
            }
        }

        private void BtnIngresar_Click(object? sender, EventArgs e)
        {
            Usuario? u = _ctrlUsuario.IniciarSesion(txtCorreo.Text, txtContrasena.Text, out string mensaje);
            if (u == null)
            {
                MessageBox.Show(mensaje, "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnRegistrarUsuario_Click(object? sender, EventArgs e)
        {
            var nuevo = new Usuario
            {
                Nombre1Usuario = txtRegN1.Text,
                Nombre2Usuario = txtRegN2.Text,
                Apellido1Usuario = txtRegA1.Text,
                Apellido2Usuario = txtRegA2.Text,
                CorreoUsuario = txtRegCorreo.Text,
                EstadoUsuario = true,
                IdTipoUsuarioFK = cboRegTipo.SelectedValue?.ToString() ?? string.Empty
            };

            if (_ctrlUsuario.RegistrarUsuario(nuevo, txtRegClave.Text, out string msg))
            {
                MessageBox.Show(
                    "Usuario registrado correctamente con contraseña cifrada en SHA-256.\nAhora puede iniciar sesión.",
                    "Registro Exitoso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                txtCorreo.Text = nuevo.CorreoUsuario;
                txtContrasena.Text = txtRegClave.Text;
                txtRegN1.Clear();
                txtRegN2.Clear();
                txtRegA1.Clear();
                txtRegA2.Clear();
                txtRegCorreo.Clear();
                txtRegClave.Clear();
            }
            else
            {
                MessageBox.Show("No se pudo registrar el usuario:\n" + msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
