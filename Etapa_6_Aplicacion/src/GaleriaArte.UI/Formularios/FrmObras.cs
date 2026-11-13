using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GaleriaArte.Entidades;
using GaleriaArte.Negocio;
using GaleriaArte.Negocio.Servicios;
using GaleriaArte.UI.Utilidades;

namespace GaleriaArte.UI.Formularios;

/// <summary>Módulo Obras: registrar seleccionando artista, categoría y ubicación; consultar, modificar y eliminar.</summary>
internal class FrmObras : FrmMantenimiento
{
    private readonly ObraServicio _servicio = new();
    private readonly ArtistaServicio _artistas = new();
    private readonly CategoriaServicio _categorias = new();
    private readonly UbicacionServicio _ubicaciones = new();
    private Obra? _actual;

    private List<Artista> _listaArtistas = new();
    private List<Categoria> _listaCategorias = new();

    private readonly TextBox _txtNombre = Ui.Texto();
    private readonly TextBox _txtDescripcion = Ui.Texto(multilinea: true);
    private readonly DateTimePicker _dtpFecha = new() { Format = DateTimePickerFormat.Short, ShowCheckBox = true };
    private readonly ComboBox _cmbEstado = Ui.Combo();
    private readonly ComboBox _cmbArtista = Ui.Combo();
    private readonly ComboBox _cmbCategoria = Ui.Combo();
    private readonly ComboBox _cmbUbicacion = Ui.Combo();
    private readonly Label _lblRegistro = new() { AutoSize = true, ForeColor = System.Drawing.Color.DimGray };

    public FrmObras() : base("Obras")
    {
        TxtBuscar.PlaceholderText = "Nombre de la obra empieza por...";
        Ui.Columnas(Grilla, ("Nombre", "Obra"), ("Artista", "Artista"), ("Categoria", "Categoría"), ("Lugar", "Lugar"),
            ("Barrio", "Barrio"), ("Comuna", "Comuna"), ("EstadoConservacion", "Estado"), ("TotalRecursos", "Recursos"));

        Ui.Campo(Campos, "Nombre *", _txtNombre);
        Ui.Campo(Campos, "Descripción *", _txtDescripcion, alto: 110);
        Ui.Campo(Campos, "Fecha de creación", _dtpFecha);
        Ui.Campo(Campos, "Estado conservación *", _cmbEstado);
        Ui.Campo(Campos, "Artista *", _cmbArtista);
        Ui.Campo(Campos, "Categoría *", _cmbCategoria);
        Ui.Campo(Campos, "Ubicación *", _cmbUbicacion);
        Ui.Campo(Campos, "", _lblRegistro);
        _cmbEstado.Items.AddRange(Constantes.EstadosConservacion);

        Botonera.Controls.Add(Ui.Boton("Eliminar", (_, _) => Eliminar()));
        // "Ver multimedia" se agrega también para el Visitante (fuera de la botonera)
        var verMultimedia = Ui.Boton("Ver multimedia de la obra", (_, _) => AbrirMultimedia());
        Ui.Campo(Campos, "", verMultimedia);
    }

    protected override void OnLoad(EventArgs e)
    {
        Ui.Intentar(() =>
        {
            _listaArtistas = _artistas.Listar();
            _listaCategorias = _categorias.Listar();
            _cmbUbicacion.Items.Clear();
            foreach (var u in _ubicaciones.Listar()) _cmbUbicacion.Items.Add(u);
        });
        base.OnLoad(e);
        foreach (Control c in Campos.Controls) if (c is Button b) b.Enabled = true;
    }

    /// <summary>
    /// En los combos solo aparecen artistas y categorías ACTIVOS,
    /// más el que ya tenga la obra (aunque esté inactivo) para no perder el dato al editar.
    /// </summary>
    private void LlenarCombosSegun(Obra? obra)
    {
        _cmbArtista.Items.Clear();
        foreach (var a in _listaArtistas.Where(a => a.Estado == Constantes.Activo || a.IdArtista == obra?.IdArtista))
            _cmbArtista.Items.Add(a);
        _cmbCategoria.Items.Clear();
        foreach (var c in _listaCategorias.Where(c => c.Estado == Constantes.Activo || c.IdCategoria == obra?.IdCategoria))
            _cmbCategoria.Items.Add(c);
    }

