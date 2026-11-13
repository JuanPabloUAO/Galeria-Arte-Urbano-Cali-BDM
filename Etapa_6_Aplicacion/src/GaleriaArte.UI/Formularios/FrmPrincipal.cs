using System;
using System.Drawing;
using System.Windows.Forms;
using GaleriaArte.Negocio;
using GaleriaArte.UI.Utilidades;

namespace GaleriaArte.UI.Formularios;

/// <summary>Menú principal. Muestra los módulos según el tipo de usuario (RN-08).</summary>
internal class FrmPrincipal : Form
{
    public bool CerrarSesionSolicitado { get; private set; }

    public FrmPrincipal()
    {
        Ui.Estilo(this, "Menú principal", 980, 620);
        StartPosition = FormStartPosition.CenterScreen;
        var usuario = Sesion.UsuarioActual!;

        var encabezado = Ui.Encabezado($"Galería de Arte Urbano de Cali   ·   {usuario.NombreCompleto} ({usuario.TipoUsuario})");

        var mosaico = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(30), AutoScroll = true };
        mosaico.Controls.Add(Tarjeta("Obras", "Murales, grafitis e intervenciones", () => new FrmObras()));
        mosaico.Controls.Add(Tarjeta("Artistas", "Autores de las obras", () => new FrmArtistas()));
        mosaico.Controls.Add(Tarjeta("Categorías", "Técnicas y temáticas", () => new FrmCategorias()));
        mosaico.Controls.Add(Tarjeta("Ubicaciones", "Lugares, barrios y comunas", () => new FrmUbicaciones()));
        mosaico.Controls.Add(Tarjeta("Multimedia", "Fotos, videos, audios, 3D y AR", () => new FrmMultimedia()));
        mosaico.Controls.Add(Tarjeta("Rutas", "Recorridos y orden de visita", () => new FrmRutas()));
        mosaico.Controls.Add(Tarjeta("Consultas", "Búsquedas y estadísticas", () => new FrmConsultas()));
        if (Sesion.EsAdministrador)
            mosaico.Controls.Add(Tarjeta("Usuarios", "Cuentas y permisos", () => new FrmUsuarios()));

        var pie = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 52, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) };
        pie.Controls.Add(Ui.Boton("Salir", (_, _) => Close()));
        pie.Controls.Add(Ui.Boton("Cerrar sesión", (_, _) => { CerrarSesionSolicitado = true; Close(); }));
        pie.Controls.Add(new Label
        {
            Text = Sesion.EsAdministrador ? "Administrador: puede registrar, modificar y eliminar." : "Visitante: modo de solo consulta.",
            AutoSize = true, ForeColor = Color.DimGray, Margin = new Padding(3, 10, 30, 3),
        });

        Controls.Add(mosaico);
        Controls.Add(pie);
        Controls.Add(encabezado);
    }

    private Button Tarjeta(string titulo, string descripcion, Func<Form> crear)
    {
        var b = new Button
        {
            Text = titulo + "\n\n" + descripcion,
            Size = new Size(200, 120),
            Margin = new Padding(10),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = Ui.ColorPrimario,
            Font = Ui.FuenteNegrita,
            TextAlign = ContentAlignment.MiddleCenter,
            Cursor = Cursors.Hand,
        };
        b.FlatAppearance.BorderColor = Ui.ColorPrimario;
        b.Click += (_, _) =>
        {
            using var formulario = crear();
            formulario.ShowDialog(this);
        };
        return b;
    }
}
