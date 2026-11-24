using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using GaleriaArteUrbanoCali.controllers;
using GaleriaArteUrbanoCali.models;

namespace GaleriaArteUrbanoCali.views
{
    public class UI_GestionUbicaciones : Form
    {
        private readonly ControllerUbicacion _ctrlUbicacion = new ControllerUbicacion();
        private readonly bool _soloLectura;

        private TextBox txtIdUbicacion = new TextBox();
        private TextBox txtNombreLugar = new TextBox();
        private TextBox txtDireccion = new TextBox();
        private ComboBox cboComuna = new ComboBox();
        private ComboBox cboBarrio = new ComboBox();
        private TextBox txtLatitud = new TextBox();
        private TextBox txtLongitud = new TextBox();
        private DataGridView dgvUbicaciones = new DataGridView();

        public UI_GestionUbicaciones(bool soloLectura = false)
        {
            _soloLectura = soloLectura;
            ConfigurarInterfaz();
            CargarCatalogos();
            RefrescarTabla();
        }

        private void ConfigurarInterfaz()
        {
            Text = "Gestión de Ubicaciones en Santiago de Cali";
            Size = new Size(980, 580);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9F);

            GroupBox grp = new GroupBox
            {
                Text = "Datos de la Ubicación (Cali: Lat [3.25 a 3.55] | Lon [-76.60 a -76.45])",
                Location = new Point(16, 12),
                Size = new Size(930, 200),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };

