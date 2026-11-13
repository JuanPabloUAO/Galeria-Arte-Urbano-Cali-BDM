using System;

namespace GaleriaArte.Entidades;

/// <summary>
/// Excepción base de la aplicación: su mensaje ya está redactado para mostrarse al usuario.
/// De ella heredan ValidacionException (capa de negocio) y DatosException (capa de datos),
/// así la interfaz las reconoce sin depender de la capa de datos.
/// </summary>
public class GaleriaException : Exception
{
    public GaleriaException(string mensaje, Exception? interna = null) : base(mensaje, interna) { }
}
