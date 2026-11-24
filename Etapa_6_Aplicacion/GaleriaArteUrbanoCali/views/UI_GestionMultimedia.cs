using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GaleriaArteUrbanoCali.controllers;
using GaleriaArteUrbanoCali.models;

namespace GaleriaArteUrbanoCali.views
{
    public class UI_GestionMultimedia : Form
    {
        private readonly ControllerRecursoMultimedia _ctrlMultimedia = new ControllerRecursoMultimedia();
        private readonly ControllerObra _ctrlObra = new ControllerObra();
        private readonly bool _soloLectura;

        private ComboBox cboObra = new ComboBox();
        private ComboBox cboTipoRecurso = new ComboBox();
        private ComboBox cboFormato = new ComboBox();
        private TextBox txtNombreRecurso = new TextBox();
        private TextBox txtRutaOrigenPC = new TextBox();
        private TextBox txtDuracionSeg = new TextBox();
        private TextBox txtDescripcion = new TextBox();
        private DataGridView dgvRecursos = new DataGridView();
        private PictureBox picPreview = new PictureBox();
        private Label lblDetallePreview = new Label();

        public UI_GestionMultimedia(bool soloLectura = false)
        {
            _soloLectura = soloLectura;
            ConfigurarInterfaz();
            CargarCatalogos();
            RefrescarTabla();
        }

        private void ConfigurarInterfaz()
        {
            Text = "Gestión y Visualización de Recursos Multimedia";
            Size = new Size(1120, 660);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9F);

            GroupBox grpAdd = new GroupBox
            {
                Text = "Asociar Nuevo Archivo Multimedia a una Obra",
                Location = new Point(16, 12),
                Size = new Size(730, 230),
                Font = new Font("Segoe UI", 9.2F, FontStyle.Bold)
            };

