using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Finpect.WinForms.Charts;
using Finpect.WinForms.UI;

namespace Finpect.WinForms.Views
{
    // Ana panel: Görsel tasarımın çalışan WinForms karşılığı.
    // Bu ekrandaki portföy tutarları ve fiyatlar TAMAMEN kurgusal örneklerdir.
    public sealed class DashboardPage : UserControl
    {
        private readonly Panel scroll = new Panel();
        private readonly TableLayoutPanel layout = Ui.Table(1, 3);
        private readonly MiniChart performanceChart = new MiniChart();
        private readonly DataGridView holdingsGrid = new DataGridView();
        private readonly Dictionary<string, Button> rangeButtons = new Dictionary<string, Button>();
        private readonly Dictionary<string, Button> filterButtons = new Dictionary<string, Button>();
        private readonly List<Holding> holdings = new List<Holding>
        {
            // Bu rakamların toplamı ve dağılımı ana ekrandaki TEMSİLİ tutarlarla tutarlıdır.
            new Holding("Arven Yazılım", "Hisse", "800", "₺120,00", "₺96.000", "+%4,8"),
            new Holding("Mera Sanayi", "Hisse", "502", "₺75,00", "₺37.650", "+%1,9"),
            new Holding("Duru Tüketim", "Hisse", "285", "₺60,00", "₺17.100", "+%3,1"),
            new Holding("Örnek Coin", "Kripto", "90", "₺876,00", "₺78.840", "-%2,3"),
            new Holding("Örnek Altın", "Emtia", "4", "₺8.040", "₺32.160", "+%0,6"),
            new Holding("Nakit", "Nakit", "1", "₺27.400", "₺27.400", "+%0,0"),
            new Holding("Örnek Fon", "Diğer", "293", "₺100,00", "₺29.300", "+%1,2")
        };

        public DashboardPage()
        {
            BackColor = Ui.Background;
            Dock = DockStyle.Fill;
            scroll.Dock = DockStyle.Fill;
            scroll.AutoScroll = true;
            scroll.BackColor = Ui.Background;
            Controls.Add(scroll);

            layout.Dock = DockStyle.None;
            layout.Padding = new Padding(24, 5, 24, 10);
            layout.Height = 880;
            Ui.Cols(layout, 100);
            Ui.Rows(layout, 135, 353, 347);
            scroll.Controls.Add(layout);
            scroll.Resize += (s, e) =>
            {
                layout.Width = Math.Max(1080, scroll.ClientSize.Width - 2);
            };
            layout.Width = 1400;

            BuildKpis();
            BuildUpperContent();
            BuildLowerContent();
            SetRange("3A");
            RefreshHoldings("Tümü");
        }

        private void BuildKpis()
        {
            var line = Ui.Table(4);
            Ui.Cols(line, 25, 25, 25, 25);
            line.Padding = new Padding(0, 8, 0, 18);
            line.Margin = Padding.Empty;
            layout.Controls.Add(line, 0, 0);

            AddMetric(line, 0, "Toplam Portföy Değeri", "₺318.450", "+%4,72", false);
            AddMetric(line, 1, "Toplam Getiri", "₺52.780", "+%19,87", false);
            AddMetric(line, 2, "Günlük Değişim", "+₺14.360", "+%4,72", true);
            AddMetric(line, 3, "Nakit Bakiye", "₺27.400", "", false);
        }