            grp.Controls.Add(new Label { Text = "ID Ubicación:", Location = new Point(16, 28), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtIdUbicacion = new TextBox { Location = new Point(16, 48), Width = 240, ReadOnly = true, BackColor = Color.WhiteSmoke, Font = new Font("Segoe UI", 9F) };

            grp.Controls.Add(new Label { Text = "Nombre del Lugar / Sector*:", Location = new Point(270, 28), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtNombreLugar = new TextBox { Location = new Point(270, 48), Width = 320, Font = new Font("Segoe UI", 9F) };

            grp.Controls.Add(new Label { Text = "Dirección Urbana*:", Location = new Point(605, 28), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtDireccion = new TextBox { Location = new Point(605, 48), Width = 305, Font = new Font("Segoe UI", 9F) };

            grp.Controls.Add(new Label { Text = "Comuna*:", Location = new Point(16, 84), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            cboComuna = new ComboBox { Location = new Point(16, 104), Width = 175, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F) };
            cboComuna.SelectedIndexChanged += CboComuna_SelectedIndexChanged;

            grp.Controls.Add(new Label { Text = "Barrio*:", Location = new Point(205, 84), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            cboBarrio = new ComboBox { Location = new Point(205, 104), Width = 230, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F) };

            grp.Controls.Add(new Label { Text = "Latitud GPS*:", Location = new Point(450, 84), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtLatitud = new TextBox { Location = new Point(450, 104), Width = 140, Text = "3.45100000", Font = new Font("Segoe UI", 9F) };

            grp.Controls.Add(new Label { Text = "Longitud GPS*:", Location = new Point(605, 84), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtLongitud = new TextBox { Location = new Point(605, 104), Width = 140, Text = "-76.53400000", Font = new Font("Segoe UI", 9F) };

            Button btnRegistrar = new Button
            {
                Text = "Registrar Ubicación",
                Location = new Point(16, 148),
                Size = new Size(210, 34),
                BackColor = Color.FromArgb(24, 90, 157),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = !_soloLectura
            };
            btnRegistrar.Click += BtnRegistrar_Click;

            Button btnModificar = new Button
            {
                Text = "Modificar Ubicación",
                Location = new Point(240, 148),
                Size = new Size(210, 34),
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = !_soloLectura
            };
            btnModificar.Click += BtnModificar_Click;

            Button btnLimpiar = new Button
            {
                Text = "Limpiar Campos",
                Location = new Point(464, 148),
                Size = new Size(160, 34),
                FlatStyle = FlatStyle.Flat
            };
            btnLimpiar.Click += (s, e) =>
            {
                txtIdUbicacion.Clear();
                txtNombreLugar.Clear();
                txtDireccion.Clear();
                txtLatitud.Text = "3.45100000";
                txtLongitud.Text = "-76.53400000";
            };

            grp.Controls.AddRange(new Control[]
            {
                txtIdUbicacion, txtNombreLugar, txtDireccion, cboComuna, cboBarrio,
                txtLatitud, txtLongitud, btnRegistrar, btnModificar, btnLimpiar
            });
            Controls.Add(grp);

            dgvUbicaciones = new DataGridView
            {
                Location = new Point(16, 224),
                Size = new Size(930, 300),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            dgvUbicaciones.SelectionChanged += DgvUbicaciones_SelectionChanged;
            Controls.Add(dgvUbicaciones);
        }

        private void CargarCatalogos()
        {
            cboComuna.DataSource = _ctrlUbicacion.ListarComunas();
            cboComuna.DisplayMember = "EtiquetaComuna";
            cboComuna.ValueMember = "IdComuna";
        }

        private void CboComuna_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cboComuna.SelectedValue is string idComuna)
            {
                cboBarrio.DataSource = _ctrlUbicacion.ListarBarriosPorComuna(idComuna);
                cboBarrio.DisplayMember = "NombreBarrio";
                cboBarrio.ValueMember = "IdBarrio";
            }
        }

        private void RefrescarTabla()
        {
            dgvUbicaciones.DataSource = _ctrlUbicacion.ConsultarTablaUbicaciones();
        }

        private void DgvUbicaciones_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvUbicaciones.CurrentRow == null) return;
            string? id = dgvUbicaciones.CurrentRow.Cells["ID"]?.Value?.ToString();
            if (string.IsNullOrWhiteSpace(id)) return;

            Ubicacion? u = _ctrlUbicacion.BuscarUbicacion(id);
            if (u != null)
            {
                txtIdUbicacion.Text = u.IdUbicacion;
                txtNombreLugar.Text = u.NombreLugar;
                txtDireccion.Text = u.Direccion;
                txtLatitud.Text = u.Latitud.ToString("0.00000000", CultureInfo.InvariantCulture);
                txtLongitud.Text = u.Longitud.ToString("0.00000000", CultureInfo.InvariantCulture);

                cboBarrio.DataSource = _ctrlUbicacion.ListarBarrios();
                cboBarrio.DisplayMember = "EtiquetaBarrioComuna";
                cboBarrio.ValueMember = "IdBarrio";
                cboBarrio.SelectedValue = u.IdBarrioFK;
            }
        }

        private bool ParsearCoordenadas(out decimal lat, out decimal lon)
        {
            lat = 0;
            lon = 0;
            string sLat = txtLatitud.Text.Trim().Replace(',', '.');
            string sLon = txtLongitud.Text.Trim().Replace(',', '.');

            return decimal.TryParse(sLat, NumberStyles.Any, CultureInfo.InvariantCulture, out lat) &&
                   decimal.TryParse(sLon, NumberStyles.Any, CultureInfo.InvariantCulture, out lon);
        }

        private void BtnRegistrar_Click(object? sender, EventArgs e)
        {
            if (!ParsearCoordenadas(out decimal lat, out decimal lon))
            {
                MessageBox.Show("Ingrese coordenadas numéricas válidas.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var u = new Ubicacion
            {
                NombreLugar = txtNombreLugar.Text,
                Direccion = txtDireccion.Text,
                Latitud = lat,
                Longitud = lon,
                IdBarrioFK = cboBarrio.SelectedValue?.ToString() ?? string.Empty
            };

            if (_ctrlUbicacion.InsertarUbicacion(u, out string msg))
            {
                MessageBox.Show("Ubicación registrada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefrescarTabla();
            }
            else
            {
                MessageBox.Show("No se pudo registrar:\n" + msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnModificar_Click(object? sender, EventArgs e)
        {
            if (!ParsearCoordenadas(out decimal lat, out decimal lon))
            {
                MessageBox.Show("Ingrese coordenadas numéricas válidas.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var u = new Ubicacion
            {
                IdUbicacion = txtIdUbicacion.Text,
                NombreLugar = txtNombreLugar.Text,
                Direccion = txtDireccion.Text,
                Latitud = lat,
                Longitud = lon,
                IdBarrioFK = cboBarrio.SelectedValue?.ToString() ?? string.Empty
            };

            if (_ctrlUbicacion.ActualizarUbicacion(u, out string msg))
            {
                MessageBox.Show("Ubicación actualizada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefrescarTabla();
            }
            else
            {
                MessageBox.Show("No se pudo actualizar:\n" + msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
