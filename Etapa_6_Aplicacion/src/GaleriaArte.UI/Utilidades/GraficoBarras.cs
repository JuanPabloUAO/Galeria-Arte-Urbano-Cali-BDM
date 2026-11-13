using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GaleriaArte.Entidades;

namespace GaleriaArte.UI.Utilidades;

/// <summary>
/// Gráfico de barras horizontales muy simple, dibujado con GDI+ (sin librerías externas).
/// Muestra el resultado de las consultas de estadísticas.
/// </summary>
internal class GraficoBarras : Panel
{
    private List<Estadistica> _datos = new();

    public GraficoBarras()
    {
        DoubleBuffered = true;
        BackColor = Color.White;
        ResizeRedraw = true;
    }

    public void Mostrar(List<Estadistica> datos)
    {
        _datos = datos;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        if (_datos.Count == 0)
        {
            TextRenderer.DrawText(g, "Sin datos para mostrar", Ui.Fuente, ClientRectangle, Color.Gray);
            return;
        }

        const int margen = 12, anchoEtiqueta = 190, altoFila = 34;
        decimal maximo = Math.Max(1, _datos.Max(d => d.Valor));
        int anchoUtil = Math.Max(50, Width - anchoEtiqueta - margen * 2 - 60);
        using var pincel = new SolidBrush(Ui.ColorPrimario);

        for (int i = 0; i < _datos.Count; i++)
        {
            var d = _datos[i];
            int y = margen + i * altoFila;
            var rectEtiqueta = new Rectangle(margen, y, anchoEtiqueta - 8, altoFila - 8);
            TextRenderer.DrawText(g, d.Etiqueta, Ui.Fuente, rectEtiqueta, Color.Black,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            int ancho = (int)(anchoUtil * (d.Valor / maximo));
            g.FillRectangle(pincel, margen + anchoEtiqueta, y + 4, Math.Max(2, ancho), altoFila - 16);
            TextRenderer.DrawText(g, d.Valor.ToString("0.##"), Ui.FuenteNegrita,
                new Point(margen + anchoEtiqueta + ancho + 6, y + 5), Color.Black);
        }
    }
}
