using System;
using System.Drawing;
using System.Windows.Forms;
using GaleriaArte.Negocio;
using GaleriaArte.UI.Utilidades;

namespace GaleriaArte.UI.Formularios;

/// <summary>
/// Formulario BASE para los módulos de registro (Artistas, Categorías, Ubicaciones, Obras, Usuarios).
/// Arma la misma estructura para todos:
///
///   ┌──────────── Encabezado ─────────────┐
///   │ [Buscar...] [Buscar] │  Campos      │
///   │  Grilla con los      │  de edición  │
///   │  registros           │              │
///   │                      │ [Nuevo][Guardar][...] │
///   └──────────────────────┴──────────────┘
///
/// Cada formulario hijo solo define sus campos y sobrescribe 4 métodos:
/// Cargar(), Limpiar(), MostrarSeleccion() y Guardar().
/// Si el usuario es Visitante, los campos quedan de solo lectura y se ocultan los botones.
/// </summary>
internal class FrmMantenimiento : Form
{
    protected readonly DataGridView Grilla = Ui.Grilla();
    protected readonly TextBox TxtBuscar = new() { Width = 260, PlaceholderText = "Escriba para buscar..." };
    protected readonly TableLayoutPanel Campos = Ui.PanelCampos();
    protected readonly FlowLayoutPanel Botonera = new() { Dock = DockStyle.Bottom, Height = 52, Padding = new Padding(6), FlowDirection = FlowDirection.LeftToRight };
    protected readonly Label LblEstado = new() { Dock = DockStyle.Bottom, Height = 26, ForeColor = Color.DimGray, Padding = new Padding(8, 4, 0, 0) };

    private bool _cargando;

    // Constructor sin parámetros requerido por el diseñador de Visual Studio
    public FrmMantenimiento() : this("Mantenimiento") { }

    public FrmMantenimiento(string titulo)
    {
        Ui.Estilo(this, titulo);

        // Columna izquierda: búsqueda + grilla
        var barraBusqueda = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(6) };
        barraBusqueda.Controls.Add(TxtBuscar);
        barraBusqueda.Controls.Add(Ui.Boton("Buscar", (_, _) => Recargar()));
        TxtBuscar.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) { Recargar(); e.SuppressKeyPress = true; } };

        var izquierda = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8) };
        izquierda.Controls.Add(Grilla);
        izquierda.Controls.Add(barraBusqueda);

        // Columna derecha: campos + botones
        var derecha = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8), BackColor = Color.White };
        derecha.Controls.Add(Campos);
        derecha.Controls.Add(Botonera);

        var cuerpo = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
        cuerpo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
        cuerpo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
        cuerpo.Controls.Add(izquierda, 0, 0);
        cuerpo.Controls.Add(derecha, 1, 0);

        Controls.Add(cuerpo);
        Controls.Add(LblEstado);
        Controls.Add(Ui.Encabezado(titulo));

        Botonera.Controls.Add(Ui.Boton("Nuevo", (_, _) => { Grilla.ClearSelection(); Limpiar(); }));
        Botonera.Controls.Add(Ui.Boton("Guardar", (_, _) => { if (Ui.Intentar(Guardar)) Recargar(); }, principal: true));

        Grilla.SelectionChanged += (_, _) =>
        {
            if (_cargando) return;
            if (Grilla.CurrentRow == null) Limpiar(); else MostrarSeleccion();
        };
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        AplicarPermisos();
        Recargar();
    }

    /// <summary>Recarga la grilla conservando, si es posible, la fila seleccionada.</summary>
    protected void Recargar()
    {
        var filaAnterior = Grilla.CurrentRow?.Index ?? -1;
        _cargando = true;
        bool ok = Ui.Intentar(Cargar);
        _cargando = false;
        if (!ok) return;

        LblEstado.Text = $"{Grilla.Rows.Count} registro(s)" + (Sesion.EsAdministrador ? "" : "   ·   Modo consulta (Visitante): solo lectura");
        if (filaAnterior >= 0 && filaAnterior < Grilla.Rows.Count)
            Grilla.CurrentCell = Grilla.Rows[filaAnterior].Cells[0];
        if (Grilla.CurrentRow != null) MostrarSeleccion(); else Limpiar();
    }

    /// <summary>Visitante: todo en solo lectura y sin botones de edición (RN-08).</summary>
    private void AplicarPermisos()
    {
        if (Sesion.EsAdministrador) return;
        Botonera.Visible = false;
        foreach (Control c in Campos.Controls)
        {
            if (c is TextBox t) t.ReadOnly = true;
            else if (c is not Label) c.Enabled = false;
        }
    }

    // ---------- Métodos que cada formulario hijo implementa ----------

    /// <summary>Consulta los datos (usando TxtBuscar.Text como filtro) y los asigna a la grilla.</summary>
    protected virtual void Cargar() { }

    /// <summary>Deja los campos vacíos para registrar uno nuevo.</summary>
    protected virtual void Limpiar() { }

    /// <summary>Copia el registro seleccionado en la grilla a los campos.</summary>
    protected virtual void MostrarSeleccion() { }

    /// <summary>Lee los campos y llama al servicio para insertar o actualizar.</summary>
    protected virtual void Guardar() { }
}
