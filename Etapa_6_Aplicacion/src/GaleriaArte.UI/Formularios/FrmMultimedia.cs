using System;
using System.Drawing;
using System.Windows.Forms;
using GaleriaArte.Entidades;
using GaleriaArte.Negocio;
using GaleriaArte.Negocio.Servicios;
using GaleriaArte.UI.Utilidades;

namespace GaleriaArte.UI.Formularios;

/// <summary>
/// Módulo Multimedia: seleccionar archivos del computador, registrarlos asociados a una obra
/// y visualizarlos (imágenes dentro del formulario) o reproducirlos (video, audio, 3D, AR)
/// con el programa predeterminado de Windows.
/// </summary>
internal class FrmMultimedia : Form
{
    private readonly ObraServicio _obras = new();
    private readonly RecursoServicio _servicio = new();

    private readonly ComboBox _cmbObra = Ui.Combo();
    private readonly DataGridView _grilla = Ui.Grilla();
    private readonly PictureBox _vista = new() { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.FromArgb(30, 30, 30) };
    private readonly Label _lblVista = new() { Dock = DockStyle.Bottom, Height = 60, Padding = new Padding(6), BackColor = Color.White };

    // Controles para registrar un recurso nuevo (solo Administrador)
    private readonly TextBox _txtOrigen = new() { ReadOnly = true, Dock = DockStyle.Fill };
    private readonly TextBox _txtNombre = Ui.Texto();
    private readonly TextBox _txtDescripcion = Ui.Texto();
    private readonly NumericUpDown _numDuracion = new() { Minimum = 0, Maximum = 86400, Width = 100 };
    private bool _origenEsCarpetaAR;

    private readonly string? _idObraInicial;

    public FrmMultimedia() : this(null) { }

    public FrmMultimedia(string? idObra)
    {
        _idObraInicial = idObra;
        Ui.Estilo(this, "Recursos multimedia", 1200, 760);

        Ui.Columnas(_grilla, ("Nombre", "Nombre"), ("TipoRecurso", "Tipo"), ("CodigoFormato", "Formato"),
            ("TamanoLegible", "Tamaño"), ("DuracionSegundos", "Duración (s)"), ("RutaArchivo", "Ruta relativa"));
        _grilla.SelectionChanged += (_, _) => MostrarVistaPrevia();
        _grilla.CellDoubleClick += (_, _) => AbrirSeleccionado();

        // Barra superior: selección de obra
        var barra = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 46, Padding = new Padding(8) };
        barra.Controls.Add(new Label { Text = "Obra:", AutoSize = true, Margin = new Padding(3, 8, 3, 3) });
        _cmbObra.Width = 420;
        _cmbObra.SelectedIndexChanged += (_, _) => CargarRecursos();
        barra.Controls.Add(_cmbObra);
        barra.Controls.Add(Ui.Boton("Abrir / reproducir", (_, _) => AbrirSeleccionado(), principal: true));
        barra.Controls.Add(Ui.Boton("Eliminar recurso", (_, _) => EliminarSeleccionado()) );
        barra.Controls[barra.Controls.Count - 1].Visible = Sesion.EsAdministrador;

