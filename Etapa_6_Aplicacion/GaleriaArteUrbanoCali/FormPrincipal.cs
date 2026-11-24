using System;
using System.Drawing;
using System.Windows.Forms;
using GaleriaArteUrbanoCali.controllers;
using GaleriaArteUrbanoCali.models;
using GaleriaArteUrbanoCali.views;

namespace GaleriaArteUrbanoCali
{
    public class FormPrincipal : Form
    {
        private readonly ControllerObra _ctrlObra = new ControllerObra();
        private readonly ControllerUsuario _ctrlUsuario = new ControllerUsuario();
        private Label lblUsuarioActivo = new Label();
        private Label lblEstadoMysql = new Label();
        private DataGridView dgvResumenPrincipal = new DataGridView();

        public FormPrincipal()
        {
            ConfigurarInterfaz();
            ActualizarEncabezadoYTabla();
        }

        private void ConfigurarInterfaz()
        {
            bool esAdmin = ControllerUsuario.SesionActual?.EsAdministrador ?? true;
            string rolTexto = esAdmin ? "ADMINISTRADOR (Gestión Total CRUD)" : "VISITANTE (Modo Solo Consulta)";

            Text = $"Galería de Arte Urbano de Cali — Panel Principal [{rolTexto}]";
            Size = new Size(1180, 720);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9.5F);

            Panel pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 76,
                BackColor = Color.FromArgb(24, 58, 90)
            };

            Label lblTitulo = new Label
            {
                Text = "SISTEMA DE INFORMACIÓN — GALERÍA DE ARTE URBANO DE CALI",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13.5F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 12)
            };

            lblUsuarioActivo = new Label
            {
                Text = "Usuario activo:",
                ForeColor = Color.FromArgb(210, 230, 250),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(22, 42)
            };

            Button btnCambiarSesion = new Button
            {
                Text = "Cambiar Usuario / Rol",
                Location = new Point(985, 20),
                Size = new Size(160, 36),
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnCambiarSesion.Click += BtnCambiarSesion_Click;

            pnlTop.Controls.AddRange(new Control[] { lblTitulo, lblUsuarioActivo, btnCambiarSesion });
            Controls.Add(pnlTop);

            GroupBox grpModulos = new GroupBox
            {
                Text = "Módulos de Gestión del Sistema",
                Location = new Point(18, 90),
                Size = new Size(1125, 105),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };

            Button btnArtistas = CrearBotonModulo("1. Artistas", 16, 32, Color.FromArgb(24, 90, 157));
            btnArtistas.Click += (s, e) =>
            {
                using var f = new UI_GestionArtistas(!(ControllerUsuario.SesionActual?.EsAdministrador ?? true));
                f.ShowDialog();
                ActualizarEncabezadoYTabla();
            };

            Button btnCategorias = CrearBotonModulo("2. Categorías", 198, 32, Color.FromArgb(24, 90, 157));
            btnCategorias.Click += (s, e) =>
            {
                using var f = new UI_GestionCategorias(!(ControllerUsuario.SesionActual?.EsAdministrador ?? true));
                f.ShowDialog();
                ActualizarEncabezadoYTabla();
            };

            Button btnUbicaciones = CrearBotonModulo("3. Ubicaciones", 380, 32, Color.FromArgb(24, 90, 157));
            btnUbicaciones.Click += (s, e) =>
            {
                using var f = new UI_GestionUbicaciones(!(ControllerUsuario.SesionActual?.EsAdministrador ?? true));
                f.ShowDialog();
                ActualizarEncabezadoYTabla();
            };

            Button btnObras = CrearBotonModulo("4. Obras de Arte Urbano", 562, 32, Color.FromArgb(24, 90, 157));
            btnObras.Click += (s, e) =>
            {
                using var f = new UI_GestionObras(!(ControllerUsuario.SesionActual?.EsAdministrador ?? true));
                f.ShowDialog();
                ActualizarEncabezadoYTabla();
            };

            Button btnMultimedia = CrearBotonModulo("5. Recursos Multimedia", 744, 32, Color.FromArgb(46, 125, 50));
            btnMultimedia.Click += (s, e) =>
            {
                using var f = new UI_GestionMultimedia(!(ControllerUsuario.SesionActual?.EsAdministrador ?? true));
                f.ShowDialog();
                ActualizarEncabezadoYTabla();
            };

            Button btnRutas = CrearBotonModulo("6. Rutas y Recorridos", 926, 32, Color.FromArgb(46, 125, 50));
            btnRutas.Click += (s, e) =>
            {
                using var f = new UI_GestionRutas(!(ControllerUsuario.SesionActual?.EsAdministrador ?? true));
                f.ShowDialog();
                ActualizarEncabezadoYTabla();
            };

            grpModulos.Controls.AddRange(new Control[]
            {
                btnArtistas, btnCategorias, btnUbicaciones, btnObras, btnMultimedia, btnRutas
            });
            Controls.Add(grpModulos);

            Button btnConsultasAvanzadas = new Button
            {
                Text = "ABRIR CENTRO DE CONSULTAS Y ESTADÍSTICAS DEL CATÁLOGO",
                Location = new Point(18, 205),
                Size = new Size(1125, 42),
                BackColor = Color.FromArgb(106, 27, 154),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnConsultasAvanzadas.Click += (s, e) =>
            {
                using var f = new UI_ConsultasYEstadisticas();
                f.ShowDialog();
            };
            Controls.Add(btnConsultasAvanzadas);

            dgvResumenPrincipal = new DataGridView
            {
                Location = new Point(18, 258),
                Size = new Size(1125, 375),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            Controls.Add(dgvResumenPrincipal);

            lblEstadoMysql = new Label
            {
                Text = "Estado MySQL:",
                Location = new Point(20, 644),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            Controls.Add(lblEstadoMysql);
        }

        private static Button CrearBotonModulo(string texto, int x, int y, Color colorFondo)
        {
            return new Button
            {
                Text = texto,
                Location = new Point(x, y),
                Size = new Size(174, 52),
                BackColor = colorFondo,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
        }

        private void ActualizarEncabezadoYTabla()
        {
            var usr = ControllerUsuario.SesionActual;
            if (usr != null)
            {
                lblUsuarioActivo.Text = $"Sesión Activa: {usr.NombreCompleto} ({usr.CorreoUsuario}) — Rol: {usr.NombreTipoUsuario.ToUpperInvariant()}";
            }

            if (_ctrlUsuario.ProbarConexionBD(out string msg))
            {
                lblEstadoMysql.Text = $"Estado MySQL: {msg} | Carpeta Multimedia: {_ctrlUsuario.ObtenerDirectorioRaizMultimedia()}\\RecursosMultimedia";
                lblEstadoMysql.ForeColor = Color.DarkGreen;
                dgvResumenPrincipal.DataSource = _ctrlObra.ConsultarCatalogoObras();
            }
            else
            {
                lblEstadoMysql.Text = "Error de conexión: " + msg;
                lblEstadoMysql.ForeColor = Color.DarkRed;
            }
        }

        private void BtnCambiarSesion_Click(object? sender, EventArgs e)
        {
            using var login = new UI_Login();
            if (login.ShowDialog() == DialogResult.OK)
            {
                ActualizarEncabezadoYTabla();
            }
        }
    }
}
