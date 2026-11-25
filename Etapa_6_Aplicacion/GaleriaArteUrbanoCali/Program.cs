using System;
using System.Windows.Forms;
using GaleriaArteUrbanoCali.views;

namespace GaleriaArteUrbanoCali
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            using var login = new UI_Login();
            if (login.ShowDialog() == DialogResult.OK)
            {
                Application.Run(new FormPrincipal());
            }
        }
    }
}
