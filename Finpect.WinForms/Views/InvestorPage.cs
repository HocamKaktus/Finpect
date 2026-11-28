using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Finpect.Core.Data;
using Finpect.Core.Models;
using Finpect.Core.Profiles;
using Finpect.Core.Services;
using Finpect.WinForms.UI;

namespace Finpect.WinForms.Views
{
    // Ali Kağan modülü: Daha önceki çalışan analiz sınıflarını kullanır.
    // Burada yatırımcı kuralları yeniden yazılmaz; arayüz yalnızca sonuçları gösterir.
    public sealed class InvestorPage : UserControl
    {
        private readonly InvestorAnalysisService service = new InvestorAnalysisService();
        private readonly List<Company> companies = DemoCompanies.GetAll();
        private readonly ComboBox companyBox = new ComboBox();
        private readonly ComboBox profileBox = new ComboBox();
        private readonly DataGridView criteriaGrid = new DataGridView();
        private readonly DataGridView comparisonGrid = new DataGridView();
        private readonly Label analysisTitle = Ui.Title("Kriter Analizi", 14);
        private readonly Label companyDetails = Ui.Label("", 10, color: Ui.Muted);
        private readonly Label summaryLabel = Ui.Label("", 11);
        private readonly TableLayoutPanel mainLayout = Ui.Table(1, 4);
        private readonly Panel scroll = new Panel();
        private readonly Dictionary<string, ScoreTile> scoreTiles = new Dictionary<string, ScoreTile>();
        private readonly Panel dataHost = new Panel();

        public InvestorPage()
        {
            Dock = DockStyle.Fill;
            BackColor = Ui.Background;
            scroll.Dock = DockStyle.Fill;
            scroll.AutoScroll = true;
            scroll.BackColor = Ui.Background;
            Controls.Add(scroll);
            mainLayout.Dock = DockStyle.None;
            mainLayout.Padding = new Padding(27, 8, 27, 20);
            Ui.Cols(mainLayout, 100);
            Ui.Rows(mainLayout, 95, 90, 163, 430);
            mainLayout.Height = 825;
            mainLayout.Width = 1400;
            scroll.Controls.Add(mainLayout);
            scroll.Resize += (s, e) => mainLayout.Width = Math.Max(1080, scroll.ClientSize.Width - 2);

            BuildHeader();
            BuildSelectors();
            BuildScoreArea();
            BuildDetails();

            // ComboBox nesnelerini şirket ve profil listelerine bağlayarak seçimleri hazırlarız.
            companyBox.DataSource = companies;
            companyBox.DisplayMember = nameof(Company.DisplayName);
            profileBox.DataSource = service.Profiles;
            profileBox.DisplayMember = nameof(InvestorProfile.Name);
            ShowAnalysis();
        }

        private void BuildHeader()
        {
            var header = Ui.Table(1, 2);
            Ui.Rows(header, 49, 31);
            header.Controls.Add(Ui.Title("Yatırımcı Analizi", 23), 0, 0);
            header.Controls.Add(Ui.Label(
                "Buffett, Lynch ve Graham yaklaşımlarını aynı şirket üzerinde incele.  •  KURGUSAL ŞİRKET VERİLERİ",
                10, color: Ui.Muted), 0, 1);
            mainLayout.Controls.Add(header, 0, 0);
        }

        private void BuildSelectors()
        {
            var root = Ui.Table(4, 2);
            Ui.Cols(root, 37, 27, 16, 20);
            Ui.Rows(root, 23, 42);
            root.Padding = new Padding(0, 8, 0, 3);
            root.Controls.Add(Ui.Label("ŞİRKET SEÇ", 8.9f, true, Ui.Muted), 0, 0);
            root.Controls.Add(Ui.Label("YATIRIMCI PROFİLİ", 8.9f, true, Ui.Muted), 1, 0);

            Ui.StyleCombo(companyBox);
            companyBox.Dock = DockStyle.Fill;
            companyBox.Margin = new Padding(0, 1, 19, 1);
            root.Controls.Add(companyBox, 0, 1);
            Ui.StyleCombo(profileBox);
            profileBox.Dock = DockStyle.Fill;
            profileBox.Margin = new Padding(0, 1, 19, 1);
            root.Controls.Add(profileBox, 1, 1);

            var analyze = Ui.Button("Analiz Et", true);
            analyze.Click += (s, e) => ShowAnalysis();
            var compare = Ui.Button("Profilleri Karşılaştır");
            compare.Click += (s, e) => ShowComparison();
            root.Controls.Add(analyze, 2, 1);
            root.Controls.Add(compare, 3, 1);
            mainLayout.Controls.Add(root, 0, 1);
        }

