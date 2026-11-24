using System;
using System.Drawing;
using System.Windows.Forms;

namespace Finpect.WinForms.UI
{
    // Uygulamadaki bütün ekranların ortak renkleri / yazı tipleri.
    // Grubun diğer üyeleri de aynı sınıfı kullanabilir.
    internal static class Ui
    {
        public static readonly Color Background = Color.FromArgb(11, 14, 17);
        public static readonly Color Panel = Color.FromArgb(15, 19, 22);
        public static readonly Color Text = Color.FromArgb(242, 244, 241);
        public static readonly Color Muted = Color.FromArgb(154, 163, 163);
        public static readonly Color Divider = Color.FromArgb(47, 54, 55);
        public static readonly Color Green = Color.FromArgb(112, 216, 149);
        public static readonly Color Red = Color.FromArgb(239, 117, 100);
        public static readonly Color Gold = Color.FromArgb(157, 133, 91);
        public static readonly Color Selected = Color.FromArgb(79, 69, 54);

        public static Label Label(string text, float size = 10, bool bold = false,
            Color? color = null, ContentAlignment alignment = ContentAlignment.MiddleLeft)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = color ?? Text,
                BackColor = Color.Transparent,
                TextAlign = alignment,
                AutoEllipsis = true,
                Dock = DockStyle.Fill,
                Margin = new Padding(0)
            };
        }

        public static Label Title(string text, float size = 17) => Label(text, size, true);

        public static Button Button(string text, bool selected = false)
        {
            var result = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 10, selected ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = Text,
                BackColor = selected ? Selected : Background,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TabStop = true,
                UseVisualStyleBackColor = false,
                Margin = new Padding(4, 4, 4, 4),
                Dock = DockStyle.Fill
            };
            result.FlatAppearance.BorderSize = 0;
            result.FlatAppearance.MouseOverBackColor = selected ? Selected : Color.FromArgb(29, 35, 37);
            result.FlatAppearance.MouseDownBackColor = Color.FromArgb(50, 60, 60);
            return result;
        }

        public static Panel Line(bool vertical = false)
        {
            return new Panel
            {
                BackColor = Divider,
                Width = vertical ? 1 : 0,
                Height = vertical ? 0 : 1,
                Dock = vertical ? DockStyle.Left : DockStyle.Bottom,
                Margin = Padding.Empty
            };
        }

        public static TableLayoutPanel Table(int columns, int rows = 1)
        {
            return new TableLayoutPanel
            {
                ColumnCount = columns,
                RowCount = rows,
                BackColor = Background,
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
        }

        public static void Cols(TableLayoutPanel layout, params float[] percents)
        {
            layout.ColumnStyles.Clear();
            foreach (float percent in percents)
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, percent));
        }

        public static void Rows(TableLayoutPanel layout, params float[] heights)
        {
            layout.RowStyles.Clear();
            foreach (float height in heights)
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, height));
        }

        public static Panel Section(string title)
        {
            Panel panel = new Panel { Dock = DockStyle.Fill, BackColor = Background, Margin = Padding.Empty };
            Label header = Title(title, 14);
            header.Height = 40;
            header.Dock = DockStyle.Top;
            header.Padding = new Padding(0, 5, 0, 0);
            panel.Controls.Add(header);
            return panel;
        }

        public static void StyleCombo(ComboBox combo)
        {
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.FlatStyle = FlatStyle.Flat;
            combo.BackColor = Color.FromArgb(30, 36, 37);
            combo.ForeColor = Text;
            combo.Font = new Font("Segoe UI", 10);
            combo.Height = 34;
        }

        public static void StyleGrid(DataGridView grid)
        {
            grid.Dock = DockStyle.Fill;
            grid.BackgroundColor = Background;
            grid.BorderStyle = BorderStyle.None;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.RowHeadersVisible = false;
            grid.EnableHeadersVisualStyles = false;
            grid.AutoGenerateColumns = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Divider;
            grid.RowTemplate.Height = 40;
            grid.ColumnHeadersHeight = 40;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.Font = new Font("Segoe UI", 10);
            grid.DefaultCellStyle.BackColor = Background;
            grid.DefaultCellStyle.ForeColor = Text;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(32, 47, 43);
            grid.DefaultCellStyle.SelectionForeColor = Text;
            grid.DefaultCellStyle.Padding = new Padding(5, 4, 5, 4);
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(16, 20, 23);
            grid.ColumnHeadersDefaultCellStyle.BackColor = Background;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Muted;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
        }
    }
}
