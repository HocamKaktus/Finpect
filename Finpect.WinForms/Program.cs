using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace Finpect.WinForms
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Sayıların ve açıklamaların Türkçe görüntülenmesi.
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("tr-TR");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