        private void BuildScoreArea()
        {
            var block = Ui.Table(1, 2);
            Ui.Rows(block, 36, 116);
            block.Controls.Add(Ui.Title("Yatırımcı Profili Uyum Puanları", 13.5f), 0, 0);
            var row = Ui.Table(3);
            Ui.Cols(row, 33.33f, 33.33f, 33.34f);
            for (int i = 0; i < service.Profiles.Count; i++)
            {
                InvestorProfile profile = service.Profiles[i];
                ScoreTile tile = new ScoreTile(profile.Name, i == 0 ? Ui.Green : i == 1
                    ? Color.FromArgb(176, 199, 135) : Ui.Gold);
                tile.Margin = new Padding(i == 0 ? 0 : 15, 0, 18, 0);
                tile.Clicked += () =>
                {
                    profileBox.SelectedItem = profile;
                    ShowAnalysis();
                };
                scoreTiles.Add(profile.Name, tile);
                row.Controls.Add(tile, i, 0);
            }
            block.Controls.Add(row, 0, 1);
            mainLayout.Controls.Add(block, 0, 2);
        }

        private void BuildDetails()
        {
            var lower = Ui.Table(2);
            Ui.Cols(lower, 67, 33);
            lower.Padding = new Padding(0, 12, 0, 0);
            mainLayout.Controls.Add(lower, 0, 3);

            var left = Ui.Table(1, 2);
            Ui.Rows(left, 42, 345);
            left.Margin = new Padding(0, 0, 27, 0);
            left.Controls.Add(analysisTitle, 0, 0);
            dataHost.Dock = DockStyle.Fill;
            dataHost.BackColor = Ui.Background;
            left.Controls.Add(dataHost, 0, 1);

            Ui.StyleGrid(criteriaGrid);
            criteriaGrid.Columns.Add("Criterion", "KRİTER");
            criteriaGrid.Columns.Add("Result", "DURUM");
            criteriaGrid.Columns.Add("Details", "AÇIKLAMA");
            criteriaGrid.Columns[0].FillWeight = 31;
            criteriaGrid.Columns[1].FillWeight = 16;
            criteriaGrid.Columns[2].FillWeight = 53;

            Ui.StyleGrid(comparisonGrid);
            comparisonGrid.Columns.Add("Profile", "PROFİL");
            comparisonGrid.Columns.Add("Score", "PUAN");
            comparisonGrid.Columns.Add("Passed", "UYGUN KRİTER");
            comparisonGrid.Columns.Add("Explanation", "DEĞERLENDİRME");
            comparisonGrid.Columns[0].FillWeight = 22;
            comparisonGrid.Columns[1].FillWeight = 11;
            comparisonGrid.Columns[2].FillWeight = 16;
            comparisonGrid.Columns[3].FillWeight = 51;

            dataHost.Controls.Add(criteriaGrid);
            dataHost.Controls.Add(comparisonGrid);
            comparisonGrid.Hide();
            lower.Controls.Add(left, 0, 0);

            var right = Ui.Table(1, 5);
            Ui.Rows(right, 45, 70, 45, 112, 92);
            right.Padding = new Padding(22, 0, 4, 0);
            right.Controls.Add(Ui.Title("Şirket Özeti", 14), 0, 0);
            right.Controls.Add(companyDetails, 0, 1);
            right.Controls.Add(Ui.Title("Değerlendirme", 14), 0, 2);
            summaryLabel.Padding = new Padding(0, 6, 10, 0);
            summaryLabel.AutoEllipsis = false;
            summaryLabel.TextAlign = ContentAlignment.TopLeft;
            right.Controls.Add(summaryLabel, 0, 3);
            var note = Ui.Label("Puanlar yalnızca seçilen örnek kriterlere uyumu gösterir; kazanç olasılığı değildir.",
                9.3f, color: Ui.Muted);
            note.TextAlign = ContentAlignment.TopLeft;
            note.Padding = new Padding(0, 12, 0, 0);
            note.AutoEllipsis = false;
            right.Controls.Add(note, 0, 4);
            lower.Controls.Add(right, 1, 0);
        }