        private static void AddMetric(TableLayoutPanel parent, int index, string caption,
            string value, string difference, bool green)
        {
            var metric = Ui.Table(1, 3);
            metric.Padding = new Padding(index == 0 ? 6 : 25, 8, 8, 4);
            metric.Margin = new Padding(0, 0, 8, 0);
            Ui.Cols(metric, 100);
            Ui.Rows(metric, 35, 46, 28);
            metric.Controls.Add(Ui.Label(caption, 10.2f, color: Ui.Muted), 0, 0);
            metric.Controls.Add(Ui.Label(value, 21, true, green ? Ui.Green : Ui.Text), 0, 1);
            metric.Controls.Add(Ui.Label(difference, 9.5f, true, Ui.Green), 0, 2);
            if (index > 0)
            {
                metric.Paint += (s, e) =>
                {
                    using var separator = new Pen(Ui.Divider);
                    e.Graphics.DrawLine(separator, 1, 8, 1, metric.Height - 9);
                };
            }
            parent.Controls.Add(metric, index, 0);
        }

        private void BuildUpperContent()
        {
            var upper = Ui.Table(3, 1);
            Ui.Cols(upper, 48, 30, 22);
            upper.Padding = new Padding(0, 17, 0, 13);
            upper.Margin = Padding.Empty;
            layout.Controls.Add(upper, 0, 1);

            // 1: Büyük performans grafiği
            var perf = Ui.Table(1, 2);
            Ui.Rows(perf, 44, 270);
            perf.Padding = new Padding(5, 0, 24, 0);
            perf.Margin = Padding.Empty;
            var pHead = Ui.Table(2);
            Ui.Cols(pHead, 43, 57);
            pHead.Controls.Add(Ui.Title("Portföy Performansı", 14), 0, 0);
            var choices = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false, BackColor = Ui.Background, Margin = Padding.Empty
            };
            foreach (string range in new[] { "Tümü", "1Y", "3A", "1A", "1H", "1G" })
            {
                string picked = range;
                var b = Ui.Button(range);
                b.Dock = DockStyle.None;
                b.Width = range == "Tümü" ? 58 : 43;
                b.Height = 32;
                b.Font = new Font("Segoe UI", 9.5f);
                b.Click += (s, e) => SetRange(picked);
                rangeButtons.Add(range, b);
                choices.Controls.Add(b);
            }
            pHead.Controls.Add(choices, 1, 0);
            perf.Controls.Add(pHead, 0, 0);
            performanceChart.Dock = DockStyle.Fill;
            perf.Controls.Add(performanceChart, 0, 1);
            upper.Controls.Add(perf, 0, 0);

            // 2: Donut ve dağılım legend'ı
            var allocation = Ui.Table(1, 2);
            Ui.Rows(allocation, 44, 270);
            allocation.Padding = new Padding(14, 0, 13, 0);
            allocation.Controls.Add(Ui.Title("Portföy Dağılımı", 14), 0, 0);
            var allocBody = Ui.Table(2);
            Ui.Cols(allocBody, 56, 44);
            allocBody.Controls.Add(new DonutChart { Dock = DockStyle.Fill, Margin = new Padding(0) }, 0, 0);
            var legends = Ui.Table(1, 5);
            Ui.Cols(legends, 100);
            Ui.Rows(legends, 43, 43, 43, 43, 43);
            string[] names = { "Hisse", "Kripto", "Altın", "Nakit", "Diğer" };
            string[] percentages = { "47,3%", "24,8%", "10,1%", "8,6%", "9,2%" };
            var colors = new DonutChart().Colors;
            for (int i = 0; i < names.Length; i++)
            {
                var row = Ui.Table(3);
                Ui.Cols(row, 12, 60, 28);
                var square = new Panel
                {
                    Width = 10, Height = 10, BackColor = colors[i], Anchor = AnchorStyles.Left,
                    Margin = new Padding(1, 0, 0, 0)
                };
                row.Controls.Add(square, 0, 0);
                row.Controls.Add(Ui.Label(names[i], 9.2f), 1, 0);
                row.Controls.Add(Ui.Label(percentages[i], 9.2f, alignment: ContentAlignment.MiddleRight), 2, 0);
                legends.Controls.Add(row, 0, i);
            }
            allocBody.Controls.Add(legends, 1, 0);
            allocation.Controls.Add(allocBody, 0, 1);
            upper.Controls.Add(allocation, 1, 0);

