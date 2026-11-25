using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GaleriaArteUrbanoCali.controllers;
using GaleriaArteUrbanoCali.models;

namespace GaleriaArteUrbanoCali.views
{
    public class UI_ConsultasYEstadisticas : Form
    {
        private readonly ControllerConsultas _ctrlConsultas = new ControllerConsultas();
        private readonly ControllerArtista _ctrlArtista = new ControllerArtista();
        private readonly ControllerCategoria _ctrlCategoria = new ControllerCategoria();
        private readonly ControllerUbicacion _ctrlUbicacion = new ControllerUbicacion();
        private readonly ControllerRuta _ctrlRuta = new ControllerRuta();
        private readonly ControllerObra _ctrlObra = new ControllerObra();
        private readonly ControllerRecursoMultimedia _ctrlMultimedia = new ControllerRecursoMultimedia();

        private ComboBox cboArtista = new ComboBox();
        private ComboBox cboCategoria = new ComboBox();
        private ComboBox cboComuna = new ComboBox();
        private ComboBox cboBarrio = new ComboBox();
        private ComboBox cboRuta = new ComboBox();
        private ComboBox cboObraMultimedia = new ComboBox();
        private Label lblTituloReporte = new Label();
        private DataGridView dgvResultados = new DataGridView();

        public UI_ConsultasYEstadisticas()
        {
            ConfigurarInterfaz();
            CargarCombos();
            EjecutarFichaCompleta();
        }

        private void ConfigurarInterfaz()
        {
            Text = "Consultas Multitabla (JOIN) y Estadísticas Culturales";
            Size = new Size(1160, 680);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9F);

            GroupBox grpFiltros = new GroupBox
            {
                Text = "Consultas Multitabla con JOIN, Vistas y Estadísticas Agregadas (GROUP BY, COUNT, HAVING)",
                Location = new Point(16, 12),
                Size = new Size(1110, 178),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };

            // 1. Consulta completa de obra
            Button btnQ1 = new Button
            {
                Text = "1. Ficha Completa de Obras",
                Location = new Point(16, 28),
                Size = new Size(235, 34),
                BackColor = Color.FromArgb(24, 90, 157),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnQ1.Click += (s, e) => EjecutarFichaCompleta();

            // 2. Obras por artista
            cboArtista = new ComboBox { Location = new Point(265, 31), Width = 185, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F) };
            Button btnQ2 = new Button
            {
                Text = "2. Obras por Artista",
                Location = new Point(456, 28),
                Size = new Size(155, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(215, 232, 248)
            };
            btnQ2.Click += (s, e) =>
            {
                string id = cboArtista.SelectedValue?.ToString() ?? string.Empty;
                lblTituloReporte.Text = $"Consulta 2: Obras del Artista '{cboArtista.Text}'";
                dgvResultados.DataSource = _ctrlConsultas.ObtenerObrasPorArtista(id);
            };

            // 3. Obras por categoría
            cboCategoria = new ComboBox { Location = new Point(625, 31), Width = 225, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F) };
            Button btnQ3 = new Button
            {
                Text = "3. Obras por Categoría",
                Location = new Point(856, 28),
                Size = new Size(165, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(215, 232, 248)
            };
            btnQ3.Click += (s, e) =>
            {
                string id = cboCategoria.SelectedValue?.ToString() ?? string.Empty;
                lblTituloReporte.Text = $"Consulta 3: Obras en la Categoría '{cboCategoria.Text}'";
                dgvResultados.DataSource = _ctrlConsultas.ObtenerObrasPorCategoria(id);
            };

            // 4. Obras por ubicación (comuna / barrio)
            cboComuna = new ComboBox { Location = new Point(16, 78), Width = 145, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F) };
            cboComuna.SelectedIndexChanged += CboComuna_SelectedIndexChanged;

            cboBarrio = new ComboBox { Location = new Point(168, 78), Width = 155, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F) };

