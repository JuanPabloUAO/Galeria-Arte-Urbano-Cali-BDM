namespace GaleriaArte.Entidades;

/// <summary>
/// Valores permitidos por los CHECK de la base de datos.
/// Se centralizan aquí para que los combos de la interfaz y las validaciones
/// usen exactamente los mismos textos que acepta MySQL.
/// </summary>
public static class Constantes
{
    public const string Activo = "ACTIVO";
    public const string Inactivo = "INACTIVO";
    public const string EnMantenimiento = "EN_MANTENIMIENTO";

    public const string Administrador = "ADMINISTRADOR";
    public const string Visitante = "VISITANTE";

    public static readonly string[] EstadosBasicos = { Activo, Inactivo };
    public static readonly string[] EstadosRuta = { Activo, Inactivo, EnMantenimiento };
    public static readonly string[] TiposUsuario = { Administrador, Visitante };
    public static readonly string[] EstadosConservacion = { "EXCELENTE", "BUENO", "DETERIORADO", "DESTRUIDO" };

    // Tipos de recurso (tabla formato_multimedia)
    public const string TipoImagen = "IMAGEN";
    public const string TipoVideo = "VIDEO";
    public const string TipoAudio = "AUDIO";
    public const string TipoModelo3D = "MODELO_3D";
    public const string TipoAR = "AR";
}
