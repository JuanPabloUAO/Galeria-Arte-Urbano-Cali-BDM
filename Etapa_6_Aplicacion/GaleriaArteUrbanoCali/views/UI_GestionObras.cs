using System;
using System.Drawing;
using System.Windows.Forms;
using GaleriaArteUrbanoCali.controllers;
using GaleriaArteUrbanoCali.models;

namespace GaleriaArteUrbanoCali.views
{
    public class UI_GestionObras : Form
    {
        private readonly ControllerObra _ctrlObra = new ControllerObra();
        private readonly ControllerArtista _ctrlArtista = new ControllerArtista();
        private readonly ControllerCategoria _ctrlCategoria = new ControllerCategoria();
        private readonly ControllerUbicacion _ctrlUbicacion = new ControllerUbicacion();
        private readonly bool _soloLectura;

        private TextBox txtIdObra = new TextBox();
        private TextBox txtNombreObra = new TextBox();
        private DateTimePicker dtpFechaCreacion = new DateTimePicker();
        private ComboBox cboEstadoConservacion = new ComboBox();
        private ComboBox cboArtista = new ComboBox();
        private ComboBox cboCategoria = new ComboBox();
        private ComboBox cboUbicacion = new ComboBox();
        private TextBox txtDescripcion = new TextBox();
        private DataGridView dgvObras = new DataGridView();

        public UI_GestionObras(bool soloLectura = false)
        {
            _soloLectura = soloLectura;
            ConfigurarInterfaz();
            CargarCombosForaneos();
            RefrescarTabla();
        }

        private void ConfigurarInterfaz()
        {
            Text = "Gestión de Obras de Arte Urbano de Cali (Selección de Artista, Categoría y Ubicación)";
            Size = new Size(1060, 640);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9F);

            GroupBox grp = new GroupBox
            {
                Text = "Ficha Técnica de la Obra (Regla 1: Toda obra debe tener Artista, Categoría y Ubicación registrados)",
                Location = new Point(16, 12),
                Size = new Size(1010, 235),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };

