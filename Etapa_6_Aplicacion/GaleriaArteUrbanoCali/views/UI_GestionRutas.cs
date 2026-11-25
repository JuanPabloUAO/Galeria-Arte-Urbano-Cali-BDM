using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using GaleriaArteUrbanoCali.controllers;
using GaleriaArteUrbanoCali.models;

namespace GaleriaArteUrbanoCali.views
{
    public class UI_GestionRutas : Form
    {
        private readonly ControllerRuta _ctrlRuta = new ControllerRuta();
        private readonly ControllerObra _ctrlObra = new ControllerObra();
        private readonly bool _soloLectura;

        private TextBox txtNombreRuta = new TextBox();
        private TextBox txtDuracionMin = new TextBox();
        private TextBox txtDistanciaKm = new TextBox();
        private TextBox txtDescripcionRuta = new TextBox();

        private ComboBox cboRutaSeleccionada = new ComboBox();
        private ComboBox cboObraAsociar = new ComboBox();
        private NumericUpDown nudOrdenVisita = new NumericUpDown();
        private NumericUpDown nudTiempoPermanencia = new NumericUpDown();
        private TextBox txtRecomendacionParada = new TextBox();
        private DataGridView dgvParadasRuta = new DataGridView();

        public UI_GestionRutas(bool soloLectura = false)
        {
            _soloLectura = soloLectura;
            ConfigurarInterfaz();
            CargarCatalogos();
        }

        private void ConfigurarInterfaz()
        {
            Text = "Gestión de Rutas y Orden de Recorrido";
            Size = new Size(1060, 650);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9F);

            GroupBox grpNuevaRuta = new GroupBox
            {
                Text = "1. Crear Nueva Ruta de Arte Urbano",
                Location = new Point(16, 12),
                Size = new Size(1010, 115),
                Font = new Font("Segoe UI", 9.2F, FontStyle.Bold)
            };

