using System.Windows.Forms;
using GaleriaArte.Entidades;
using GaleriaArte.Negocio.Servicios;
using GaleriaArte.UI.Utilidades;

namespace GaleriaArte.UI.Formularios;

/// <summary>Módulo Artistas: registrar, consultar, modificar y eliminar/desactivar (RN-02).</summary>
internal class FrmArtistas : FrmMantenimiento
{
    private readonly ArtistaServicio _servicio = new();
    private Artista? _actual; // null = registrando uno nuevo

    private readonly TextBox _txtNombreArtistico = Ui.Texto();
    private readonly TextBox _txtNombreReal = Ui.Texto();
    private readonly TextBox _txtPais = Ui.Texto();
    private readonly TextBox _txtRedSocial = Ui.Texto();
    private readonly TextBox _txtBiografia = Ui.Texto(multilinea: true);
    private readonly ComboBox _cmbEstado = Ui.Combo();

    public FrmArtistas() : base("Artistas")
    {
        Ui.Columnas(Grilla, ("NombreArtistico", "Nombre artístico"), ("NombreReal", "Nombre real"),
            ("PaisOrigen", "País"), ("Estado", "Estado"), ("TotalObras", "Obras"));

        Ui.Campo(Campos, "Nombre artístico *", _txtNombreArtistico);
        Ui.Campo(Campos, "Nombre real", _txtNombreReal);
        Ui.Campo(Campos, "País de origen *", _txtPais);
        Ui.Campo(Campos, "Red social", _txtRedSocial);
        Ui.Campo(Campos, "Biografía", _txtBiografia, alto: 140);
        Ui.Campo(Campos, "Estado", _cmbEstado);
        _cmbEstado.Items.AddRange(Constantes.EstadosBasicos);

        Botonera.Controls.Add(Ui.Boton("Eliminar / desactivar", (_, _) => EliminarODesactivar()));
    }

    protected override void Cargar() => Ui.Mostrar(Grilla, _servicio.Listar(TxtBuscar.Text));

    protected override void Limpiar()
    {
        _actual = null;
        _txtNombreArtistico.Clear();
        _txtNombreReal.Clear();
        _txtPais.Text = "Colombia";
        _txtRedSocial.Clear();
        _txtBiografia.Clear();
        _cmbEstado.SelectedItem = Constantes.Activo;
        _txtNombreArtistico.Focus();
    }

    protected override void MostrarSeleccion()
    {
        _actual = Ui.Seleccionado<Artista>(Grilla);
        if (_actual == null) return;
        _txtNombreArtistico.Text = _actual.NombreArtistico;
        _txtNombreReal.Text = _actual.NombreReal;
        _txtPais.Text = _actual.PaisOrigen;
        _txtRedSocial.Text = _actual.RedSocial;
        _txtBiografia.Text = _actual.Biografia;
        _cmbEstado.SelectedItem = _actual.Estado;
    }

    protected override void Guardar()
    {
        var artista = new Artista
        {
            IdArtista = _actual?.IdArtista ?? string.Empty,
            NombreArtistico = _txtNombreArtistico.Text,
            NombreReal = _txtNombreReal.Text,
            PaisOrigen = _txtPais.Text,
            RedSocial = _txtRedSocial.Text,
            Biografia = _txtBiografia.Text,
            Estado = _cmbEstado.SelectedItem as string ?? Constantes.Activo,
        };
        _servicio.Guardar(artista);
        Ui.Informar("Artista guardado correctamente.");
    }

    private void EliminarODesactivar()
    {
        if (_actual == null) { Ui.Informar("Seleccione un artista de la lista."); return; }
        if (!Ui.Confirmar($"¿Eliminar al artista '{_actual.NombreArtistico}'?\n\nSi tiene obras registradas no se eliminará: quedará INACTIVO.")) return;
        string mensaje = string.Empty;
        if (Ui.Intentar(() => mensaje = _servicio.EliminarODesactivar(_actual)))
        {
            Ui.Informar(mensaje);
            Recargar();
        }
    }
}
