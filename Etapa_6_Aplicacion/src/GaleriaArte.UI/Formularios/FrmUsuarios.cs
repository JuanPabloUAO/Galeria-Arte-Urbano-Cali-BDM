using System.Windows.Forms;
using GaleriaArte.Entidades;
using GaleriaArte.Negocio.Servicios;
using GaleriaArte.UI.Utilidades;

namespace GaleriaArte.UI.Formularios;

/// <summary>Gestión de usuarios (solo Administrador). La contraseña se cifra con BCrypt en la capa de negocio.</summary>
internal class FrmUsuarios : FrmMantenimiento
{
    private readonly UsuarioServicio _servicio = new();
    private Usuario? _actual;

    private readonly TextBox _txtNombre = Ui.Texto();
    private readonly TextBox _txtApellido = Ui.Texto();
    private readonly TextBox _txtCorreo = Ui.Texto();
    private readonly ComboBox _cmbTipo = Ui.Combo();
    private readonly ComboBox _cmbEstado = Ui.Combo();
    private readonly TextBox _txtContrasena = new() { UseSystemPasswordChar = true };
    private readonly Label _lblAyuda = new() { AutoSize = true, ForeColor = System.Drawing.Color.DimGray };

    public FrmUsuarios() : base("Usuarios")
    {
        Ui.Columnas(Grilla, ("NombreCompleto", "Nombre"), ("Correo", "Correo"), ("TipoUsuario", "Tipo"),
            ("Estado", "Estado"), ("FechaRegistro", "Registrado"));
        Ui.Campo(Campos, "Nombre *", _txtNombre);
        Ui.Campo(Campos, "Apellido *", _txtApellido);
        Ui.Campo(Campos, "Correo *", _txtCorreo);
        Ui.Campo(Campos, "Tipo de usuario *", _cmbTipo);
        Ui.Campo(Campos, "Estado *", _cmbEstado);
        Ui.Campo(Campos, "Contraseña", _txtContrasena);
        Ui.Campo(Campos, "", _lblAyuda);
        _cmbTipo.Items.AddRange(Constantes.TiposUsuario);
        _cmbEstado.Items.AddRange(Constantes.EstadosBasicos);
    }

    protected override void Cargar()
    {
        var filtro = TxtBuscar.Text.Trim().ToLowerInvariant();
        Ui.Mostrar(Grilla, _servicio.Listar().FindAll(u =>
            filtro.Length == 0 || u.NombreCompleto.ToLowerInvariant().Contains(filtro) || u.Correo.Contains(filtro)));
    }

    protected override void Limpiar()
    {
        _actual = null;
        _txtNombre.Clear(); _txtApellido.Clear(); _txtCorreo.Clear(); _txtContrasena.Clear();
        _cmbTipo.SelectedItem = Constantes.Visitante;
        _cmbEstado.SelectedItem = Constantes.Activo;
        _lblAyuda.Text = "Mínimo 8 caracteres, con letras y números.";
    }

    protected override void MostrarSeleccion()
    {
        _actual = Ui.Seleccionado<Usuario>(Grilla);
        if (_actual == null) return;
        _txtNombre.Text = _actual.Nombre;
        _txtApellido.Text = _actual.Apellido;
        _txtCorreo.Text = _actual.Correo;
        _cmbTipo.SelectedItem = _actual.TipoUsuario;
        _cmbEstado.SelectedItem = _actual.Estado;
        _txtContrasena.Clear();
        _lblAyuda.Text = "Deje la contraseña vacía para conservar la actual.";
    }

    protected override void Guardar()
    {
        _servicio.Guardar(new Usuario
        {
            IdUsuario = _actual?.IdUsuario ?? string.Empty,
            Nombre = _txtNombre.Text,
            Apellido = _txtApellido.Text,
            Correo = _txtCorreo.Text,
            TipoUsuario = _cmbTipo.SelectedItem as string ?? Constantes.Visitante,
            Estado = _cmbEstado.SelectedItem as string ?? Constantes.Activo,
        }, _txtContrasena.Text);
        Ui.Informar("Usuario guardado correctamente.");
    }
}
