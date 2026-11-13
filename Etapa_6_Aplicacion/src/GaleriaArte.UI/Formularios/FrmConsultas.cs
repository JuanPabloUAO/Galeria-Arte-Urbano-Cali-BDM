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
/// Consultas requeridas (sección 12 del enunciado), que combinan varias tablas:
///  1. Ficha de obra (obra, artista, categoría, ubicación y recursos multimedia)
///  2. Obras por artista / por categoría / por ubicación (barrio o comuna)
///  3. Consulta de ruta (obras, orden y ubicación)
///  4. Recursos multimedia de una obra (dentro de la ficha)
///  5. Estadísticas con agregación (GROUP BY / COUNT / HAVING)
/// </summary>
internal class FrmConsultas : Form
{
    private readonly ObraServicio _obras = new();
    private readonly RecursoServicio _recursos = new();
    private readonly ArtistaServicio _artistas = new();
    private readonly CategoriaServicio _categorias = new();
    private readonly UbicacionServicio _ubicaciones = new();
    private readonly RutaServicio _rutas = new();
    private readonly EstadisticaServicio _estadisticas = new();

    // Cada pestaña carga sus datos la primera vez que se muestra
    private readonly Dictionary<TabPage, Action> _alMostrar = new();
    private readonly TabControl _pestanas = new() { Dock = DockStyle.Fill, Padding = new Point(14, 6) };

    public FrmConsultas()
    {
        Ui.Estilo(this, "Consultas y estadísticas", 1250, 780);
        _pestanas.TabPages.Add(PestanaFichaObra());
        _pestanas.TabPages.Add(PestanaFiltros());
        _pestanas.TabPages.Add(PestanaRutas());
        _pestanas.TabPages.Add(PestanaEstadisticas());
        _pestanas.SelectedIndexChanged += (_, _) => MostrarPestanaActual();
        Controls.Add(_pestanas);
        Controls.Add(Ui.Encabezado("Consultas y estadísticas"));
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        MostrarPestanaActual();
    }

    private void MostrarPestanaActual()
    {
        if (_pestanas.SelectedTab != null && _alMostrar.TryGetValue(_pestanas.SelectedTab, out var cargar))
            cargar();
    }