            grpAdd.Controls.Add(new Label { Text = "Obra Asociada*:", Location = new Point(16, 26), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            cboObra = new ComboBox { Location = new Point(16, 46), Width = 340, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F) };

            grpAdd.Controls.Add(new Label { Text = "Tipo de Recurso*:", Location = new Point(370, 26), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            cboTipoRecurso = new ComboBox { Location = new Point(370, 46), Width = 185, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F) };
            cboTipoRecurso.SelectedIndexChanged += CboTipoRecurso_SelectedIndexChanged;

            grpAdd.Controls.Add(new Label { Text = "Formato*:", Location = new Point(568, 26), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            cboFormato = new ComboBox { Location = new Point(568, 46), Width = 145, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F) };

            grpAdd.Controls.Add(new Label { Text = "Seleccionar Archivo del Computador*:", Location = new Point(16, 80), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtRutaOrigenPC = new TextBox { Location = new Point(16, 100), Width = 540, ReadOnly = true, BackColor = Color.WhiteSmoke, Font = new Font("Segoe UI", 9F) };

            Button btnExaminar = new Button
            {
                Text = "Examinar PC...",
                Location = new Point(568, 98),
                Size = new Size(145, 28),
                BackColor = Color.FromArgb(215, 232, 248),
                FlatStyle = FlatStyle.Flat,
                Enabled = !_soloLectura
            };
            btnExaminar.Click += BtnExaminar_Click;

            grpAdd.Controls.Add(new Label { Text = "Nombre Descriptivo*:", Location = new Point(16, 134), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtNombreRecurso = new TextBox { Location = new Point(16, 154), Width = 250, Font = new Font("Segoe UI", 9F) };

            grpAdd.Controls.Add(new Label { Text = "Duración (seg, opcional):", Location = new Point(280, 134), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtDuracionSeg = new TextBox { Location = new Point(280, 154), Width = 120, Font = new Font("Segoe UI", 9F) };

            grpAdd.Controls.Add(new Label { Text = "Descripción:", Location = new Point(415, 134), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtDescripcion = new TextBox { Location = new Point(415, 154), Width = 298, Font = new Font("Segoe UI", 9F) };

            Button btnRegistrar = new Button
            {
                Text = "Copiar y Registrar en MySQL",
                Location = new Point(16, 188),
                Size = new Size(235, 32),
                BackColor = Color.FromArgb(24, 90, 157),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = !_soloLectura
            };
            btnRegistrar.Click += BtnRegistrar_Click;

            Button btnFiltrarObra = new Button
            {
                Text = "Filtrar por Obra",
                Location = new Point(258, 188),
                Size = new Size(145, 32),
                FlatStyle = FlatStyle.Flat
            };
            btnFiltrarObra.Click += (s, e) =>
            {
                string? idObra = cboObra.SelectedValue?.ToString();
                dgvRecursos.DataSource = _ctrlMultimedia.ConsultarRecursosPorObra(idObra);
            };

            Button btnVerTodos = new Button
            {
                Text = "Ver Todos",
                Location = new Point(410, 188),
                Size = new Size(135, 32),
                FlatStyle = FlatStyle.Flat
            };
            btnVerTodos.Click += (s, e) => RefrescarTabla();

            Button btnEliminarRecurso = new Button
            {
                Text = "Eliminar Recurso",
                Location = new Point(552, 188),
                Size = new Size(160, 32),
                BackColor = Color.FromArgb(183, 28, 28),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = !_soloLectura
            };
            btnEliminarRecurso.Click += BtnEliminarRecurso_Click;

            grpAdd.Controls.AddRange(new Control[]
            {
                cboObra, cboTipoRecurso, cboFormato, txtRutaOrigenPC, btnExaminar,
                txtNombreRecurso, txtDuracionSeg, txtDescripcion,
                btnRegistrar, btnFiltrarObra, btnVerTodos, btnEliminarRecurso
            });
            Controls.Add(grpAdd);

            GroupBox grpPreview = new GroupBox
            {
                Text = "Previsualización / Reproducción Multimedia",
                Location = new Point(758, 12),
                Size = new Size(330, 230),
                Font = new Font("Segoe UI", 9.2F, FontStyle.Bold)
            };

            picPreview = new PictureBox
            {
                Location = new Point(14, 24),
                Size = new Size(300, 145),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Black
            };

            lblDetallePreview = new Label
            {
                Text = "Seleccione un recurso en la tabla inferior",
                Location = new Point(14, 173),
                Size = new Size(300, 20),
                Font = new Font("Segoe UI", 8F, FontStyle.Regular)
            };

            Button btnAbrirReproducir = new Button
            {
                Text = "Abrir / Reproducir Archivo en Visor del Sistema",
                Location = new Point(14, 194),
                Size = new Size(300, 28),
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAbrirReproducir.Click += BtnAbrirReproducir_Click;

            grpPreview.Controls.AddRange(new Control[] { picPreview, lblDetallePreview, btnAbrirReproducir });
            Controls.Add(grpPreview);

            dgvRecursos = new DataGridView
            {
                Location = new Point(16, 252),
                Size = new Size(1072, 355),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            dgvRecursos.SelectionChanged += DgvRecursos_SelectionChanged;
            Controls.Add(dgvRecursos);
        }

        private void CargarCatalogos()
        {
            cboObra.DataSource = _ctrlObra.ListarObras();
            cboObra.DisplayMember = "EtiquetaObra";
            cboObra.ValueMember = "IdObra";

            cboTipoRecurso.DataSource = _ctrlMultimedia.ListarTiposRecurso();
            cboTipoRecurso.DisplayMember = "EtiquetaTipo";
            cboTipoRecurso.ValueMember = "IdTipoRecurso";
        }

        private void CboTipoRecurso_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cboTipoRecurso.SelectedValue is string idTipo)
            {
                cboFormato.DataSource = _ctrlMultimedia.ListarFormatosPorTipo(idTipo);
                cboFormato.DisplayMember = "Extension";
                cboFormato.ValueMember = "IdFormato";
            }
        }

        private void RefrescarTabla()
        {
            dgvRecursos.DataSource = _ctrlMultimedia.ConsultarRecursosPorObra(null);
            if (dgvRecursos.Columns["CarpetaBase"] is DataGridViewColumn colCarpeta)
                colCarpeta.Visible = false;
            if (dgvRecursos.Columns["ArchivoRelativo"] is DataGridViewColumn colArchivo)
                colArchivo.Visible = false;
        }

        private void BtnExaminar_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Seleccionar archivo multimedia para la Galería de Arte Urbano",
                Filter = "Archivos Multimedia (*.jpg;*.jpeg;*.png;*.webp;*.mp4;*.mp3;*.wav;*.glb;*.usdz)|*.jpg;*.jpeg;*.png;*.webp;*.mp4;*.mp3;*.wav;*.glb;*.usdz|Todos los archivos (*.*)|*.*"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtRutaOrigenPC.Text = ofd.FileName;
                if (string.IsNullOrWhiteSpace(txtNombreRecurso.Text))
                {
                    txtNombreRecurso.Text = Path.GetFileNameWithoutExtension(ofd.FileName);
                }
            }
        }

        private void BtnRegistrar_Click(object? sender, EventArgs e)
        {
            if (cboFormato.SelectedItem is not FormatoRecurso fmt)
            {
                MessageBox.Show("Seleccione un formato válido.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int? dur = null;
            if (int.TryParse(txtDuracionSeg.Text.Trim(), out int dVal) && dVal > 0)
            {
                dur = dVal;
            }

            var rm = new RecursoMultimedia
            {
                NombreRecurso = txtNombreRecurso.Text,
                DescripcionRecurso = txtDescripcion.Text,
                Duracion = dur,
                IdFormatoFK = fmt.IdFormato,
                IdObraFK = cboObra.SelectedValue?.ToString() ?? string.Empty
            };

            if (_ctrlMultimedia.CopiarYRegistrarArchivoMultimedia(txtRutaOrigenPC.Text, fmt, rm, out string msg))
            {
                MessageBox.Show(
                    $"Recurso multimedia registrado exitosamente.\nRuta relativa guardada en MySQL:\n{fmt.CarpetaBase}{rm.RutaArchivo}",
                    "Registro Exitoso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                txtRutaOrigenPC.Clear();
                txtNombreRecurso.Clear();
                txtDescripcion.Clear();
                txtDuracionSeg.Clear();
                RefrescarTabla();
            }
            else
            {
                MessageBox.Show("No se pudo registrar el recurso:\n" + msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnEliminarRecurso_Click(object? sender, EventArgs e)
        {
            if (dgvRecursos.CurrentRow == null) return;
            string? idRec = dgvRecursos.CurrentRow.Cells["ID"]?.Value?.ToString();
            string? nomRec = dgvRecursos.CurrentRow.Cells["Nombre del Recurso"]?.Value?.ToString();
            if (string.IsNullOrWhiteSpace(idRec)) return;

            DialogResult confirm = MessageBox.Show(
                $"¿Está seguro de eliminar el registro multimedia '{nomRec}'?",
                "Confirmar Eliminación de Recurso Multimedia",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            if (_ctrlMultimedia.EliminarRecurso(idRec, out string msg))
            {
                MessageBox.Show("Recurso multimedia eliminado correctamente.", "Eliminación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefrescarTabla();
            }
            else
            {
                MessageBox.Show("Error al eliminar recurso:\n" + msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvRecursos_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvRecursos.CurrentRow == null) return;
            string carpeta = dgvRecursos.CurrentRow.Cells["CarpetaBase"]?.Value?.ToString() ?? string.Empty;
            string archivo = dgvRecursos.CurrentRow.Cells["ArchivoRelativo"]?.Value?.ToString() ?? string.Empty;
            string tipo = dgvRecursos.CurrentRow.Cells["Tipo"]?.Value?.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(archivo)) return;

            string rutaFisica = _ctrlMultimedia.ResolverRutaFisica(carpeta, archivo);
            lblDetallePreview.Text = $"{tipo}: {carpeta}{archivo}";

            if (picPreview.Image != null)
            {
                var old = picPreview.Image;
                picPreview.Image = null;
                old.Dispose();
            }

            if (File.Exists(rutaFisica) && string.Equals(tipo, "IMAGEN", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    using var fs = new FileStream(rutaFisica, FileMode.Open, FileAccess.Read);
                    picPreview.Image = Image.FromStream(fs);
                }
                catch
                {
                    picPreview.Image = null;
                }
            }
        }

        private void BtnAbrirReproducir_Click(object? sender, EventArgs e)
        {
            if (dgvRecursos.CurrentRow == null) return;
            string carpeta = dgvRecursos.CurrentRow.Cells["CarpetaBase"]?.Value?.ToString() ?? string.Empty;
            string archivo = dgvRecursos.CurrentRow.Cells["ArchivoRelativo"]?.Value?.ToString() ?? string.Empty;
            string rutaFisica = _ctrlMultimedia.ResolverRutaFisica(carpeta, archivo);

            if (!File.Exists(rutaFisica))
            {
                MessageBox.Show($"El archivo físico aún no se encuentra en:\n{rutaFisica}", "Archivo en File System", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo(rutaFisica) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir el archivo: " + ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