            // 3: İzleme listesi; sadece demo veri
            var watch = Ui.Table(1, 2);
            Ui.Rows(watch, 44, 270);
            watch.Padding = new Padding(24, 0, 4, 0);
            var watchHeading = Ui.Title("İzleme Listesi", 14);
            watch.Controls.Add(watchHeading, 0, 0);
            var watchRows = Ui.Table(1, 6);
            Ui.Cols(watchRows, 100);
            Ui.Rows(watchRows, 31, 42, 42, 42, 42, 42);
            watchRows.Controls.Add(CreateListRow("Sembol", "Örnek fiyat", "Değişim", true), 0, 0);
            watchRows.Controls.Add(CreateListRow("ARV", "₺120,00", "+%4,8"), 0, 1);
            watchRows.Controls.Add(CreateListRow("MER", "₺75,00", "+%1,9"), 0, 2);
            watchRows.Controls.Add(CreateListRow("KZE", "₺90,00", "-%2,3"), 0, 3);
            watchRows.Controls.Add(CreateListRow("DUR", "₺60,00", "+%3,1"), 0, 4);
            watchRows.Controls.Add(CreateListRow("NVR", "₺230,00", "+%7,2"), 0, 5);
            watch.Controls.Add(watchRows, 0, 1);
            upper.Controls.Add(watch, 2, 0);
        }

        private static TableLayoutPanel CreateListRow(string a, string b, string c, bool heading = false)
        {
            var row = Ui.Table(3);
            Ui.Cols(row, 25, 43, 32);
            row.Controls.Add(Ui.Label(a, heading ? 8.5f : 10,
                !heading, heading ? Ui.Muted : Ui.Text), 0, 0);
            row.Controls.Add(Ui.Label(b, 9.5f, color: heading ? Ui.Muted : Ui.Text,
                alignment: ContentAlignment.MiddleRight), 1, 0);
            row.Controls.Add(Ui.Label(c, 9.5f, !heading,
                heading ? Ui.Muted : c.StartsWith("-") ? Ui.Red : Ui.Green,
                ContentAlignment.MiddleRight), 2, 0);
            return row;
        }

        private void BuildLowerContent()
        {
            var bottom = Ui.Table(2);
            Ui.Cols(bottom, 73, 27);
            bottom.Padding = new Padding(0, 8, 0, 0);
            bottom.Margin = Padding.Empty;
            layout.Controls.Add(bottom, 0, 2);

            var holdingsSection = Ui.Table(1, 3);
            Ui.Rows(holdingsSection, 45, 40, 235);
            holdingsSection.Padding = new Padding(5, 0, 26, 0);
            var titleRow = Ui.Table(2);
            Ui.Cols(titleRow, 51, 49);
            titleRow.Controls.Add(Ui.Title("Portföy Detayı", 14), 0, 0);
            var filterBar = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false, BackColor = Ui.Background, Margin = Padding.Empty
            };
            foreach (string filter in new[] { "Diğer", "Nakit", "Emtia", "Kripto", "Hisse", "Tümü" })
            {
                string picked = filter;
                var b = Ui.Button(filter);
                b.Dock = DockStyle.None;
                b.Width = filter == "Tümü" ? 65 : 62;
                b.Height = 31;
                b.Font = new Font("Segoe UI", 9.3f);
                b.Click += (s, e) => RefreshHoldings(picked);
                filterButtons.Add(filter, b);
                filterBar.Controls.Add(b);
            }
            titleRow.Controls.Add(filterBar, 1, 0);
            holdingsSection.Controls.Add(titleRow, 0, 0);
            holdingsSection.Controls.Add(Ui.Label("Örnek kayıtlar — canlı veri veya gerçek kişisel portföy değildir", 8.7f, color: Ui.Muted), 0, 1);

