using System;
using System.Drawing;
using System.Windows.Forms;
using GaleriaArte.Negocio.Servicios;
using GaleriaArte.UI.Utilidades;

namespace GaleriaArte.UI.Formularios;

/// <summary>Inicio de sesión con correo y contraseña (verificada contra el hash BCrypt).</summary>
internal class FrmLogin : Form
{
    private readonly AutenticacionServicio _servicio = new();
    private readonly TextBox _txtCorreo = new() { Width = 300 };
    private readonly TextBox _txtContrasena = new() { Width = 300, UseSystemPasswordChar = true };

    public FrmLogin()
    {
        Ui.Estilo(this, "Iniciar sesión", 460, 430);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        var titulo = new Label
        {
            Text = "Galería de Arte\nUrbano de Cali",
            Font = new Font("Segoe UI Semibold", 18F),
            ForeColor = Color.White, BackColor = Ui.ColorPrimario,
            Dock = DockStyle.Top, Height = 110, TextAlign = ContentAlignment.MiddleCenter,
        };

        var cuerpo = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, Padding = new Padding(70, 20, 20, 10), WrapContents = false,
        };
        cuerpo.Controls.Add(new Label { Text = "Correo", AutoSize = true });
        cuerpo.Controls.Add(_txtCorreo);
        cuerpo.Controls.Add(new Label { Text = "Contraseña", AutoSize = true, Margin = new Padding(3, 12, 3, 3) });
        cuerpo.Controls.Add(_txtContrasena);
        var ingresar = Ui.Boton("Ingresar", (_, _) => Ingresar(), principal: true);
        ingresar.Width = 300; ingresar.Margin = new Padding(3, 20, 3, 3);
        cuerpo.Controls.Add(ingresar);
        cuerpo.Controls.Add(new Label
        {
            Text = "Usuarios de prueba (script 02):\nadmin@galeriacali.co / Admin123*\nvisitante@galeriacali.co / Visitante123*",
            AutoSize = true, ForeColor = Color.Gray, Margin = new Padding(3, 16, 3, 3),
        });

        Controls.Add(cuerpo);
        Controls.Add(titulo);
        AcceptButton = ingresar; // Enter = Ingresar
    }

    private void Ingresar()
    {
        if (Ui.Intentar(() => _servicio.IniciarSesion(_txtCorreo.Text, _txtContrasena.Text)))
        {
            DialogResult = DialogResult.OK;
            Close();
        }
        else
        {
            _txtContrasena.Clear();
            _txtContrasena.Focus();
        }
    }
}
