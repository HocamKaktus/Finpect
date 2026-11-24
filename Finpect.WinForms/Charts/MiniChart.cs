using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Finpect.WinForms.UI;

namespace Finpect.WinForms.Charts
{
    // Herhangi bir harici grafik paketi gerektirmeyen örnek performans çizimi.
    // Bu seri tamamen demo amaçlıdır, canlı borsa verisi değildir.
    internal sealed class MiniChart : Control
    {
        private double[] values = Array.Empty<double>();
        public Color ChartColor { get; set; } = Ui.Green;
        public bool ShowAxis { get; set; } = true;
        public bool ShowFill { get; set; } = true;
        public string[] DateLabels { get; set; } = { "24 Ağu", "7 Eyl", "21 Eyl", "5 Eki", "19 Eki", "2 Kas", "16 Kas" };

        public MiniChart()
        {
            DoubleBuffered = true;
            BackColor = Ui.Background;
            ForeColor = Ui.Muted;
            Font = new Font("Segoe UI", 9);
        }

        public void SetValues(double[] source)
        {
            values = source == null ? Array.Empty<double>() : source.ToArray();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (values.Length < 2) return;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            float left = ShowAxis ? 48 : 3;
            float top = 10;
            float bottom = ShowAxis ? Height - 34 : Height - 4;
            float right = Width - 12;
            if (right <= left + 20 || bottom <= top + 20) return;
            double min = values.Min();
            double max = values.Max();
            double padding = Math.Max((max - min) * 0.18, 1);
            min -= padding;
            max += padding;
            var chartRect = new RectangleF(left, top, right - left, bottom - top);

            if (ShowAxis)
            {
                using var gridPen = new Pen(Color.FromArgb(35, 43, 45), 1);
                using var labelBrush = new SolidBrush(Ui.Muted);
                for (int i = 0; i <= 4; i++)
                {
                    float y = top + (bottom - top) * i / 4f;
                    g.DrawLine(gridPen, left, y, right, y);
                    string label = ((max - (max - min) * i / 4) / 1000.0).ToString("0") + "K";
                    g.DrawString(label, Font, labelBrush, 0, y - 7);
                }
                if (DateLabels != null && DateLabels.Length > 0)
                {
                    for (int i = 0; i < DateLabels.Length; i++)
                    {
                        float x = left + i * (right - left) / Math.Max(1, DateLabels.Length - 1);
                        g.DrawString(DateLabels[i], Font, labelBrush, x - 19, bottom + 7);
                    }
                }
            }

            var points = new PointF[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                float x = left + i * (right - left) / (values.Length - 1);
                float y = top + (float)((max - values[i]) / (max - min) * (bottom - top));
                points[i] = new PointF(x, y);
            }
            using var path = new GraphicsPath();
            path.AddLines(points);
            if (ShowFill)
            {
                using var fillPath = new GraphicsPath();
                fillPath.AddLines(points);
                fillPath.AddLine(points[^1].X, points[^1].Y, right, bottom);
                fillPath.AddLine(right, bottom, left, bottom);
                fillPath.CloseFigure();
                using var gradient = new LinearGradientBrush(chartRect,
                    Color.FromArgb(90, ChartColor), Color.FromArgb(4, ChartColor), LinearGradientMode.Vertical);
                g.FillPath(gradient, fillPath);
            }
            using var pen = new Pen(ChartColor, ShowAxis ? 2.1f : 1.6f);
            g.DrawPath(pen, path);
            if (ShowAxis)
            {
                using var marker = new SolidBrush(Ui.Background);
                g.FillEllipse(marker, points[^1].X - 4.3f, points[^1].Y - 4.3f, 8.6f, 8.6f);
                g.DrawEllipse(pen, points[^1].X - 4.3f, points[^1].Y - 4.3f, 8.6f, 8.6f);
            }
        }
    }
}
