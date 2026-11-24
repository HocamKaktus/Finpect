using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Finpect.WinForms.UI;

namespace Finpect.WinForms.Charts
{
    internal sealed class DonutChart : Control
    {
        // Veriler gerçek portföy dağılımı değildir.
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double[] Percentages { get; set; } = { 47.3, 24.8, 10.1, 8.6, 9.2 };
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color[] Colors { get; set; } =
        {
            Color.FromArgb(105, 201, 139), Color.FromArgb(104, 168, 148),
            Color.FromArgb(219, 178, 111), Color.FromArgb(130, 137, 137),
            Color.FromArgb(70, 92, 97)
        };

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowFill { get; set; } = true;

        public DonutChart()
        {
            DoubleBuffered = true;
            BackColor = Ui.Background;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int diameter = Math.Max(32, Math.Min(Width - 26, Height - 30));
            var r = new Rectangle((Width - diameter) / 2, (Height - diameter) / 2, diameter, diameter);
            float start = -90;
            if (ShowFill)
            {
                for (int i = 0; i < Percentages.Length && i < Colors.Length; i++)
                {
                    float sweep = (float)(Percentages[i] / 100.0 * 360);
                    using var brush = new SolidBrush(Colors[i]);
                    g.FillPie(brush, r, start, sweep);
                    start += sweep;
                }
            }
            var cutout = new Rectangle(r.X + diameter / 4, r.Y + diameter / 4,
                diameter / 2, diameter / 2);
            using (var bg = new SolidBrush(Ui.Background))
                g.FillEllipse(bg, cutout);
            using var bold = new Font("Segoe UI", 15, FontStyle.Bold);
            using var regular = new Font("Segoe UI", 9);
            using var fg = new SolidBrush(Ui.Text);
            using var secondary = new SolidBrush(Ui.Muted);
            var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("318.450", bold, fg, new RectangleF(cutout.X - 6, cutout.Y + diameter / 5 - 8, cutout.Width + 12, 27), format);
            g.DrawString("ÖRNEK ₺", regular, secondary,
                new RectangleF(cutout.X - 6, cutout.Y + diameter / 5 + 19, cutout.Width + 12, 18), format);
        }
    }
}