            grpNuevaRuta.Controls.Add(new Label { Text = "Nombre de la Ruta*:", Location = new Point(16, 26), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtNombreRuta = new TextBox { Location = new Point(16, 46), Width = 310, Font = new Font("Segoe UI", 9F) };

            grpNuevaRuta.Controls.Add(new Label { Text = "Duración (min)*:", Location = new Point(340, 26), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtDuracionMin = new TextBox { Location = new Point(340, 46), Width = 95, Text = "120", Font = new Font("Segoe UI", 9F) };

            grpNuevaRuta.Controls.Add(new Label { Text = "Distancia (km)*:", Location = new Point(450, 26), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtDistanciaKm = new TextBox { Location = new Point(450, 46), Width = 95, Text = "3.0", Font = new Font("Segoe UI", 9F) };

            grpNuevaRuta.Controls.Add(new Label { Text = "Descripción del Recorrido:", Location = new Point(560, 26), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtDescripcionRuta = new TextBox { Location = new Point(560, 46), Width = 285, Font = new Font("Segoe UI", 9F) };

            Button btnCrearRuta = new Button
            {
                Text = "Crear Ruta",
                Location = new Point(858, 42),
                Size = new Size(135, 34),
                BackColor = Color.FromArgb(24, 90, 157),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = !_soloLectura
            };
            btnCrearRuta.Click += BtnCrearRuta_Click;

            grpNuevaRuta.Controls.AddRange(new Control[] { txtNombreRuta, txtDuracionMin, txtDistanciaKm, txtDescripcionRuta, btnCrearRuta });
            Controls.Add(grpNuevaRuta);

            GroupBox grpParadas = new GroupBox
            {
                Text = "2. Configurar Obras y Orden de Recorrido en la Ruta",
                Location = new Point(16, 136),
                Size = new Size(1010, 138),
                Font = new Font("Segoe UI", 9.2F, FontStyle.Bold)
            };

            grpParadas.Controls.Add(new Label { Text = "Seleccionar Ruta*:", Location = new Point(16, 26), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            cboRutaSeleccionada = new ComboBox { Location = new Point(16, 46), Width = 410, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F) };
            cboRutaSeleccionada.SelectedIndexChanged += (s, e) => RefrescarParadasRuta();

            grpParadas.Controls.Add(new Label { Text = "Obra a Agregar*:", Location = new Point(440, 26), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            cboObraAsociar = new ComboBox { Location = new Point(440, 46), Width = 335, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F) };

            grpParadas.Controls.Add(new Label { Text = "Orden Visita*:", Location = new Point(790, 26), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            nudOrdenVisita = new NumericUpDown { Location = new Point(790, 46), Width = 85, Minimum = 1, Maximum = 99, Value = 5, Font = new Font("Segoe UI", 9F) };

            grpParadas.Controls.Add(new Label { Text = "Minutos*:", Location = new Point(890, 26), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            nudTiempoPermanencia = new NumericUpDown { Location = new Point(890, 46), Width = 100, Minimum = 5, Maximum = 240, Value = 25, Font = new Font("Segoe UI", 9F) };

            grpParadas.Controls.Add(new Label { Text = "Recomendación:", Location = new Point(16, 84), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtRecomendacionParada = new TextBox { Location = new Point(125, 82), Width = 365, Font = new Font("Segoe UI", 9F) };

            Button btnAgregarParada = new Button
            {
                Text = "Agregar Obra",
                Location = new Point(500, 78),
                Size = new Size(155, 34),
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = !_soloLectura
            };
            btnAgregarParada.Click += BtnAgregarParada_Click;

            Button btnActualizarOrden = new Button
            {
                Text = "Cambiar Orden Parada",
                Location = new Point(662, 78),
                Size = new Size(165, 34),
                BackColor = Color.FromArgb(24, 90, 157),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = !_soloLectura
            };
            btnActualizarOrden.Click += BtnActualizarOrden_Click;

            Button btnRetirarParada = new Button
            {
                Text = "Retirar Parada",
                Location = new Point(834, 78),
                Size = new Size(160, 34),
                BackColor = Color.FromArgb(183, 28, 28),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = !_soloLectura
            };
            btnRetirarParada.Click += BtnRetirarParada_Click;

            grpParadas.Controls.AddRange(new Control[]
            {
                cboRutaSeleccionada, cboObraAsociar, nudOrdenVisita, nudTiempoPermanencia,
                txtRecomendacionParada, btnAgregarParada, btnActualizarOrden, btnRetirarParada
            });
            Controls.Add(grpParadas);

            dgvParadasRuta = new DataGridView
            {
                Location = new Point(16, 284),
                Size = new Size(1010, 315),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            dgvParadasRuta.SelectionChanged += DgvParadasRuta_SelectionChanged;
            Controls.Add(dgvParadasRuta);
        }

        private void CargarCatalogos()
        {
            cboObraAsociar.DataSource = _ctrlObra.ListarObras();
            cboObraAsociar.DisplayMember = "EtiquetaObra";
            cboObraAsociar.ValueMember = "IdObra";

            RecargarComboRutas();
        }

        private void RecargarComboRutas()
        {
            cboRutaSeleccionada.DataSource = _ctrlRuta.ListarRutas();
            cboRutaSeleccionada.DisplayMember = "EtiquetaRuta";
            cboRutaSeleccionada.ValueMember = "IdRuta";
            RefrescarParadasRuta();
        }

        private void RefrescarParadasRuta()
        {
            if (cboRutaSeleccionada.SelectedValue is string idRuta)
            {
                dgvParadasRuta.DataSource = _ctrlRuta.ConsultarParadasPorRuta(idRuta);
            }
        }

        private void DgvParadasRuta_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvParadasRuta.CurrentRow == null) return;
            if (int.TryParse(dgvParadasRuta.CurrentRow.Cells["Orden Visita"]?.Value?.ToString(), out int orden) && orden >= 1 && orden <= 99)
            {
                nudOrdenVisita.Value = orden;
            }
            if (int.TryParse(dgvParadasRuta.CurrentRow.Cells["Permanencia (min)"]?.Value?.ToString(), out int min) && min >= 5 && min <= 240)
            {
                nudTiempoPermanencia.Value = min;
            }
            txtRecomendacionParada.Text = dgvParadasRuta.CurrentRow.Cells["Recomendación de Visita"]?.Value?.ToString() ?? string.Empty;
        }

        private void BtnCrearRuta_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtDuracionMin.Text.Trim(), out int dur) ||
                !decimal.TryParse(txtDistanciaKm.Text.Trim().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal dist))
            {
                MessageBox.Show("Ingrese valores numéricos válidos para duración y distancia.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var r = new Ruta
            {
                NombreRuta = txtNombreRuta.Text,
                DescripcionRuta = txtDescripcionRuta.Text,
                DuracionEstimada = dur,
                DistanciaEstimada = dist,
                EstadoRuta = true
            };

            if (_ctrlRuta.CrearRuta(r, out string msg))
            {
                MessageBox.Show("Ruta creada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtNombreRuta.Clear();
                txtDescripcionRuta.Clear();
                RecargarComboRutas();
            }
            else
            {
                MessageBox.Show("No se pudo crear la ruta:\n" + msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnAgregarParada_Click(object? sender, EventArgs e)
        {
            var ro = new RutaObra
            {
                IdRutaFK = cboRutaSeleccionada.SelectedValue?.ToString() ?? string.Empty,
                IdObraFK = cboObraAsociar.SelectedValue?.ToString() ?? string.Empty,
                OrdenVisita = (int)nudOrdenVisita.Value,
                TiempoPermanencia = (int)nudTiempoPermanencia.Value,
                Recomendacion = txtRecomendacionParada.Text
            };

            if (_ctrlRuta.AgregarObraARuta(ro, out string msg))
            {
                MessageBox.Show("Obra agregada al itinerario de la ruta.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtRecomendacionParada.Clear();
                nudOrdenVisita.Value = Math.Min(99, nudOrdenVisita.Value + 1);
                RefrescarParadasRuta();
            }
            else
            {
                MessageBox.Show("No se pudo agregar la parada:\n" + msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnActualizarOrden_Click(object? sender, EventArgs e)
        {
            if (dgvParadasRuta.CurrentRow == null) return;
            string? idParada = dgvParadasRuta.CurrentRow.Cells["ID Parada"]?.Value?.ToString();
            string idRuta = cboRutaSeleccionada.SelectedValue?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(idParada)) return;

            if (_ctrlRuta.ActualizarOrdenParada(idParada, idRuta, (int)nudOrdenVisita.Value, (int)nudTiempoPermanencia.Value, txtRecomendacionParada.Text, out string msg))
            {
                MessageBox.Show("Orden y datos de la parada actualizados correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefrescarParadasRuta();
            }
            else
            {
                MessageBox.Show("No se pudo actualizar la parada:\n" + msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnRetirarParada_Click(object? sender, EventArgs e)
        {
            if (dgvParadasRuta.CurrentRow == null) return;
            string? idParada = dgvParadasRuta.CurrentRow.Cells["ID Parada"]?.Value?.ToString();
            string? obra = dgvParadasRuta.CurrentRow.Cells["Obra de Arte Urbano"]?.Value?.ToString();
            if (string.IsNullOrWhiteSpace(idParada)) return;

            DialogResult confirm = MessageBox.Show(
                $"¿Está seguro de retirar la obra '{obra}' de esta ruta?",
                "Confirmar Retiro de Obra",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            if (_ctrlRuta.RetirarParadaDeRuta(idParada, out string msg))
            {
                RefrescarParadasRuta();
            }
            else
            {
                MessageBox.Show("Error al retirar parada:\n" + msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