            Ui.StyleGrid(holdingsGrid);
            holdingsGrid.Columns.Add("Name", "Varlık");
            holdingsGrid.Columns.Add("Category", "Tür");
            holdingsGrid.Columns.Add("Amount", "Adet");
            holdingsGrid.Columns.Add("Price", "Örnek Fiyat");
            holdingsGrid.Columns.Add("Value", "Değer");
            holdingsGrid.Columns.Add("Return", "Getiri (%)");
            holdingsGrid.Columns[0].FillWeight = 32;
            holdingsGrid.Columns[1].FillWeight = 13;
            holdingsGrid.Columns[2].FillWeight = 10;
            holdingsGrid.Columns[3].FillWeight = 17;
            holdingsGrid.Columns[4].FillWeight = 14;
            holdingsGrid.Columns[5].FillWeight = 14;
            holdingsGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            holdingsSection.Controls.Add(holdingsGrid, 0, 2);
            bottom.Controls.Add(holdingsSection, 0, 0);

            var market = Ui.Table(1, 2);
            Ui.Rows(market, 45, 250);
            market.Padding = new Padding(22, 0, 2, 0);
            market.Controls.Add(Ui.Title("Piyasa Özeti", 14), 0, 0);
            var marketRows = Ui.Table(1, 5);
            Ui.Cols(marketRows, 100);
            Ui.Rows(marketRows, 44, 44, 44, 44, 44);
            marketRows.Controls.Add(CreateListRow("BIST 100*", "9.200", "+%0,8"), 0, 0);
            marketRows.Controls.Add(CreateListRow("S&P 500*", "6.020", "+%1,2"), 0, 1);
            marketRows.Controls.Add(CreateListRow("USD/TRY*", "34,20", "+%0,2"), 0, 2);
            marketRows.Controls.Add(CreateListRow("BTC/USD*", "60.000", "-%0,4"), 0, 3);
            marketRows.Controls.Add(Ui.Label("* Temsili piyasa verisi", 8.5f, color: Ui.Muted), 0, 4);
            market.Controls.Add(marketRows, 0, 1);
            bottom.Controls.Add(market, 1, 0);
        }

        private void SetRange(string selected)
        {
            foreach (var item in rangeButtons)
            {
                item.Value.BackColor = item.Key == selected ? Ui.Selected : Ui.Background;
                item.Value.Font = new Font("Segoe UI", 9.5f,
                    item.Key == selected ? FontStyle.Bold : FontStyle.Regular);
            }
            int count = selected switch
            {
                "1G" => 20, "1H" => 30, "1A" => 42,
                "3A" => 70, "1Y" => 90, _ => 100
            };
            // Aynı aralık seçildiğinde aynı grafik üretilir: test edilebilir demo.
            var random = new Random(42 + count);
            double value = 245000;
            var points = new double[count];
            for (int i = 0; i < count; i++)
            {
                value += 83000.0 / count + (random.NextDouble() - 0.5) * 8500;
                points[i] = value;
            }
            double correction = 318450 - points[count - 1];
            for (int i = 0; i < count; i++)
                points[i] += correction * i / Math.Max(1, count - 1);
            performanceChart.SetValues(points);
        }

        private void RefreshHoldings(string selected)
        {
            foreach (var item in filterButtons)
                item.Value.BackColor = item.Key == selected ? Ui.Selected : Ui.Background;
            holdingsGrid.Rows.Clear();
            foreach (var holding in holdings.Where(h => selected == "Tümü" || h.Category == selected))
            {
                int index = holdingsGrid.Rows.Add(holding.Name, holding.Category, holding.Amount,
                    holding.Price, holding.Value, holding.Return);
                holdingsGrid.Rows[index].Cells[5].Style.ForeColor =
                    holding.Return.StartsWith("-") ? Ui.Red : Ui.Green;
            }
            holdingsGrid.ClearSelection();
        }

        private sealed record Holding(string Name, string Category, string Amount,
            string Price, string Value, string Return);
    }
}
