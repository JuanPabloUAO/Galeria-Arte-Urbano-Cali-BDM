using System.Windows.Forms;
using GaleriaArte.Entidades;
using GaleriaArte.Negocio.Servicios;
using GaleriaArte.UI.Utilidades;

namespace GaleriaArte.UI.Formularios;

/// <summary>Módulo Categorías: registrar, consultar, modificar y activar/desactivar.</summary>
internal class FrmCategorias : FrmMantenimiento
{
    private readonly CategoriaServicio _servicio = new();
    private Categoria? _actual;

    private readonly TextBox _txtNombre = Ui.Texto();
    private readonly TextBox _txtDescripcion = Ui.Texto(multilinea: true);
    private readonly Label _lblEstado = new() { AutoSize = true, Font = Ui.FuenteNegrita };

    public FrmCategorias() : base("Categorías")
    {
        Ui.Columnas(Grilla, ("Nombre", "Nombre"), ("Estado", "Estado"), ("TotalObras", "Obras"));
        Ui.Campo(Campos, "Nombre *", _txtNombre);
        Ui.Campo(Campos, "Descripción", _txtDescripcion, alto: 140);
        Ui.Campo(Campos, "Estado", _lblEstado);
        Botonera.Controls.Add(Ui.Boton("Activar / desactivar", (_, _) => CambiarEstado()));
    }

    protected override void Cargar() => Ui.Mostrar(Grilla, _servicio.Listar(TxtBuscar.Text));

    protected override void Limpiar()
    {
        _actual = null;
        _txtNombre.Clear();
        _txtDescripcion.Clear();
        _lblEstado.Text = Constantes.Activo + " (nueva)";
        _txtNombre.Focus();
    }

    protected override void MostrarSeleccion()
    {
        _actual = Ui.Seleccionado<Categoria>(Grilla);
        if (_actual == null) return;
        _txtNombre.Text = _actual.Nombre;
        _txtDescripcion.Text = _actual.Descripcion;
        _lblEstado.Text = _actual.Estado;
        _lblEstado.ForeColor = _actual.Estado == Constantes.Activo ? System.Drawing.Color.SeaGreen : Ui.ColorAcento;
    }

    protected override void Guardar()
    {
        _servicio.Guardar(new Categoria
        {
            IdCategoria = _actual?.IdCategoria ?? string.Empty,
            Nombre = _txtNombre.Text,
            Descripcion = _txtDescripcion.Text,
            Estado = _actual?.Estado ?? Constantes.Activo,
        });
        Ui.Informar("Categoría guardada correctamente.");
    }

    private void CambiarEstado()
    {
        if (_actual == null) { Ui.Informar("Seleccione una categoría de la lista."); return; }
        string nuevo = string.Empty;
        if (Ui.Intentar(() => nuevo = _servicio.CambiarEstado(_actual)))
        {
            Ui.Informar($"La categoría '{_actual.Nombre}' ahora está {nuevo}.");
            Recargar();
        }
    }
}
