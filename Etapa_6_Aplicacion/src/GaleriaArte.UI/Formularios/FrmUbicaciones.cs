using System.Globalization;
using System.Windows.Forms;
using GaleriaArte.Entidades;
using GaleriaArte.Negocio;
using GaleriaArte.Negocio.Servicios;
using GaleriaArte.UI.Utilidades;

namespace GaleriaArte.UI.Formularios;

/// <summary>Módulo Ubicaciones: registrar, consultar y modificar lugares (con barrio y coordenadas).</summary>
internal class FrmUbicaciones : FrmMantenimiento
{
    private readonly UbicacionServicio _servicio = new();
    private Ubicacion? _actual;

    private readonly TextBox _txtLugar = Ui.Texto();
    private readonly TextBox _txtDireccion = Ui.Texto();
    private readonly ComboBox _cmbBarrio = Ui.Combo();
    private readonly NumericUpDown _numLatitud = Coordenada(UbicacionServicio.LatitudMin, UbicacionServicio.LatitudMax);
    private readonly NumericUpDown _numLongitud = Coordenada(UbicacionServicio.LongitudMin, UbicacionServicio.LongitudMax);

    public FrmUbicaciones() : base("Ubicaciones")
    {
        Ui.Columnas(Grilla, ("NombreLugar", "Lugar"), ("Direccion", "Dirección"), ("NombreBarrio", "Barrio"),
            ("Comuna", "Comuna"), ("Latitud", "Latitud"), ("Longitud", "Longitud"));

        Ui.Campo(Campos, "Nombre del lugar *", _txtLugar);
        Ui.Campo(Campos, "Dirección *", _txtDireccion);

        // Barrio + botón para crear un barrio nuevo
        var filaBarrio = new TableLayoutPanel { ColumnCount = 2, Height = 34 };
        filaBarrio.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        filaBarrio.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _cmbBarrio.Dock = DockStyle.Fill;
        filaBarrio.Controls.Add(_cmbBarrio, 0, 0);
        filaBarrio.Controls.Add(Ui.Boton("+ Barrio", (_, _) => NuevoBarrio()), 1, 0);
        Ui.Campo(Campos, "Barrio *", filaBarrio, alto: 44);

        Ui.Campo(Campos, "Latitud *", _numLatitud);
        Ui.Campo(Campos, "Longitud *", _numLongitud);
        Ui.Campo(Campos, "", new Label
        {
            Text = "Rango permitido (área de Cali): latitud 3.25 a 3.55; longitud -76.60 a -76.45.\n" +
                   "Tip: en Google Maps, clic derecho sobre el lugar → copie las coordenadas.",
            AutoSize = true, ForeColor = System.Drawing.Color.DimGray,
        });

        // El botón de mapa también lo puede usar el Visitante, por eso va en el panel de campos
        var verMapa = Ui.Boton("Ver en mapa", (_, _) => VerEnMapa());
        Ui.Campo(Campos, "", verMapa);
    }

    private static NumericUpDown Coordenada(decimal min, decimal max) => new()
    {
        DecimalPlaces = 6, Increment = 0.0001m, Minimum = min, Maximum = max, Value = (min + max) / 2,
    };

    protected override void OnLoad(System.EventArgs e)
    {
        CargarBarrios();
        base.OnLoad(e);
        // El botón "Ver en mapa" debe seguir habilitado para el visitante
        foreach (Control c in Campos.Controls) if (c is Button b) b.Enabled = true;
    }

    private void CargarBarrios()
    {
        Ui.Intentar(() =>
        {
            _cmbBarrio.Items.Clear();
            foreach (var b in _servicio.ListarBarrios()) _cmbBarrio.Items.Add(b);
        });
    }

    protected override void Cargar() => Ui.Mostrar(Grilla, _servicio.Listar(TxtBuscar.Text));

    protected override void Limpiar()
    {
        _actual = null;
        _txtLugar.Clear();
        _txtDireccion.Clear();
        _cmbBarrio.SelectedIndex = -1;
        _numLatitud.Value = 3.4516m;    // centro aproximado de Cali
        _numLongitud.Value = -76.5320m;
        _txtLugar.Focus();
    }

    protected override void MostrarSeleccion()
    {
        _actual = Ui.Seleccionado<Ubicacion>(Grilla);
        if (_actual == null) return;
        _txtLugar.Text = _actual.NombreLugar;
        _txtDireccion.Text = _actual.Direccion;
        Ui.SeleccionarPor<Barrio>(_cmbBarrio, b => b.IdBarrio == _actual.IdBarrio);
        _numLatitud.Value = _actual.Latitud;
        _numLongitud.Value = _actual.Longitud;
    }

    protected override void Guardar()
    {
        _servicio.Guardar(new Ubicacion
        {
            IdUbicacion = _actual?.IdUbicacion ?? string.Empty,
            NombreLugar = _txtLugar.Text,
            Direccion = _txtDireccion.Text,
            IdBarrio = (_cmbBarrio.SelectedItem as Barrio)?.IdBarrio ?? 0,
            Latitud = _numLatitud.Value,
            Longitud = _numLongitud.Value,
        });
        Ui.Informar("Ubicación guardada correctamente.");
    }

    private void NuevoBarrio()
    {
        using var dialogo = new FrmNuevoBarrio();
        if (dialogo.ShowDialog(this) != DialogResult.OK) return;
        Barrio? creado = null;
        if (Ui.Intentar(() => creado = _servicio.CrearBarrio(dialogo.NombreBarrio, dialogo.Comuna)))
        {
            CargarBarrios();
            Ui.SeleccionarPor<Barrio>(_cmbBarrio, b => b.IdBarrio == creado!.IdBarrio);
        }
    }

    private void VerEnMapa()
    {
        var lat = _numLatitud.Value.ToString(CultureInfo.InvariantCulture);
        var lon = _numLongitud.Value.ToString(CultureInfo.InvariantCulture);
        Ui.Intentar(() => System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(
            $"https://www.openstreetmap.org/?mlat={lat}&mlon={lon}#map=18/{lat}/{lon}") { UseShellExecute = true }));
    }
}

/// <summary>Diálogo pequeño para registrar un barrio (nombre + comuna).</summary>
internal class FrmNuevoBarrio : Form
{
    private readonly TextBox _txtNombre = Ui.Texto();
    private readonly NumericUpDown _numComuna = new() { Minimum = 1, Maximum = 22, Value = 1 };

    public string NombreBarrio => _txtNombre.Text;
    public int Comuna => (int)_numComuna.Value;

    public FrmNuevoBarrio()
    {
        Ui.Estilo(this, "Nuevo barrio", 420, 230);
        MinimumSize = Size;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = MinimizeBox = false;

        var campos = Ui.PanelCampos();
        Ui.Campo(campos, "Nombre *", _txtNombre);
        Ui.Campo(campos, "Comuna (1-22) *", _numComuna);
        var botones = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 50, FlowDirection = FlowDirection.RightToLeft };
        var aceptar = Ui.Boton("Aceptar", (_, _) => { DialogResult = DialogResult.OK; }, principal: true);
        botones.Controls.Add(aceptar);
        botones.Controls.Add(Ui.Boton("Cancelar", (_, _) => { DialogResult = DialogResult.Cancel; }));
        Controls.Add(campos);
        Controls.Add(botones);
        AcceptButton = aceptar;
    }
}
