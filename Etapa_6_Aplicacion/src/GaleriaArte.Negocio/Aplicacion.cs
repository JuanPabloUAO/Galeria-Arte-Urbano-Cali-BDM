using GaleriaArte.Datos;

namespace GaleriaArte.Negocio;

/// <summary>Punto de arranque: la interfaz entrega la configuración y la capa de negocio prepara datos y archivos.</summary>
public static class Aplicacion
{
    public static void Iniciar(string cadenaConexion, string? rutaRecursos)
    {
        Conexion.Configurar(cadenaConexion);
        GestorArchivos.Configurar(rutaRecursos);
        Conexion.Probar(); // falla con un mensaje claro si MySQL no responde o la base no existe
    }
}
