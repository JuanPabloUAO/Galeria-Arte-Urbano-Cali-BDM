using GaleriaArte.Entidades;

namespace GaleriaArte.Negocio;

/// <summary>Dato inválido detectado ANTES de ir a la base de datos. Su mensaje se muestra tal cual al usuario.</summary>
public class ValidacionException : GaleriaException
{
    public ValidacionException(string mensaje) : base(mensaje) { }
}
