using System.Drawing;
using System.Windows.Forms;
using Finpect.WinForms.UI;

namespace Finpect.WinForms.Views
{
    // Diğer grup üyelerinin modülleri hazır olana kadar görünen açık durum ekranı.
    // Bilerek sahte bir çalışır simülasyon veya rapor sunmuyoruz.
    public sealed class PendingModulePage : UserControl
    {
        public PendingModulePage(string heading, string developer, string description)
        {
            Dock = DockStyle.Fill;
            BackColor = Ui.Background;
            var wrapper = Ui.Table(1, 5);
            wrapper.Padding = new Padding(32, 45, 32, 0);
            Ui.Rows(wrapper, 62, 54, 50, 110, 80);
            wrapper.Controls.Add(Ui.Title(heading, 27), 0, 0);
            wrapper.Controls.Add(Ui.Label("Bu modül henüz ana uygulamaya bağlanmadı.",
                13, color: Ui.Muted), 0, 1);
            wrapper.Controls.Add(Ui.Label("Sorumlu: " + developer, 11.5f, true, Ui.Gold), 0, 2);
            var note = Ui.Label(description, 12, color: Ui.Text);
            note.MaximumSize = new Size(760, 100);
            note.AutoEllipsis = false;
            note.TextAlign = ContentAlignment.TopLeft;
            wrapper.Controls.Add(note, 0, 3);
            wrapper.Controls.Add(Ui.Label(
                "Ekip, bu alanı aynı renkler ve yazı stiliyle yeni bir WinForms UserControl ekleyerek tamamlayacak.",
                10, color: Ui.Muted), 0, 4);
            Controls.Add(wrapper);
        }
    }
}
