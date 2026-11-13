using System;
using GaleriaArte.Entidades;

namespace GaleriaArte.Datos;

/// <summary>Error de base de datos ya traducido a un mensaje que se puede mostrar al usuario.</summary>
public class DatosException : GaleriaException
{
    public DatosException(string mensaje, Exception? interna = null) : base(mensaje, interna) { }
}
