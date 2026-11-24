using System;
using System.Drawing;
using System.Windows.Forms;
using GaleriaArteUrbanoCali.controllers;
using GaleriaArteUrbanoCali.models;

namespace GaleriaArteUrbanoCali.views
{
    public class UI_GestionArtistas : Form
    {
        private readonly ControllerArtista _ctrlArtista = new ControllerArtista();
        private readonly bool _soloLectura;

        private TextBox txtIdArtista = new TextBox();
        private TextBox txtNombreArtistico = new TextBox();
        private TextBox txtNombre1Real = new TextBox();
        private TextBox txtNombre2Real = new TextBox();
        private TextBox txtApellido1Real = new TextBox();
        private TextBox txtApellido2Real = new TextBox();
        private ComboBox cboPais = new ComboBox();
        private TextBox txtRedSocial = new TextBox();
        private TextBox txtBiografia = new TextBox();
        private CheckBox chkEstado = new CheckBox();
        private DataGridView dgvArtistas = new DataGridView();

        public UI_GestionArtistas(bool soloLectura = false)
        {
            _soloLectura = soloLectura;
            ConfigurarInterfaz();
            CargarDatos();
        }

        private void ConfigurarInterfaz()
        {
            Text = "Gestión de Artistas";
            Size = new Size(1020, 640);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9F);

            GroupBox grpDatos = new GroupBox
            {
                Text = "Datos del Artista",
                Location = new Point(16, 12),
                Size = new Size(970, 235),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };

            grpDatos.Controls.Add(new Label { Text = "ID Artista:", Location = new Point(16, 28), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtIdArtista = new TextBox { Location = new Point(16, 48), Width = 250, ReadOnly = true, BackColor = Color.WhiteSmoke, Font = new Font("Segoe UI", 9F) };

            grpDatos.Controls.Add(new Label { Text = "Nombre Artístico*:", Location = new Point(280, 28), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtNombreArtistico = new TextBox { Location = new Point(280, 48), Width = 220, Font = new Font("Segoe UI", 9F) };

            grpDatos.Controls.Add(new Label { Text = "País Origen*:", Location = new Point(515, 28), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            cboPais = new ComboBox { Location = new Point(515, 48), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F) };

            chkEstado = new CheckBox { Text = "Artista Activo", Location = new Point(735, 48), AutoSize = true, Checked = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };

            grpDatos.Controls.Add(new Label { Text = "1er Nombre Real*:", Location = new Point(16, 82), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtNombre1Real = new TextBox { Location = new Point(16, 102), Width = 165, Font = new Font("Segoe UI", 9F) };

            grpDatos.Controls.Add(new Label { Text = "2do Nombre Real:", Location = new Point(195, 82), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtNombre2Real = new TextBox { Location = new Point(195, 102), Width = 165, Font = new Font("Segoe UI", 9F) };

            grpDatos.Controls.Add(new Label { Text = "1er Apellido Real*:", Location = new Point(375, 82), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtApellido1Real = new TextBox { Location = new Point(375, 102), Width = 165, Font = new Font("Segoe UI", 9F) };

            grpDatos.Controls.Add(new Label { Text = "2do Apellido Real:", Location = new Point(555, 82), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtApellido2Real = new TextBox { Location = new Point(555, 102), Width = 165, Font = new Font("Segoe UI", 9F) };

            grpDatos.Controls.Add(new Label { Text = "Perfil Red Social:", Location = new Point(735, 82), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtRedSocial = new TextBox { Location = new Point(735, 102), Width = 215, Font = new Font("Segoe UI", 9F) };

            grpDatos.Controls.Add(new Label { Text = "Biografía / Reseña:", Location = new Point(16, 136), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtBiografia = new TextBox { Location = new Point(16, 156), Width = 540, Height = 60, Multiline = true, ScrollBars = ScrollBars.Vertical, Font = new Font("Segoe UI", 9F) };

            Button btnRegistrar = new Button
            {
                Text = "Registrar Nuevo",
                Location = new Point(575, 156),
                Size = new Size(125, 36),
                BackColor = Color.FromArgb(24, 90, 157),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = !_soloLectura
            };
            btnRegistrar.Click += BtnRegistrar_Click;

            Button btnModificar = new Button
            {
                Text = "Modificar Artista",
                Location = new Point(708, 156),
                Size = new Size(125, 36),
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = !_soloLectura
            };
            btnModificar.Click += BtnModificar_Click;

            Button btnEliminarDesactivar = new Button
            {
                Text = "Eliminar / Desactivar",
                Location = new Point(840, 156),
                Size = new Size(115, 36),
                BackColor = Color.FromArgb(183, 28, 28),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = !_soloLectura
            };
            btnEliminarDesactivar.Click += BtnEliminarDesactivar_Click;

            Button btnLimpiar = new Button
            {
                Text = "Limpiar Campos",
                Location = new Point(575, 196),
                Size = new Size(380, 28),
                FlatStyle = FlatStyle.Flat
            };
            btnLimpiar.Click += (s, e) => LimpiarFormulario();

            grpDatos.Controls.AddRange(new Control[]
            {
                txtIdArtista, txtNombreArtistico, cboPais, chkEstado,
                txtNombre1Real, txtNombre2Real, txtApellido1Real, txtApellido2Real,
                txtRedSocial, txtBiografia, btnRegistrar, btnModificar, btnEliminarDesactivar, btnLimpiar
            });
            Controls.Add(grpDatos);

            dgvArtistas = new DataGridView
            {
                Location = new Point(16, 258),
                Size = new Size(970, 325),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            dgvArtistas.SelectionChanged += DgvArtistas_SelectionChanged;
            Controls.Add(dgvArtistas);
        }

        private void CargarDatos()
        {
            cboPais.DataSource = _ctrlArtista.ListarPaises();
            cboPais.DisplayMember = "NombrePais";
            cboPais.ValueMember = "IdPais";

            RefrescarTabla();
        }

        private void RefrescarTabla()
        {
            dgvArtistas.DataSource = _ctrlArtista.ConsultarTablaArtistas();
        }

        private void DgvArtistas_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvArtistas.CurrentRow == null) return;
            string? id = dgvArtistas.CurrentRow.Cells["ID"]?.Value?.ToString();
            if (string.IsNullOrWhiteSpace(id)) return;

            Artista? a = _ctrlArtista.BuscarArtista(id);
            if (a != null)
            {
                txtIdArtista.Text = a.IdArtista;
                txtNombreArtistico.Text = a.NombreArtistico;
                txtNombre1Real.Text = a.Nombre1Real;
                txtNombre2Real.Text = a.Nombre2Real ?? string.Empty;
                txtApellido1Real.Text = a.Apellido1Real;
                txtApellido2Real.Text = a.Apellido2Real ?? string.Empty;
                txtRedSocial.Text = a.PerfilRedSocial ?? string.Empty;
                txtBiografia.Text = a.Biografia ?? string.Empty;
                chkEstado.Checked = a.EstadoArtista;
                cboPais.SelectedValue = a.IdPaisFK;
            }
        }

        private void BtnRegistrar_Click(object? sender, EventArgs e)
        {
            var a = new Artista
            {
                NombreArtistico = txtNombreArtistico.Text,
                Nombre1Real = txtNombre1Real.Text,
                Nombre2Real = txtNombre2Real.Text,
                Apellido1Real = txtApellido1Real.Text,
                Apellido2Real = txtApellido2Real.Text,
                PerfilRedSocial = txtRedSocial.Text,
                Biografia = txtBiografia.Text,
                EstadoArtista = chkEstado.Checked,
                IdPaisFK = cboPais.SelectedValue?.ToString() ?? string.Empty
            };

            if (_ctrlArtista.InsertarArtista(a, out string msg))
            {
                MessageBox.Show("Artista registrado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarFormulario();
                RefrescarTabla();
            }
            else
            {
                MessageBox.Show("Error al registrar artista:\n" + msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnModificar_Click(object? sender, EventArgs e)
        {
            var a = new Artista
            {
                IdArtista = txtIdArtista.Text,
                NombreArtistico = txtNombreArtistico.Text,
                Nombre1Real = txtNombre1Real.Text,
                Nombre2Real = txtNombre2Real.Text,
                Apellido1Real = txtApellido1Real.Text,
                Apellido2Real = txtApellido2Real.Text,
                PerfilRedSocial = txtRedSocial.Text,
                Biografia = txtBiografia.Text,
                EstadoArtista = chkEstado.Checked,
                IdPaisFK = cboPais.SelectedValue?.ToString() ?? string.Empty
            };

            if (_ctrlArtista.ActualizarArtista(a, out string msg))
            {
                MessageBox.Show("Datos del artista actualizados correctamente.", "Modificación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefrescarTabla();
            }
            else
            {
                MessageBox.Show("No se pudo actualizar:\n" + msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnEliminarDesactivar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdArtista.Text))
            {
                MessageBox.Show("Seleccione primero un artista de la tabla.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int cantObras = _ctrlArtista.ContarObrasDeArtista(txtIdArtista.Text);
            string pregunta = cantObras > 0
                ? $"¿Está seguro de dar de baja al artista '{txtNombreArtistico.Text}'?\n\nNota: Tiene {cantObras} obra(s) registrada(s), por lo cual se DESACTIVARÁ lógicamente sin eliminar sus obras (Regla RN-02)."
                : $"¿Está seguro de eliminar definitivamente al artista '{txtNombreArtistico.Text}'?";

            DialogResult confirm = MessageBox.Show(pregunta, "Confirmar Eliminación / Desactivación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            if (_ctrlArtista.EliminarODesactivarArtista(txtIdArtista.Text, out string msg))
            {
                MessageBox.Show(msg, "Operación Completada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarFormulario();
                RefrescarTabla();
            }
            else
            {
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarFormulario()
        {
            txtIdArtista.Clear();
            txtNombreArtistico.Clear();
            txtNombre1Real.Clear();
            txtNombre2Real.Clear();
            txtApellido1Real.Clear();
            txtApellido2Real.Clear();
            txtRedSocial.Clear();
            txtBiografia.Clear();
            chkEstado.Checked = true;
        }
    }
}