            grp.Controls.Add(new Label { Text = "ID Obra (UUID):", Location = new Point(16, 28), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtIdObra = new TextBox { Location = new Point(16, 48), Width = 240, ReadOnly = true, BackColor = Color.WhiteSmoke, Font = new Font("Segoe UI", 9F) };

            grp.Controls.Add(new Label { Text = "Título de la Obra*:", Location = new Point(270, 28), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtNombreObra = new TextBox { Location = new Point(270, 48), Width = 360, Font = new Font("Segoe UI", 9F) };

            grp.Controls.Add(new Label { Text = "Fecha Creación*:", Location = new Point(645, 28), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            dtpFechaCreacion = new DateTimePicker { Location = new Point(645, 48), Width = 145, Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 9F) };

            grp.Controls.Add(new Label { Text = "Estado Conservación*:", Location = new Point(805, 28), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            cboEstadoConservacion = new ComboBox { Location = new Point(805, 48), Width = 185, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F) };
            cboEstadoConservacion.Items.AddRange(new object[] { "Excelente", "Bueno", "Regular", "Deteriorado", "Destruido" });
            cboEstadoConservacion.SelectedIndex = 0;

            grp.Controls.Add(new Label { Text = "Artista Autor (FK)*:", Location = new Point(16, 84), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            cboArtista = new ComboBox { Location = new Point(16, 104), Width = 280, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F) };

            grp.Controls.Add(new Label { Text = "Categoría Técnica (FK)*:", Location = new Point(310, 84), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            cboCategoria = new ComboBox { Location = new Point(310, 104), Width = 290, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F) };

            grp.Controls.Add(new Label { Text = "Ubicación Física en Cali (FK)*:", Location = new Point(615, 84), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            cboUbicacion = new ComboBox { Location = new Point(615, 104), Width = 375, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F) };

            grp.Controls.Add(new Label { Text = "Descripción Conceptual e Histórica:", Location = new Point(16, 138), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtDescripcion = new TextBox { Location = new Point(16, 158), Width = 580, Height = 60, Multiline = true, ScrollBars = ScrollBars.Vertical, Font = new Font("Segoe UI", 9F) };

            Button btnRegistrar = new Button
            {
                Text = "Registrar Nueva Obra",
                Location = new Point(615, 158),
                Size = new Size(180, 34),
                BackColor = Color.FromArgb(24, 90, 157),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = !_soloLectura
            };
            btnRegistrar.Click += BtnRegistrar_Click;

            Button btnModificar = new Button
            {
                Text = "Modificar Obra",
                Location = new Point(808, 158),
                Size = new Size(180, 34),
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = !_soloLectura
            };
            btnModificar.Click += BtnModificar_Click;

            Button btnLimpiar = new Button
            {
                Text = "Limpiar Campos",
                Location = new Point(615, 196),
                Size = new Size(373, 26),
                FlatStyle = FlatStyle.Flat
            };
            btnLimpiar.Click += (s, e) =>
            {
                txtIdObra.Clear();
                txtNombreObra.Clear();
                txtDescripcion.Clear();
                dtpFechaCreacion.Value = DateTime.Today;
                cboEstadoConservacion.SelectedIndex = 0;
            };

            grp.Controls.AddRange(new Control[]
            {
                txtIdObra, txtNombreObra, dtpFechaCreacion, cboEstadoConservacion,
                cboArtista, cboCategoria, cboUbicacion, txtDescripcion,
                btnRegistrar, btnModificar, btnLimpiar
            });
            Controls.Add(grp);

            dgvObras = new DataGridView
            {
                Location = new Point(16, 258),
                Size = new Size(1010, 325),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            dgvObras.SelectionChanged += DgvObras_SelectionChanged;
            Controls.Add(dgvObras);
        }

        private void CargarCombosForaneos()
        {
            cboArtista.DataSource = _ctrlArtista.ListarArtistasActivos();
            cboArtista.DisplayMember = "NombreArtistico";
            cboArtista.ValueMember = "IdArtista";

            cboCategoria.DataSource = _ctrlCategoria.ListarCategoriasActivas();
            cboCategoria.DisplayMember = "NombreCategoria";
            cboCategoria.ValueMember = "IdCategoria";

            cboUbicacion.DataSource = _ctrlUbicacion.ListarUbicaciones();
            cboUbicacion.DisplayMember = "EtiquetaCompleta";
            cboUbicacion.ValueMember = "IdUbicacion";
        }

        private void RefrescarTabla()
        {
            dgvObras.DataSource = _ctrlObra.ConsultarCatalogoObras();
        }

        private void DgvObras_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvObras.CurrentRow == null) return;
            string? id = dgvObras.CurrentRow.Cells["ID"]?.Value?.ToString();
            if (string.IsNullOrWhiteSpace(id)) return;

            Obra? o = _ctrlObra.BuscarObra(id);
            if (o != null)
            {
                txtIdObra.Text = o.IdObra;
                txtNombreObra.Text = o.NombreObra;
                txtDescripcion.Text = o.DescripcionObra ?? string.Empty;
                dtpFechaCreacion.Value = o.FechaCreacion;
                cboEstadoConservacion.SelectedItem = o.EstadoConservacion;
                cboArtista.SelectedValue = o.IdArtistaFK;
                cboCategoria.SelectedValue = o.IdCategoriaFK;
                cboUbicacion.SelectedValue = o.IdUbicacionFK;
            }
        }

        private void BtnRegistrar_Click(object? sender, EventArgs e)
        {
            string idUsr = ControllerUsuario.SesionActual?.IdUsuario ?? "90000000-0000-4000-8000-000000000001";

            var o = new Obra
            {
                NombreObra = txtNombreObra.Text,
                DescripcionObra = txtDescripcion.Text,
                FechaCreacion = dtpFechaCreacion.Value,
                EstadoConservacion = cboEstadoConservacion.SelectedItem?.ToString() ?? "Excelente",
                IdArtistaFK = cboArtista.SelectedValue?.ToString() ?? string.Empty,
                IdCategoriaFK = cboCategoria.SelectedValue?.ToString() ?? string.Empty,
                IdUbicacionFK = cboUbicacion.SelectedValue?.ToString() ?? string.Empty,
                IdUsuarioFK = idUsr
            };

            if (_ctrlObra.InsertarObra(o, out string msg))
            {
                MessageBox.Show("Obra de arte urbano registrada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefrescarTabla();
            }
            else
            {
                MessageBox.Show("No se pudo registrar la obra:\n" + msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnModificar_Click(object? sender, EventArgs e)
        {
            var o = new Obra
            {
                IdObra = txtIdObra.Text,
                NombreObra = txtNombreObra.Text,
                DescripcionObra = txtDescripcion.Text,
                FechaCreacion = dtpFechaCreacion.Value,
                EstadoConservacion = cboEstadoConservacion.SelectedItem?.ToString() ?? "Excelente",
                IdArtistaFK = cboArtista.SelectedValue?.ToString() ?? string.Empty,
                IdCategoriaFK = cboCategoria.SelectedValue?.ToString() ?? string.Empty,
                IdUbicacionFK = cboUbicacion.SelectedValue?.ToString() ?? string.Empty
            };

            if (_ctrlObra.ActualizarObra(o, out string msg))
            {
                MessageBox.Show("Obra actualizada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefrescarTabla();
            }
            else
            {
                MessageBox.Show("No se pudo actualizar la obra:\n" + msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