    protected override void Cargar() =>
        Ui.Mostrar(Grilla, _servicio.Buscar(new FiltroObra { NombreEmpiezaPor = TxtBuscar.Text }));

    protected override void Limpiar()
    {
        _actual = null;
        LlenarCombosSegun(null);
        _txtNombre.Clear();
        _txtDescripcion.Clear();
        _dtpFecha.Value = DateTime.Today;
        _dtpFecha.Checked = false;
        _cmbEstado.SelectedItem = "BUENO";
        _cmbArtista.SelectedIndex = _cmbCategoria.SelectedIndex = _cmbUbicacion.SelectedIndex = -1;
        _lblRegistro.Text = "Nueva obra";
        _txtNombre.Focus();
    }

    protected override void MostrarSeleccion()
    {
        _actual = Ui.Seleccionado<Obra>(Grilla);
        if (_actual == null) return;
        LlenarCombosSegun(_actual);
        _txtNombre.Text = _actual.Nombre;
        _txtDescripcion.Text = _actual.Descripcion;
        _dtpFecha.Checked = _actual.FechaCreacion.HasValue;
        _dtpFecha.Value = _actual.FechaCreacion ?? DateTime.Today;
        _cmbEstado.SelectedItem = _actual.EstadoConservacion;
        Ui.SeleccionarPor<Artista>(_cmbArtista, a => a.IdArtista == _actual.IdArtista);
        Ui.SeleccionarPor<Categoria>(_cmbCategoria, c => c.IdCategoria == _actual.IdCategoria);
        Ui.SeleccionarPor<Ubicacion>(_cmbUbicacion, u => u.IdUbicacion == _actual.IdUbicacion);
        _lblRegistro.Text = $"Registrada por {_actual.RegistradoPor} el {_actual.FechaRegistro:dd/MM/yyyy HH:mm} · {_actual.TotalRecursos} recurso(s)";
    }

    protected override void Guardar()
    {
        _servicio.Guardar(new Obra
        {
            IdObra = _actual?.IdObra ?? string.Empty,
            IdUsuario = _actual?.IdUsuario ?? string.Empty,
            Nombre = _txtNombre.Text,
            Descripcion = _txtDescripcion.Text,
            FechaCreacion = _dtpFecha.Checked ? _dtpFecha.Value.Date : null,
            EstadoConservacion = _cmbEstado.SelectedItem as string ?? string.Empty,
            IdArtista = (_cmbArtista.SelectedItem as Artista)?.IdArtista ?? string.Empty,
            IdCategoria = (_cmbCategoria.SelectedItem as Categoria)?.IdCategoria ?? string.Empty,
            IdUbicacion = (_cmbUbicacion.SelectedItem as Ubicacion)?.IdUbicacion ?? string.Empty,
        });
        Ui.Informar("Obra guardada correctamente.");
    }

    private void Eliminar()
    {
        if (_actual == null) { Ui.Informar("Seleccione una obra de la lista."); return; }
        if (!Ui.Confirmar($"¿Eliminar la obra '{_actual.Nombre}'?\n\nTambién se eliminarán sus {_actual.TotalRecursos} recurso(s) multimedia y sus archivos.\n" +
                          "Si la obra solo cambió de estado, considere marcarla como DESTRUIDO en lugar de eliminarla.")) return;
        if (Ui.Intentar(() => _servicio.Eliminar(_actual)))
        {
            Ui.Informar("Obra eliminada.");
            Recargar();
        }
    }

    private void AbrirMultimedia()
    {
        if (_actual == null) { Ui.Informar("Seleccione una obra de la lista."); return; }
        using var frm = new FrmMultimedia(_actual.IdObra);
        frm.ShowDialog(this);
        Recargar(); // actualiza el conteo de recursos
    }
}
