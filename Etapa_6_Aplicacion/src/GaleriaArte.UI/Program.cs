using System;
using System.Windows.Forms;
using GaleriaArte.Negocio;
using GaleriaArte.UI.Formularios;
using GaleriaArte.UI.Utilidades;

namespace GaleriaArte.UI;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.ThreadException += (_, e) => Ui.MostrarError(e.Exception);

        // 1. Configuración: cadena de conexión y carpeta de recursos (appsettings.json)
        try
        {
            Configuracion.Cargar();
            Aplicacion.Iniciar(Configuracion.CadenaConexion, Configuracion.RutaRecursos);
        }
        catch (Exception ex)
        {
            MessageBox.Show("No se pudo iniciar la aplicación:\n\n" + ex.Message,
                "Galería de Arte Urbano de Cali", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // 2. Ciclo de sesión: login → ventana principal → (cerrar sesión vuelve al login)
        while (true)
        {
            using (var login = new FrmLogin())
            {
                if (login.ShowDialog() != DialogResult.OK) return;
            }
            using var principal = new FrmPrincipal();
            principal.ShowDialog();
            if (!principal.CerrarSesionSolicitado) return;
            Sesion.Cerrar();
        }
    }
}
