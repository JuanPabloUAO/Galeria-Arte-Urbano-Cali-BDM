using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GaleriaArte.Entidades;
using GaleriaArte.Negocio;

namespace GaleriaArte.UI.Utilidades;

/// <summary>
/// Utilidades de interfaz: estilo común, creación de controles y manejo de mensajes.
/// Los formularios se construyen por código (sin diseñador) para que todo quede visible y explicado.
/// </summary>
internal static class Ui
{
    public static readonly Color ColorPrimario = Color.FromArgb(31, 78, 121);   // azul del enunciado
    public static readonly Color ColorAcento = Color.FromArgb(230, 57, 70);
    public static readonly Color ColorFondo = Color.FromArgb(245, 247, 250);
    public static readonly Font Fuente = new("Segoe UI", 10F);
    public static readonly Font FuenteTitulo = new("Segoe UI Semibold", 15F);
    public static readonly Font FuenteNegrita = new("Segoe UI Semibold", 10F);

    /// <summary>Aplica tamaño, fuente y colores estándar a un formulario.</summary>
    public static void Estilo(Form f, string titulo, int ancho = 1150, int alto = 700)
    {
        f.Text = titulo + " — Galería de Arte Urbano de Cali";
        f.Font = Fuente;
        f.BackColor = ColorFondo;
        f.StartPosition = FormStartPosition.CenterParent;
        f.Size = new Size(ancho, alto);
        f.MinimumSize = new Size(Math.Min(ancho, 900), Math.Min(alto, 560));
    }

    public static Label Encabezado(string texto) => new()
    {
        Text = texto,
        Dock = DockStyle.Top,
        Height = 52,
        Font = FuenteTitulo,
        ForeColor = Color.White,
        BackColor = ColorPrimario,
        TextAlign = ContentAlignment.MiddleLeft,
        Padding = new Padding(16, 0, 0, 0),
    };

    public static Button Boton(string texto, EventHandler alHacerClic, bool principal = false)
    {
        var b = new Button
        {
            Text = texto,
            AutoSize = true,
            MinimumSize = new Size(110, 34),
            Margin = new Padding(4),
            FlatStyle = FlatStyle.Flat,
            BackColor = principal ? ColorPrimario : Color.White,
            ForeColor = principal ? Color.White : ColorPrimario,
            Cursor = Cursors.Hand,
        };
        b.FlatAppearance.BorderColor = ColorPrimario;
        b.Click += alHacerClic;
        return b;
    }

    public static DataGridView Grilla()
    {
        var g = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            AutoGenerateColumns = false,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible = false,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            EnableHeadersVisualStyles = false,
        };
        g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 230, 241);
        g.ColumnHeadersDefaultCellStyle.Font = FuenteNegrita;
        g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        return g;
    }

    /// <summary>Define qué propiedades de la entidad se muestran como columnas y con qué título.</summary>
    public static void Columnas(DataGridView g, params (string Propiedad, string Titulo)[] columnas)
    {
        g.Columns.Clear();
        foreach (var (propiedad, titulo) in columnas)
            g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = propiedad, HeaderText = titulo, Name = propiedad });
    }

    /// <summary>Asigna una lista a la grilla (se crea una copia para que la grilla detecte el cambio).</summary>
    public static void Mostrar<T>(DataGridView g, IEnumerable<T> datos) => g.DataSource = new List<T>(datos);

    public static T? Seleccionado<T>(DataGridView g) where T : class => g.CurrentRow?.DataBoundItem as T;

    /// <summary>Panel de 2 columnas (etiqueta | control) para los campos de edición.</summary>
    public static TableLayoutPanel PanelCampos()
    {
        var p = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            AutoScroll = true,
            Padding = new Padding(8),
        };
        p.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        p.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        return p;
    }

    /// <summary>Agrega una fila "Etiqueta: control" al panel y devuelve el control.</summary>
    public static T Campo<T>(TableLayoutPanel panel, string etiqueta, T control, int alto = 0) where T : Control
    {
        var fila = panel.RowCount++;
        panel.RowStyles.Add(alto > 0 ? new RowStyle(SizeType.Absolute, alto) : new RowStyle(SizeType.AutoSize));
        panel.Controls.Add(new Label { Text = etiqueta, AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Top, Margin = new Padding(3, 8, 3, 3) }, 0, fila);
        control.Dock = DockStyle.Fill;
        control.Margin = new Padding(3, 4, 3, 4);
        panel.Controls.Add(control, 1, fila);
        return control;
    }

    public static TextBox Texto(bool multilinea = false) => new()
    {
        Multiline = multilinea,
        ScrollBars = multilinea ? ScrollBars.Vertical : ScrollBars.None,
    };

    public static ComboBox Combo() => new() { DropDownStyle = ComboBoxStyle.DropDownList };

    /// <summary>Selecciona en el combo el elemento cuya clave coincide (por ejemplo, el id del artista).</summary>
    public static void SeleccionarPor<T>(ComboBox combo, Func<T, bool> condicion)
    {
        combo.SelectedIndex = -1;
        for (int i = 0; i < combo.Items.Count; i++)
            if (combo.Items[i] is T item && condicion(item)) { combo.SelectedIndex = i; return; }
    }

    // ---------- Mensajes ----------

    public static void Informar(string mensaje) =>
        MessageBox.Show(mensaje, "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

    public static bool Confirmar(string mensaje) =>
        MessageBox.Show(mensaje, "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes;

    /// <summary>
    /// Muestra un error. Las validaciones y errores de base de datos ya traen un mensaje
    /// amigable (advertencia); cualquier otro error inesperado se muestra como error.
    /// </summary>
    public static void MostrarError(Exception ex)
    {
        if (ex is GaleriaException)
            MessageBox.Show(ex.Message, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        else
            MessageBox.Show("Ocurrió un error inesperado:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    /// <summary>Ejecuta una acción mostrando cualquier error. Devuelve true si terminó bien.</summary>
    public static bool Intentar(Action accion)
    {
        try
        {
            Cursor.Current = Cursors.WaitCursor;
            accion();
            return true;
        }
        catch (Exception ex)
        {
            MostrarError(ex);
            return false;
        }
        finally
        {
            Cursor.Current = Cursors.Default;
        }
    }

    /// <summary>Abre un archivo con el programa predeterminado de Windows (reproductor, visor 3D, explorador...).</summary>
    public static void AbrirConProgramaPredeterminado(string rutaCompleta)
    {
        if (!File.Exists(rutaCompleta) && !Directory.Exists(rutaCompleta))
            throw new ValidacionException("El archivo no existe en el disco.");
        Process.Start(new ProcessStartInfo(rutaCompleta) { UseShellExecute = true });
    }

    /// <summary>Carga una imagen SIN bloquear el archivo (permite borrarlo luego desde la app).</summary>
    public static Image? CargarImagen(string rutaCompleta)
    {
        if (!File.Exists(rutaCompleta)) return null;
        using var flujo = new FileStream(rutaCompleta, FileMode.Open, FileAccess.Read);
        using var original = Image.FromStream(flujo);
        return new Bitmap(original);
    }
}

/// <summary>Elemento para combos con opción "(Todos)". Valor null = sin filtro.</summary>
internal class Opcion
{
    public Opcion(string texto, object? valor) { Texto = texto; Valor = valor; }
    public string Texto { get; }
    public object? Valor { get; }
    public override string ToString() => Texto;
}
