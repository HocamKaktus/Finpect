using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Finpect.WinForms.UI;
using Finpect.WinForms.Views;

namespace Finpect.WinForms
{
    // Bütün grup için ortak ana pencere: başlık, menü, sayfa alanı ve tema.
    public class MainForm : Form
    {
        private readonly Label pageTitle = Ui.Title("Ana Panel", 22);
        private readonly Panel body = new Panel();
        private readonly Dictionary<string, Button> navButtons = new Dictionary<string, Button>();
        private readonly Dictionary<string, Control> modulePages = new Dictionary<string, Control>();

        public MainForm()
        {
            Text = "FINPECT  |  Yatırım Analiz Platformu";
            StartPosition = FormStartPosition.CenterScreen;
            Width = 1580;
            Height = 940;
            MinimumSize = new Size(1230, 740);
            BackColor = Ui.Background;
            ForeColor = Ui.Text;
            Font = new Font("Segoe UI", 10);
            AutoScaleMode = AutoScaleMode.Dpi;
            DoubleBuffered = true;
            BuildShell();
            RegisterPages();
            Navigate("Ana Panel");
        }

        private void BuildShell()
        {
            var root = Ui.Table(1, 3);
            Ui.Rows(root, 80, 0, 31);
            // İlk ve son satır sabit; orta satır kalan tüm alanı kaplar.
            root.RowStyles[1] = new RowStyle(SizeType.Percent, 100);
            Controls.Add(root);

            var header = Ui.Table(9);
            header.Padding = new Padding(30, 10, 22, 9);
            Ui.Cols(header, 15, 10, 11, 14, 13, 14, 10, 7, 6);
            header.Controls.Add(pageTitle, 0, 0);
            var brand = Ui.Label("▥  Finpect", 12, true, Ui.Text);
            brand.Padding = new Padding(17, 0, 0, 0);
            header.Controls.Add(brand, 1, 0);

            string[] menus = { "Ana Panel", "Yatırımcı Analizi", "Risk Simülatörü",
                "Şirket Benzerliği", "Raporlar" };
            for (int i = 0; i < menus.Length; i++)
            {
                string id = menus[i];
                var button = Ui.Button(id);
                button.Font = new Font("Segoe UI", 9.3f);
                button.Click += (s, e) => Navigate(id);
                navButtons.Add(id, button);
                header.Controls.Add(button, i + 2, 0);
            }

            header.Controls.Add(Ui.Label("ÖRNEK VERİ", 8, true, Ui.Gold,
                ContentAlignment.MiddleRight), 7, 0);
            header.Controls.Add(Ui.Label("AK", 11, true, Ui.Text,
                ContentAlignment.MiddleCenter), 8, 0);
            root.Controls.Add(header, 0, 0);

            body.Dock = DockStyle.Fill;
            body.BackColor = Ui.Background;
            body.Margin = Padding.Empty;
            root.Controls.Add(body, 0, 1);

            var footer = Ui.Table(2);
            Ui.Cols(footer, 48, 52);
            footer.Padding = new Padding(35, 0, 30, 0);
            footer.Controls.Add(Ui.Label("FINPECT  ·  C# WinForms  ·  Ekip ortak arayüzü", 8.5f,
                color: Ui.Muted), 0, 0);
            footer.Controls.Add(Ui.Label("Tüm fiyatlar ve portföyler temsili örnektir.", 8.5f,
                color: Ui.Gold, alignment: ContentAlignment.MiddleRight), 1, 0);
            root.Controls.Add(footer, 0, 2);
        }

        private void RegisterPages()
        {
            modulePages.Add("Ana Panel", new DashboardPage());
            modulePages.Add("Yatırımcı Analizi", new InvestorPage());
            modulePages.Add("Risk Simülatörü", new PendingModulePage(
                "Risk Simülatörü", "Berke Yalçın",
                "Düşük, orta ve yüksek risk seviyeleri ile örnek fiyat senaryoları burada geliştirilecek. "
                + "Pozisyon büyüklüğü ve olası kâr/zarar hesaplama kodları bu ekrana bağlanabilir."));
            modulePages.Add("Şirket Benzerliği", new PendingModulePage(
                "Benzer Şirketler", "Alp Tuğ Avcı",
                "ML.NET ile şirketlerin finansal özelliklerinden öğrenilen kümeler ve "
                + "benzer şirket listeleri bu sayfaya eklenecek."));
            modulePages.Add("Raporlar", new PendingModulePage(
                "Raporlar ve Karar Geçmişi", "Ahmet Faik Dinçer",
                "SQLite'a kaydedilen örnek kararlar, haftalık/aylık analiz özetleri "
                + "ve gerçekleşen veya simüle edilen sonuçların karşılaştırılması bu sayfaya eklenecek."));
        }

        // Arkadaşların modüllerini sonradan ortak programa bağlamak için.
        public void RegisterModule(string menuName, UserControl page)
        {
            if (!modulePages.ContainsKey(menuName))
                throw new ArgumentException("Tanımsız menü: " + menuName);
            Control old = modulePages[menuName];
            if (body.Controls.Contains(old)) body.Controls.Remove(old);
            modulePages[menuName] = page;
            page.Dock = DockStyle.Fill;
            old.Dispose();
        }

        private void Navigate(string menuName)
        {
            if (!modulePages.TryGetValue(menuName, out Control? page)) return;
            pageTitle.Text = menuName;
            foreach (var entry in navButtons)
            {
                bool active = entry.Key == menuName;
                entry.Value.BackColor = active ? Ui.Selected : Ui.Background;
                entry.Value.Font = new Font("Segoe UI", 9.3f,
                    active ? FontStyle.Bold : FontStyle.Regular);
            }
            body.SuspendLayout();
            body.Controls.Clear();
            page.Dock = DockStyle.Fill;
            body.Controls.Add(page);
            body.ResumeLayout();
        }
    }
}
