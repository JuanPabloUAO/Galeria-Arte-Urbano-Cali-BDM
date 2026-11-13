using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GaleriaArte.Entidades;
using GaleriaArte.Negocio;
using GaleriaArte.Negocio.Servicios;
using GaleriaArte.UI.Utilidades;

namespace GaleriaArte.UI.Formularios;

/// <summary>
/// Módulo Rutas: crear rutas, agregar y retirar obras, establecer el orden del recorrido
/// y consultar las obras de una ruta. El orden lo mantienen los procedimientos almacenados.
/// Hereda la parte de datos de la ruta de FrmMantenimiento y agrega abajo el panel de paradas.
/// </summary>
internal class FrmRutas : FrmMantenimiento
{
    private readonly RutaServicio _servicio = new();
    private readonly ObraServicio _obras = new();
    private Ruta? _actual;
    private List<ParadaRuta> _paradas = new();

    private readonly TextBox _txtNombre = Ui.Texto();
    private readonly TextBox _txtDescripcion = Ui.Texto(multilinea: true);
    private readonly NumericUpDown _numDuracion = new() { Minimum = 1, Maximum = 1440, Value = 60 };
    private readonly NumericUpDown _numDistancia = new() { Minimum = 0.01m, Maximum = 999.99m, DecimalPlaces = 2, Increment = 0.1m, Value = 1 };
    private readonly ComboBox _cmbEstado = Ui.Combo();

    // Panel de paradas (ruta_obra)
    private readonly DataGridView _grillaParadas = Ui.Grilla();
    private readonly Label _lblParadas = new() { Dock = DockStyle.Top, Height = 30, Font = Ui.FuenteNegrita, Padding = new Padding(4, 6, 0, 0) };
    private readonly ComboBox _cmbObra = Ui.Combo();
    private readonly NumericUpDown _numTiempo = new() { Minimum = 1, Maximum = 600, Value = 10, Width = 70 };
    private readonly TextBox _txtRecomendacion = new() { Width = 260, PlaceholderText = "Recomendación (opcional)" };

    public FrmRutas() : base("Rutas de arte urbano")
    {
        Size = new Size(1200, 820);
        Ui.Columnas(Grilla, ("Nombre", "Ruta"), ("Estado", "Estado"), ("TotalObras", "Obras"),
            ("DuracionEstimadaMin", "Duración (min)"), ("DistanciaEstimadaKm", "Distancia (km)"));

        Ui.Campo(Campos, "Nombre *", _txtNombre);
        Ui.Campo(Campos, "Descripción *", _txtDescripcion, alto: 90);
        Ui.Campo(Campos, "Duración (min) *", _numDuracion);
        Ui.Campo(Campos, "Distancia (km) *", _numDistancia);
        Ui.Campo(Campos, "Estado", _cmbEstado);
        _cmbEstado.Items.AddRange(Constantes.EstadosRuta);
        Botonera.Controls.Add(Ui.Boton("Eliminar ruta", (_, _) => EliminarRuta()));

        Controls.Add(CrearPanelParadas());
        Controls.SetChildIndex(Controls[Controls.Count - 1], 1); // el panel de paradas va debajo del cuerpo
    }