        // Seçilen şirketi yalnızca seçili yatırımcı profiliyle analiz eder.
        private void ShowAnalysis()
        {
            try
            {
                Company? company = companyBox.SelectedItem as Company;
                InvestorProfile? selectedProfile = profileBox.SelectedItem as InvestorProfile;
                if (company == null || selectedProfile == null) return;

                ProfileAnalysis selected = service.Analyze(company, selectedProfile);
                UpdateScores(company);
                ShowCompany(company);
                // Önce eski satırları temizler, sonra her kriterin sonucunu tabloya ekleriz.
                criteriaGrid.Rows.Clear();
                foreach (CriterionResult criterion in selected.Criteria)
                {
                    int row = criteriaGrid.Rows.Add(criterion.Title, criterion.Status, criterion.Explanation);
                    criteriaGrid.Rows[row].Cells[1].Style.ForeColor = criterion.Passed == true
                        ? Ui.Green : criterion.Passed == false ? Ui.Red : Ui.Gold;
                }
                criteriaGrid.ClearSelection();
                comparisonGrid.Hide();
                criteriaGrid.Show();
                analysisTitle.Text = "Kriter Analizi  /  " + selected.ProfileName;
                summaryLabel.Text = selected.ProfileDescription + Environment.NewLine + Environment.NewLine
                    + "Uyum puanı: " + selected.Score + "/100" + Environment.NewLine + selected.Comment;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Analiz sırasında hata: " + ex.Message, "FINPECT",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Aynı şirket için üç profil sonucunu karşılaştırma tablosuna yazar.
        private void ShowComparison()
        {
            try
            {
                Company? company = companyBox.SelectedItem as Company;
                if (company == null) return;
                List<ProfileAnalysis> analyses = service.Compare(company);
                UpdateScores(company);
                ShowCompany(company);
                comparisonGrid.Rows.Clear();
                foreach (ProfileAnalysis result in analyses)
                {
                    int passed = result.Criteria.Count(c => c.Passed == true);
                    int used = result.Criteria.Count(c => c.Passed.HasValue);
                    comparisonGrid.Rows.Add(result.ProfileName, result.Score + "/100",
                        passed + " / " + used, result.Comment);
                }
                comparisonGrid.ClearSelection();
                criteriaGrid.Hide();
                comparisonGrid.Show();
                analysisTitle.Text = "Profil Karşılaştırması";
                summaryLabel.Text = "En yüksek örnek kriter uyumu: " + analyses[0].ProfileName
                    + " (" + analyses[0].Score + "/100)." + Environment.NewLine + Environment.NewLine
                    + "Bu sonuç, ilgili yatırımcının gerçekten bu hisseyi alacağı anlamına gelmez.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Karşılaştırma sırasında hata: " + ex.Message, "FINPECT",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Üstteki küçük puan göstergelerini analiz servisi sonucuna göre yeniler.
        private void UpdateScores(Company company)
        {
            foreach (ProfileAnalysis result in service.Compare(company))
            {
                if (scoreTiles.TryGetValue(result.ProfileName, out ScoreTile? tile))
                    tile.UpdateScore(result.Score);
            }
        }

        private void ShowCompany(Company company)
        {
            string pe = company.PriceToEarnings?.ToString("0.##") ?? "Veri yok";
            string peg = company.PegRatio?.ToString("0.##") ?? "Veri yok";
            companyDetails.Text = company.DisplayName + "  •  " + company.Sector + Environment.NewLine
                + "Örnek fiyat: " + company.Price.ToString("0.##")
                + "   |   F/K: " + pe + "   |   PEG: " + peg;
        }

        // Tek profilin adını, 100 üzerinden puanını ve doluluk çubuğunu gösteren kontrol.
        private sealed class ScoreTile : UserControl
        {
            private readonly Label score;
            private readonly Panel track;
            private readonly Panel fill;
            private int currentScore;
            public event Action? Clicked;

            public ScoreTile(string name, Color accent)
            {
                Dock = DockStyle.Fill;
                BackColor = Ui.Background;
                var stack = Ui.Table(1, 3);
                Ui.Rows(stack, 33, 47, 19);
                Controls.Add(stack);
                stack.Controls.Add(Ui.Label(name, 12, true), 0, 0);
                score = Ui.Label("— /100", 22, true, accent);
                stack.Controls.Add(score, 0, 1);
                track = new Panel { Dock = DockStyle.Fill, Height = 8,
                    BackColor = Color.FromArgb(40, 49, 49), Margin = new Padding(0, 5, 18, 5) };
                fill = new Panel { BackColor = accent, Height = 8, Width = 1 };
                track.Controls.Add(fill);
                track.Resize += (s, e) => fill.Width = (int)(track.ClientSize.Width * currentScore / 100.0);
                stack.Controls.Add(track, 0, 2);
                // Profil başlığına / skoruna tıklamak seçim yapar.
                foreach (Control control in new Control[] { this, score })
                    control.Click += (s, e) => Clicked?.Invoke();
            }

            public void UpdateScore(int value)
            {
                currentScore = Math.Max(0, Math.Min(100, value));
                score.Text = currentScore + " /100";
                fill.Width = (int)(track.ClientSize.Width * currentScore / 100.0);
            }
        }
    }
}