    // =====================================================================
    // 1. FICHA DE OBRA + RECURSOS MULTIMEDIA
    // =====================================================================
    private TabPage PestanaFichaObra()
    {
        var pagina = new TabPage("Ficha de obra");
        var txtBuscar = new TextBox { Width = 260, PlaceholderText = "Nombre empieza por..." };
        var grillaObras = Ui.Grilla();
        Ui.Columnas(grillaObras, ("Nombre", "Obra"), ("Artista", "Artista"));
        var ficha = new TextBox { Multiline = true, ReadOnly = true, Dock = DockStyle.Top, Height = 190, BackColor = Color.White, ScrollBars = ScrollBars.Vertical };
        var grillaRecursos = Ui.Grilla();
        Ui.Columnas(grillaRecursos, ("Nombre", "Recurso"), ("TipoRecurso", "Tipo"), ("CodigoFormato", "Formato"), ("TamanoLegible", "Tamaño"), ("RutaArchivo", "Ruta relativa"));
        var imagen = new PictureBox { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.FromArgb(30, 30, 30) };

        void Buscar() => Ui.Intentar(() => Ui.Mostrar(grillaObras, _obras.Buscar(new FiltroObra { NombreEmpiezaPor = txtBuscar.Text })));

        grillaObras.SelectionChanged += (_, _) =>
        {
            var o = Ui.Seleccionado<Obra>(grillaObras);
            var anterior = imagen.Image; imagen.Image = null; anterior?.Dispose();
            if (o == null) { ficha.Clear(); grillaRecursos.DataSource = null; return; }

            ficha.Text =
                $"OBRA: {o.Nombre}{Environment.NewLine}" +
                $"Artista: {o.Artista}   ·   Categoría: {o.Categoria}{Environment.NewLine}" +
                $"Ubicación: {o.Lugar} — barrio {o.Barrio}, comuna {o.Comuna}{Environment.NewLine}" +
                $"Fecha de creación: {(o.FechaCreacion.HasValue ? o.FechaCreacion.Value.ToString("dd/MM/yyyy") : "sin dato")}   ·   " +
                $"Estado: {o.EstadoConservacion}{Environment.NewLine}{Environment.NewLine}{o.Descripcion}";

            Ui.Intentar(() =>
            {
                var lista = _recursos.ListarPorObra(o.IdObra);
                Ui.Mostrar(grillaRecursos, lista);
                // Vista previa de la primera imagen disponible
                var primera = lista.FirstOrDefault(r => r.TipoRecurso == Constantes.TipoImagen && GestorArchivos.Existe(r.RutaArchivo));
                if (primera != null) imagen.Image = Ui.CargarImagen(GestorArchivos.RutaAbsoluta(primera.RutaArchivo));
            });
        };
        grillaRecursos.CellDoubleClick += (_, _) =>
        {
            var r = Ui.Seleccionado<RecursoMultimedia>(grillaRecursos);
            if (r != null) Ui.Intentar(() => Ui.AbrirConProgramaPredeterminado(_recursos.ObtenerRutaParaAbrir(r)));
        };
        txtBuscar.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) { Buscar(); e.SuppressKeyPress = true; } };

        var barra = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(4) };
        barra.Controls.Add(txtBuscar);
        barra.Controls.Add(Ui.Boton("Buscar", (_, _) => Buscar(), principal: true));

        var izquierda = new Panel { Dock = DockStyle.Fill };
        izquierda.Controls.Add(grillaObras);
        izquierda.Controls.Add(barra);

        var derecha = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
        derecha.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        derecha.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        var arriba = new Panel { Dock = DockStyle.Fill };
        arriba.Controls.Add(imagen);
        arriba.Controls.Add(ficha);
        var abajo = new Panel { Dock = DockStyle.Fill };
        abajo.Controls.Add(grillaRecursos);
        abajo.Controls.Add(new Label { Text = "Recursos multimedia (doble clic para abrir / reproducir)", Dock = DockStyle.Top, Height = 26, Font = Ui.FuenteNegrita });
        derecha.Controls.Add(arriba, 0, 0);
        derecha.Controls.Add(abajo, 0, 1);

        pagina.Controls.Add(DosColumnas(izquierda, derecha, 35));
        _alMostrar[pagina] = () => { if (grillaObras.Rows.Count == 0) Buscar(); };
        return pagina;
    }

    // =====================================================================
    // 2. OBRAS POR ARTISTA / CATEGORÍA / UBICACIÓN (filtros combinables)
    // =====================================================================
    private TabPage PestanaFiltros()
    {
        var pagina = new TabPage("Obras por artista, categoría o ubicación");
        var cmbArtista = Ui.Combo(); var cmbCategoria = Ui.Combo(); var cmbComuna = Ui.Combo(); var cmbBarrio = Ui.Combo();
        foreach (var c in new[] { cmbArtista, cmbCategoria, cmbComuna, cmbBarrio }) c.Width = 210;
        var grilla = Ui.Grilla();
        Ui.Columnas(grilla, ("Nombre", "Obra"), ("Artista", "Artista"), ("Categoria", "Categoría"), ("Lugar", "Lugar"),
            ("Barrio", "Barrio"), ("Comuna", "Comuna"), ("EstadoConservacion", "Estado"));
        var lblTotal = new Label { Dock = DockStyle.Bottom, Height = 28, Padding = new Padding(6, 6, 0, 0) };

        void Consultar()
        {
            var filtro = new FiltroObra
            {
                IdArtista = (cmbArtista.SelectedItem as Opcion)?.Valor as string,
                IdCategoria = (cmbCategoria.SelectedItem as Opcion)?.Valor as string,
                Comuna = (cmbComuna.SelectedItem as Opcion)?.Valor as int?,
                IdBarrio = (cmbBarrio.SelectedItem as Opcion)?.Valor as int?,
            };
            Ui.Intentar(() =>
            {
                var lista = _obras.Buscar(filtro);
                Ui.Mostrar(grilla, lista);
                lblTotal.Text = $"{lista.Count} obra(s) encontradas";
            });
        }

        _alMostrar[pagina] = () =>
        {
            if (cmbArtista.Items.Count > 0) return; // ya cargado
            Ui.Intentar(() =>
            {
                cmbArtista.Items.Add(new Opcion("(Todos los artistas)", null));
                foreach (var a in _artistas.Listar()) cmbArtista.Items.Add(new Opcion(a.ToString(), a.IdArtista));
                cmbCategoria.Items.Add(new Opcion("(Todas las categorías)", null));
                foreach (var c in _categorias.Listar()) cmbCategoria.Items.Add(new Opcion(c.ToString(), c.IdCategoria));
                cmbComuna.Items.Add(new Opcion("(Todas las comunas)", null));
                foreach (var n in _ubicaciones.ListarComunas()) cmbComuna.Items.Add(new Opcion($"Comuna {n}", n));
                cmbBarrio.Items.Add(new Opcion("(Todos los barrios)", null));
                foreach (var b in _ubicaciones.ListarBarrios()) cmbBarrio.Items.Add(new Opcion(b.ToString(), b.IdBarrio));
            });
            foreach (var c in new[] { cmbArtista, cmbCategoria, cmbComuna, cmbBarrio }) if (c.Items.Count > 0) c.SelectedIndex = 0;
            Consultar();
        };

        var barra = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 84, Padding = new Padding(4) };
        void Agregar(string texto, Control c) { barra.Controls.Add(new Label { Text = texto, AutoSize = true, Margin = new Padding(3, 8, 0, 0) }); barra.Controls.Add(c); }
        Agregar("Artista:", cmbArtista);
        Agregar("Categoría:", cmbCategoria);
        Agregar("Comuna:", cmbComuna);
        Agregar("Barrio:", cmbBarrio);
        barra.Controls.Add(Ui.Boton("Consultar", (_, _) => Consultar(), principal: true));

        pagina.Controls.Add(grilla);
        pagina.Controls.Add(lblTotal);
        pagina.Controls.Add(barra);
        return pagina;
    }

    // =====================================================================
    // 3. CONSULTA DE RUTA
    // =====================================================================
    private TabPage PestanaRutas()
    {
        var pagina = new TabPage("Consulta de ruta");
        var cmbRuta = Ui.Combo(); cmbRuta.Width = 360;
        var lblResumen = new Label { AutoSize = true, Margin = new Padding(20, 8, 0, 0), Font = Ui.FuenteNegrita };
        var grilla = Ui.Grilla();
        Ui.Columnas(grilla, ("OrdenVisita", "Orden"), ("Obra", "Obra"), ("Artista", "Artista"), ("Lugar", "Lugar"),
            ("Direccion", "Dirección"), ("Barrio", "Barrio"), ("Comuna", "Comuna"), ("TiempoEstimadoMin", "Min"), ("Recomendacion", "Recomendación"));

        cmbRuta.SelectedIndexChanged += (_, _) =>
        {
            if (cmbRuta.SelectedItem is not Ruta r) return;
            Ui.Intentar(() =>
            {
                var paradas = _rutas.ObtenerParadas(r.IdRuta);
                Ui.Mostrar(grilla, paradas);
                lblResumen.Text = $"{r.Estado} · {paradas.Count} obras · {r.DuracionEstimadaMin} min · {r.DistanciaEstimadaKm} km";
            });
        };
        _alMostrar[pagina] = () =>
        {
            if (cmbRuta.Items.Count > 0) return;
            Ui.Intentar(() => { foreach (var r in _rutas.Listar()) cmbRuta.Items.Add(r); });
            if (cmbRuta.Items.Count > 0) cmbRuta.SelectedIndex = 0;
        };

        var barra = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(4) };
        barra.Controls.Add(new Label { Text = "Ruta:", AutoSize = true, Margin = new Padding(3, 8, 0, 0) });
        barra.Controls.Add(cmbRuta);
        barra.Controls.Add(lblResumen);
        pagina.Controls.Add(grilla);
        pagina.Controls.Add(barra);
        return pagina;
    }

    // =====================================================================
    // 4. ESTADÍSTICAS (GROUP BY, COUNT, SUM, HAVING)
    // =====================================================================
    private TabPage PestanaEstadisticas()
    {
        var pagina = new TabPage("Estadísticas");
        var cmbTipo = Ui.Combo(); cmbTipo.Width = 300;
        cmbTipo.Items.AddRange(EstadisticaServicio.Disponibles);
        var numN = new NumericUpDown { Minimum = 0, Maximum = 100, Value = 1, Width = 60, Enabled = false };
        var grilla = Ui.Grilla();
        Ui.Columnas(grilla, ("Etiqueta", "Grupo"), ("Valor", "Cantidad"), ("Detalle", "Detalle"));
        var grafico = new GraficoBarras { Dock = DockStyle.Fill };

        void Calcular()
        {
            if (cmbTipo.SelectedIndex < 0) return;
            numN.Enabled = cmbTipo.SelectedIndex == 2;
            Ui.Intentar(() =>
            {
                var datos = _estadisticas.Obtener(cmbTipo.SelectedIndex, (int)numN.Value);
                Ui.Mostrar(grilla, datos);
                grafico.Mostrar(datos);
            });
        }
        cmbTipo.SelectedIndexChanged += (_, _) => Calcular();
        numN.ValueChanged += (_, _) => Calcular();
        _alMostrar[pagina] = () => { if (cmbTipo.SelectedIndex < 0) cmbTipo.SelectedIndex = 0; };

        var barra = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(4) };
        barra.Controls.Add(new Label { Text = "Estadística:", AutoSize = true, Margin = new Padding(3, 8, 0, 0) });
        barra.Controls.Add(cmbTipo);
        barra.Controls.Add(new Label { Text = "N (artistas con más de N obras):", AutoSize = true, Margin = new Padding(20, 8, 0, 0) });
        barra.Controls.Add(numN);

        pagina.Controls.Add(DosColumnas(grilla, grafico, 45));
        pagina.Controls.Add(barra);
        return pagina;
    }

    private static TableLayoutPanel DosColumnas(Control izquierda, Control derecha, float porcentajeIzquierda)
    {
        var t = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(4) };
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, porcentajeIzquierda));
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100 - porcentajeIzquierda));
        izquierda.Dock = DockStyle.Fill; derecha.Dock = DockStyle.Fill;
        t.Controls.Add(izquierda, 0, 0);
        t.Controls.Add(derecha, 1, 0);
        return t;
    }
}