    private Control CrearPanelParadas()
    {
        Ui.Columnas(_grillaParadas, ("OrdenVisita", "Orden"), ("Obra", "Obra"), ("Artista", "Artista"), ("Lugar", "Lugar"),
            ("Barrio", "Barrio"), ("TiempoEstimadoMin", "Tiempo (min)"), ("Recomendacion", "Recomendación"));
        _grillaParadas.Columns["OrdenVisita"]!.FillWeight = 35;
        _grillaParadas.Columns["TiempoEstimadoMin"]!.FillWeight = 50;

        var panel = new Panel { Dock = DockStyle.Bottom, Height = 300, Padding = new Padding(8), BackColor = Color.White };
        panel.Controls.Add(_grillaParadas);

        if (Sesion.EsAdministrador)
        {
            var acciones = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 48, Padding = new Padding(2) };
            _cmbObra.Width = 260;
            acciones.Controls.Add(_cmbObra);
            acciones.Controls.Add(new Label { Text = "min:", AutoSize = true, Margin = new Padding(3, 10, 0, 0) });
            acciones.Controls.Add(_numTiempo);
            acciones.Controls.Add(_txtRecomendacion);
            acciones.Controls.Add(Ui.Boton("Agregar al final", (_, _) => AgregarObra(), principal: true));
            acciones.Controls.Add(Ui.Boton("▲ Subir", (_, _) => Mover(-1)));
            acciones.Controls.Add(Ui.Boton("▼ Bajar", (_, _) => Mover(+1)));
            acciones.Controls.Add(Ui.Boton("Retirar de la ruta", (_, _) => Retirar()));
            panel.Controls.Add(acciones);
        }
        panel.Controls.Add(_lblParadas);
        return panel;
    }

    protected override void OnLoad(EventArgs e)
    {
        Ui.Intentar(() => { foreach (var o in _obras.Buscar()) _cmbObra.Items.Add(o); });
        base.OnLoad(e);
    }

    protected override void Cargar() => Ui.Mostrar(Grilla, _servicio.Listar(TxtBuscar.Text));

    protected override void Limpiar()
    {
        _actual = null;
        _txtNombre.Clear();
        _txtDescripcion.Clear();
        _numDuracion.Value = 60;
        _numDistancia.Value = 1;
        _cmbEstado.SelectedItem = Constantes.Activo;
        _paradas = new List<ParadaRuta>();
        Ui.Mostrar(_grillaParadas, _paradas);
        _lblParadas.Text = "Guarde la ruta para poder agregarle obras.";
    }

    protected override void MostrarSeleccion()
    {
        _actual = Ui.Seleccionado<Ruta>(Grilla);
        if (_actual == null) return;
        _txtNombre.Text = _actual.Nombre;
        _txtDescripcion.Text = _actual.Descripcion;
        _numDuracion.Value = Math.Max(_numDuracion.Minimum, Math.Min(_numDuracion.Maximum, _actual.DuracionEstimadaMin));
        _numDistancia.Value = Math.Max(_numDistancia.Minimum, Math.Min(_numDistancia.Maximum, _actual.DistanciaEstimadaKm));
        _cmbEstado.SelectedItem = _actual.Estado;
        CargarParadas();
    }

    /// <summary>Consulta de ruta: obras que la componen, en orden, con su ubicación.</summary>
    private void CargarParadas(int seleccionar = -1)
    {
        if (_actual == null) return;
        Ui.Intentar(() => _paradas = _servicio.ObtenerParadas(_actual.IdRuta));
        Ui.Mostrar(_grillaParadas, _paradas);
        int minutos = _paradas.Sum(p => p.TiempoEstimadoMin);
        _lblParadas.Text = $"Recorrido de '{_actual.Nombre}': {_paradas.Count} obra(s) · {minutos} min frente a las obras";
        if (seleccionar >= 0 && seleccionar < _grillaParadas.Rows.Count)
            _grillaParadas.CurrentCell = _grillaParadas.Rows[seleccionar].Cells[0];
    }

    protected override void Guardar()
    {
        _servicio.Guardar(new Ruta
        {
            IdRuta = _actual?.IdRuta ?? string.Empty,
            Nombre = _txtNombre.Text,
            Descripcion = _txtDescripcion.Text,
            DuracionEstimadaMin = (int)_numDuracion.Value,
            DistanciaEstimadaKm = _numDistancia.Value,
            Estado = _cmbEstado.SelectedItem as string ?? Constantes.Activo,
        });
        Ui.Informar("Ruta guardada. Ahora puede agregarle obras en el panel inferior.");
    }

    private void EliminarRuta()
    {
        if (_actual == null) { Ui.Informar("Seleccione una ruta."); return; }
        if (!Ui.Confirmar($"¿Eliminar la ruta '{_actual.Nombre}'?\nSe quitarán sus {_actual.TotalObras} parada(s); las obras NO se eliminan.")) return;
        if (Ui.Intentar(() => _servicio.Eliminar(_actual))) Recargar();
    }

    private void AgregarObra()
    {
        if (_actual == null) { Ui.Informar("Seleccione o guarde primero una ruta."); return; }
        var obra = _cmbObra.SelectedItem as Obra;
        if (Ui.Intentar(() => _servicio.AgregarObra(_actual.IdRuta, obra?.IdObra ?? string.Empty, (int)_numTiempo.Value, _txtRecomendacion.Text)))
        {
            _txtRecomendacion.Clear();
            CargarParadas(_paradas.Count); // selecciona la nueva (última)
            RecargarConteo();
        }
    }

    private void Retirar()
    {
        var p = Ui.Seleccionado<ParadaRuta>(_grillaParadas);
        if (p == null) { Ui.Informar("Seleccione una obra del recorrido."); return; }
        if (!Ui.Confirmar($"¿Retirar '{p.Obra}' de la ruta? Las obras siguientes subirán una posición.")) return;
        if (Ui.Intentar(() => _servicio.RetirarObra(p)))
        {
            CargarParadas();
            RecargarConteo();
        }
    }

    private void Mover(int desplazamiento)
    {
        var p = Ui.Seleccionado<ParadaRuta>(_grillaParadas);
        if (p == null) { Ui.Informar("Seleccione una obra del recorrido."); return; }
        if (Ui.Intentar(() => _servicio.Mover(p, desplazamiento, _paradas.Count)))
            CargarParadas(Math.Max(0, Math.Min(_paradas.Count - 1, p.OrdenVisita - 1 + desplazamiento)));
    }

    /// <summary>Actualiza la columna "Obras" de la grilla de rutas sin perder la selección.</summary>
    private void RecargarConteo()
    {
        if (_actual != null) _actual.TotalObras = _paradas.Count;
        Grilla.Refresh();
    }
}