        // Centro: grilla | vista previa
        var vistaPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(6) };
        vistaPanel.Controls.Add(_vista);
        vistaPanel.Controls.Add(_lblVista);
        var centro = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(6) };
        centro.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
        centro.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
        centro.Controls.Add(_grilla, 0, 0);
        centro.Controls.Add(vistaPanel, 1, 0);

        Controls.Add(centro);
        if (Sesion.EsAdministrador) Controls.Add(CrearPanelRegistro());
        Controls.Add(barra);
        Controls.Add(Ui.Encabezado("Recursos multimedia"));
    }

    private GroupBox CrearPanelRegistro()
    {
        var grupo = new GroupBox { Text = "Agregar recurso a la obra seleccionada", Dock = DockStyle.Bottom, Height = 190, Padding = new Padding(8), BackColor = Color.White };
        var campos = Ui.PanelCampos();

        var filaArchivo = new TableLayoutPanel { ColumnCount = 3, Height = 36 };
        filaArchivo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        filaArchivo.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        filaArchivo.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        filaArchivo.Controls.Add(_txtOrigen, 0, 0);
        filaArchivo.Controls.Add(Ui.Boton("Elegir archivo...", (_, _) => ElegirArchivo()), 1, 0);
        filaArchivo.Controls.Add(Ui.Boton("Elegir carpeta AR...", (_, _) => ElegirCarpetaAR()), 2, 0);
        Ui.Campo(campos, "Archivo *", filaArchivo, alto: 44);
        Ui.Campo(campos, "Nombre", _txtNombre);
        Ui.Campo(campos, "Descripción", _txtDescripcion);

        var filaFinal = new FlowLayoutPanel { Height = 40 };
        filaFinal.Controls.Add(_numDuracion);
        filaFinal.Controls.Add(new Label { Text = "segundos (solo audio/video; 0 = sin dato)", AutoSize = true, Margin = new Padding(3, 8, 20, 3) });
        filaFinal.Controls.Add(Ui.Boton("Registrar recurso", (_, _) => Registrar(), principal: true));
        Ui.Campo(campos, "Duración", filaFinal, alto: 46);

        grupo.Controls.Add(campos);
        return grupo;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        Ui.Intentar(() =>
        {
            foreach (var o in _obras.Buscar()) _cmbObra.Items.Add(o);
        });
        if (_idObraInicial != null) Ui.SeleccionarPor<Obra>(_cmbObra, o => o.IdObra == _idObraInicial);
        else if (_cmbObra.Items.Count > 0) _cmbObra.SelectedIndex = 0;
    }

    private Obra? ObraActual => _cmbObra.SelectedItem as Obra;

    private void CargarRecursos()
    {
        if (ObraActual == null) return;
        Ui.Intentar(() => Ui.Mostrar(_grilla, _servicio.ListarPorObra(ObraActual.IdObra)));
        MostrarVistaPrevia();
    }

    /// <summary>Imágenes: se muestran en el PictureBox. Otros tipos: se describe cómo abrirlos.</summary>
    private void MostrarVistaPrevia()
    {
        var anterior = _vista.Image;
        _vista.Image = null;
        anterior?.Dispose();

        var r = Ui.Seleccionado<RecursoMultimedia>(_grilla);
        if (r == null) { _lblVista.Text = "Seleccione un recurso."; return; }

        if (!GestorArchivos.Existe(r.RutaArchivo))
        {
            _lblVista.Text = $"⚠ Archivo no encontrado en RecursosMultimedia/{r.RutaArchivo}";
            return;
        }

        if (r.TipoRecurso == Constantes.TipoImagen)
        {
            try { _vista.Image = Ui.CargarImagen(GestorArchivos.RutaAbsoluta(r.RutaArchivo)); }
            catch (Exception) { _lblVista.Text = "No se pudo leer la imagen (archivo dañado)."; return; }
            _lblVista.Text = $"{r.Nombre} · {r.CodigoFormato} · {r.TamanoLegible}\n{r.Descripcion}";
        }
        else
        {
            string accion = r.TipoRecurso switch
            {
                Constantes.TipoVideo => "Doble clic o 'Abrir / reproducir' para ver el video",
                Constantes.TipoAudio => "Doble clic o 'Abrir / reproducir' para escuchar el audio",
                Constantes.TipoModelo3D => "Doble clic para abrir el modelo 3D (Visor 3D de Windows u otro visor .glb)",
                _ => "Doble clic para abrir la carpeta de realidad aumentada",
            };
            _lblVista.Text = $"{r.Nombre} · {r.TipoRecurso} · {r.TamanoLegible}" +
                             (r.DuracionSegundos.HasValue ? $" · {r.DuracionSegundos} s" : "") + "\n" + accion;
        }
    }

    private void AbrirSeleccionado()
    {
        var r = Ui.Seleccionado<RecursoMultimedia>(_grilla);
        if (r == null) { Ui.Informar("Seleccione un recurso."); return; }
        Ui.Intentar(() => Ui.AbrirConProgramaPredeterminado(_servicio.ObtenerRutaParaAbrir(r)));
    }

    private void ElegirArchivo()
    {
        using var dialogo = new OpenFileDialog { Title = "Seleccionar recurso multimedia", Filter = GestorArchivos.FiltroDialogo };
        if (dialogo.ShowDialog(this) != DialogResult.OK) return;
        _txtOrigen.Text = dialogo.FileName;
        _origenEsCarpetaAR = false;
        if (string.IsNullOrWhiteSpace(_txtNombre.Text))
            _txtNombre.Text = System.IO.Path.GetFileNameWithoutExtension(dialogo.FileName);
    }

    private void ElegirCarpetaAR()
    {
        using var dialogo = new FolderBrowserDialog { Description = "Seleccione la carpeta con el contenido de realidad aumentada" };
        if (dialogo.ShowDialog(this) != DialogResult.OK) return;
        _txtOrigen.Text = dialogo.SelectedPath;
        _origenEsCarpetaAR = true;
        if (string.IsNullOrWhiteSpace(_txtNombre.Text)) _txtNombre.Text = "Contenido de realidad aumentada";
    }

    private void Registrar()
    {
        if (ObraActual == null) { Ui.Informar("Seleccione primero la obra."); return; }
        int? duracion = _numDuracion.Value > 0 ? (int)_numDuracion.Value : null;
        RecursoMultimedia? nuevo = null;
        if (!Ui.Intentar(() => nuevo = _servicio.Registrar(ObraActual.IdObra, _txtOrigen.Text, _txtNombre.Text,
                                                          _txtDescripcion.Text, duracion, _origenEsCarpetaAR))) return;
        Ui.Informar($"Recurso registrado.\nSe copió a: RecursosMultimedia/{nuevo!.RutaArchivo}");
        _txtOrigen.Clear(); _txtNombre.Clear(); _txtDescripcion.Clear(); _numDuracion.Value = 0;
        CargarRecursos();
    }

    private void EliminarSeleccionado()
    {
        var r = Ui.Seleccionado<RecursoMultimedia>(_grilla);
        if (r == null) { Ui.Informar("Seleccione un recurso."); return; }
        if (!Ui.Confirmar($"¿Eliminar el recurso '{r.Nombre}'?\nSe borrará el registro y el archivo {r.RutaArchivo}.")) return;
        // Liberar la imagen antes de borrar el archivo
        var img = _vista.Image; _vista.Image = null; img?.Dispose();
        if (Ui.Intentar(() => _servicio.Eliminar(r))) CargarRecursos();
    }
}
