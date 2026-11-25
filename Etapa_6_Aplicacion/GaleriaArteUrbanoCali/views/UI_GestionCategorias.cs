using System;
using System.Drawing;
using System.Windows.Forms;
using GaleriaArteUrbanoCali.controllers;
using GaleriaArteUrbanoCali.models;

namespace GaleriaArteUrbanoCali.views
{
    public class UI_GestionCategorias : Form
    {
        private readonly ControllerCategoria _ctrlCategoria = new ControllerCategoria();
        private readonly bool _soloLectura;

        private TextBox txtIdCategoria = new TextBox();
        private TextBox txtNombreCategoria = new TextBox();
        private TextBox txtDescripcion = new TextBox();
        private CheckBox chkEstado = new CheckBox();
        private DataGridView dgvCategorias = new DataGridView();

        public UI_GestionCategorias(bool soloLectura = false)
        {
            _soloLectura = soloLectura;
            ConfigurarInterfaz();
            RefrescarTabla();
        }

        private void ConfigurarInterfaz()
        {
            Text = "Gestión de Categorías Técnicas y Temáticas";
            Size = new Size(880, 560);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9F);

            GroupBox grp = new GroupBox
            {
                Text = "Datos de la Categoría",
                Location = new Point(16, 12),
                Size = new Size(830, 195),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };

            grp.Controls.Add(new Label { Text = "ID Categoría (UUID):", Location = new Point(16, 28), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtIdCategoria = new TextBox { Location = new Point(16, 48), Width = 260, ReadOnly = true, BackColor = Color.WhiteSmoke, Font = new Font("Segoe UI", 9F) };

            grp.Controls.Add(new Label { Text = "Nombre de Categoría*:", Location = new Point(290, 28), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtNombreCategoria = new TextBox { Location = new Point(290, 48), Width = 320, Font = new Font("Segoe UI", 9F) };

            chkEstado = new CheckBox { Text = "Categoría Activa", Location = new Point(630, 48), AutoSize = true, Checked = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };

            grp.Controls.Add(new Label { Text = "Descripción Técnica:", Location = new Point(16, 82), AutoSize = true, Font = new Font("Segoe UI", 8.5F) });
            txtDescripcion = new TextBox { Location = new Point(16, 102), Width = 440, Height = 65, Multiline = true, ScrollBars = ScrollBars.Vertical, Font = new Font("Segoe UI", 9F) };

            Button btnRegistrar = new Button
            {
                Text = "Registrar Nueva",
                Location = new Point(475, 102),
                Size = new Size(165, 32),
                BackColor = Color.FromArgb(24, 90, 157),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = !_soloLectura
            };
            btnRegistrar.Click += BtnRegistrar_Click;

            Button btnModificar = new Button
            {
                Text = "Modificar Categoría",
                Location = new Point(648, 102),
                Size = new Size(165, 32),
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = !_soloLectura
            };
            btnModificar.Click += BtnModificar_Click;

            Button btnAlternarEstado = new Button
            {
                Text = "Activar / Desactivar",
                Location = new Point(475, 140),
                Size = new Size(165, 30),
                BackColor = Color.FromArgb(230, 81, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = !_soloLectura
            };
            btnAlternarEstado.Click += BtnAlternarEstado_Click;

            Button btnLimpiar = new Button
            {
                Text = "Limpiar",
                Location = new Point(648, 140),
                Size = new Size(165, 30),
                FlatStyle = FlatStyle.Flat
            };
            btnLimpiar.Click += (s, e) =>
            {
                txtIdCategoria.Clear();
                txtNombreCategoria.Clear();
                txtDescripcion.Clear();
                chkEstado.Checked = true;
            };

            grp.Controls.AddRange(new Control[] { txtIdCategoria, txtNombreCategoria, chkEstado, txtDescripcion, btnRegistrar, btnModificar, btnAlternarEstado, btnLimpiar });
            Controls.Add(grp);

            dgvCategorias = new DataGridView
            {
                Location = new Point(16, 220),
                Size = new Size(830, 285),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            dgvCategorias.SelectionChanged += DgvCategorias_SelectionChanged;
            Controls.Add(dgvCategorias);
        }

        private void RefrescarTabla()
        {
            dgvCategorias.DataSource = _ctrlCategoria.ConsultarTablaCategorias();
        }

        private void DgvCategorias_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvCategorias.CurrentRow == null) return;
            string? id = dgvCategorias.CurrentRow.Cells["ID"]?.Value?.ToString();
            if (string.IsNullOrWhiteSpace(id)) return;

            Categoria? c = _ctrlCategoria.BuscarCategoria(id);
            if (c != null)
            {
                txtIdCategoria.Text = c.IdCategoria;
                txtNombreCategoria.Text = c.NombreCategoria;
                txtDescripcion.Text = c.DescripcionCategoria ?? string.Empty;
                chkEstado.Checked = c.EstadoCategoria;
            }
        }

        private void BtnRegistrar_Click(object? sender, EventArgs e)
        {
            var c = new Categoria
            {
                NombreCategoria = txtNombreCategoria.Text,
                DescripcionCategoria = txtDescripcion.Text,
                EstadoCategoria = chkEstado.Checked
            };

            if (_ctrlCategoria.InsertarCategoria(c, out string msg))
            {
                MessageBox.Show("Categoría registrada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefrescarTabla();
            }
            else
            {
                MessageBox.Show("Error:\n" + msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnModificar_Click(object? sender, EventArgs e)
        {
            var c = new Categoria
            {
                IdCategoria = txtIdCategoria.Text,
                NombreCategoria = txtNombreCategoria.Text,
                DescripcionCategoria = txtDescripcion.Text,
                EstadoCategoria = chkEstado.Checked
            };

            if (_ctrlCategoria.ActualizarCategoria(c, out string msg))
            {
                MessageBox.Show("Categoría actualizada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefrescarTabla();
            }
            else
            {
                MessageBox.Show("Error:\n" + msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnAlternarEstado_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdCategoria.Text)) return;
            bool nuevoEstado = !chkEstado.Checked;
            if (_ctrlCategoria.CambiarEstado(txtIdCategoria.Text, nuevoEstado, out string msg))
            {
                chkEstado.Checked = nuevoEstado;
                MessageBox.Show($"Categoría {(nuevoEstado ? "activada" : "desactivada")} correctamente.", "Estado Actualizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefrescarTabla();
            }
            else
            {
                MessageBox.Show("Error:\n" + msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