            Button btnQ4 = new Button
            {
                Text = "4. Obras por Ubicación",
                Location = new Point(330, 74),
                Size = new Size(170, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(215, 232, 248)
            };
            btnQ4.Click += (s, e) =>
            {
                string? idComuna = cboComuna.SelectedValue?.ToString();
                string? idBarrio = cboBarrio.SelectedValue?.ToString();
                lblTituloReporte.Text = $"Consulta 4: Obras por Ubicación ({cboComuna.Text} / {cboBarrio.Text})";
                dgvResultados.DataSource = _ctrlConsultas.ObtenerObrasPorComunaOBarrio(idComuna, idBarrio);
            };

            // 5. Consulta de ruta
            cboRuta = new ComboBox { Location = new Point(515, 78), Width = 335, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F) };
            Button btnQ5 = new Button
            {
                Text = "5. Itinerario de Ruta",
                Location = new Point(856, 74),
                Size = new Size(165, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(215, 232, 248)
            };
            btnQ5.Click += (s, e) =>
            {
                string id = cboRuta.SelectedValue?.ToString() ?? string.Empty;
                lblTituloReporte.Text = $"Consulta 5: Itinerario Ordenado de la Ruta '{cboRuta.Text}'";
                dgvResultados.DataSource = _ctrlConsultas.ObtenerItinerarioRuta(id);
            };

            // 6. Recursos multimedia de una obra
            cboObraMultimedia = new ComboBox { Location = new Point(16, 128), Width = 295, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F) };
            Button btnQ6 = new Button
            {
                Text = "6. Multimedia de la Obra",
                Location = new Point(318, 124),
                Size = new Size(175, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(215, 232, 248)
            };
            btnQ6.Click += (s, e) =>
            {
                string id = cboObraMultimedia.SelectedValue?.ToString() ?? string.Empty;
                lblTituloReporte.Text = $"Consulta 6: Recursos Multimedia de '{cboObraMultimedia.Text}'";
                dgvResultados.DataSource = _ctrlMultimedia.ConsultarRecursosPorObra(id);
            };

            // 7. Estadísticas con agregación (3 consultas GROUP BY, COUNT, HAVING)
            Button btnEstComuna = new Button
            {
                Text = "7A. Obras por Comuna",
                Location = new Point(510, 124),
                Size = new Size(195, 34),
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnEstComuna.Click += (s, e) =>
            {
                lblTituloReporte.Text = "Consulta 7A (GROUP BY + COUNT): Estadísticas de Obras por Comuna";
                dgvResultados.DataSource = _ctrlConsultas.ObtenerEstadisticasPorComuna();
            };

            Button btnEstCategoria = new Button
            {
                Text = "7B. Obras por Categoría",
                Location = new Point(712, 124),
                Size = new Size(185, 34),
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnEstCategoria.Click += (s, e) =>
            {
                lblTituloReporte.Text = "Consulta 7B (GROUP BY + COUNT + SUM): Obras y Recursos Multimedia por Categoría";
                dgvResultados.DataSource = _ctrlConsultas.ObtenerEstadisticasPorCategoria();
            };

            Button btnEstArtistas = new Button
            {
                Text = "7C. Artistas >= 2 Obras (HAVING)",
                Location = new Point(904, 124),
                Size = new Size(190, 34),
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnEstArtistas.Click += (s, e) =>
            {
                lblTituloReporte.Text = "Consulta 7C (GROUP BY + COUNT + HAVING >= 2): Artistas con Múltiples Obras";
                dgvResultados.DataSource = _ctrlConsultas.ObtenerArtistasMasProlificos(2);
            };

            grpFiltros.Controls.AddRange(new Control[]
            {
                btnQ1, cboArtista, btnQ2, cboCategoria, btnQ3,
                cboComuna, cboBarrio, btnQ4, cboRuta, btnQ5,
                cboObraMultimedia, btnQ6, btnEstComuna, btnEstCategoria, btnEstArtistas
            });
            Controls.Add(grpFiltros);

            lblTituloReporte = new Label
            {
                Text = "Resultados de la Consulta:",
                Location = new Point(18, 198),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(24, 58, 90)
            };
            Controls.Add(lblTituloReporte);

            dgvResultados = new DataGridView
            {
                Location = new Point(16, 224),
                Size = new Size(1110, 400),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            Controls.Add(dgvResultados);
        }

        private void CargarCombos()
        {
            cboArtista.DataSource = _ctrlArtista.ListarArtistasActivos();
            cboArtista.DisplayMember = "NombreArtistico";
            cboArtista.ValueMember = "IdArtista";

            cboCategoria.DataSource = _ctrlCategoria.ListarCategoriasActivas();
            cboCategoria.DisplayMember = "NombreCategoria";
            cboCategoria.ValueMember = "IdCategoria";

            var comunas = new List<Comuna> { new Comuna("", 0) };
            comunas.AddRange(_ctrlUbicacion.ListarComunas());
            cboComuna.DataSource = comunas;
            cboComuna.DisplayMember = "EtiquetaComuna";
            cboComuna.ValueMember = "IdComuna";

            cboRuta.DataSource = _ctrlRuta.ListarRutas();
            cboRuta.DisplayMember = "NombreRuta";
            cboRuta.ValueMember = "IdRuta";

            cboObraMultimedia.DataSource = _ctrlObra.ListarObras();
            cboObraMultimedia.DisplayMember = "NombreObra";
            cboObraMultimedia.ValueMember = "IdObra";
        }

        private void CboComuna_SelectedIndexChanged(object? sender, EventArgs e)
        {
            string idComuna = cboComuna.SelectedValue?.ToString() ?? string.Empty;
            var barrios = new List<Barrio>
            {
                new Barrio { IdBarrio = "", NombreBarrio = "-- Todos los barrios --", NumeroComuna = 0 }
            };
            barrios.AddRange(_ctrlUbicacion.ListarBarriosPorComuna(idComuna));
            cboBarrio.DataSource = barrios;
            cboBarrio.DisplayMember = "EtiquetaBarrioComuna";
            cboBarrio.ValueMember = "IdBarrio";
        }

        private void EjecutarFichaCompleta()
        {
            lblTituloReporte.Text = "Consulta 1: Ficha Completa de Obras de Arte Urbano";
            dgvResultados.DataSource = _ctrlConsultas.ObtenerFichaCompletaObras();
        }
    }
}
