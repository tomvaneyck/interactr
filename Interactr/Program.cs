using System;
using System.Windows.Forms;
using Interactr.View;

namespace Interactr
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            MainWindow window = new MainWindow();
            window.Show();
        }
    }
}
