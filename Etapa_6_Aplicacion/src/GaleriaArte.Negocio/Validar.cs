using System;
using System.Text.RegularExpressions;

namespace GaleriaArte.Negocio;

/// <summary>
/// Validaciones reutilizables. Repiten (en C#) las reglas de la base de datos para
/// avisar al usuario con un mensaje claro antes de intentar guardar.
/// </summary>
internal static class Validar
{
    /// <summary>Exige un texto no vacío de máximo <paramref name="maximo"/> caracteres y lo devuelve sin espacios sobrantes.</summary>
    public static string Requerido(string? valor, string campo, int maximo)
    {
        var texto = (valor ?? string.Empty).Trim();
        if (texto.Length == 0)
            throw new ValidacionException($"El campo '{campo}' es obligatorio.");
        if (texto.Length > maximo)
            throw new ValidacionException($"El campo '{campo}' admite máximo {maximo} caracteres.");
        return texto;
    }

    /// <summary>Texto opcional: si viene vacío devuelve null (se guarda NULL en MySQL).</summary>
    public static string? Opcional(string? valor, string campo, int maximo)
    {
        var texto = (valor ?? string.Empty).Trim();
        if (texto.Length == 0) return null;
        if (texto.Length > maximo)
            throw new ValidacionException($"El campo '{campo}' admite máximo {maximo} caracteres.");
        return texto;
    }

    public static string Correo(string? valor)
    {
        var correo = Requerido(valor, "Correo", 100).ToLowerInvariant();
        if (!Regex.IsMatch(correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new ValidacionException("El correo no tiene un formato válido (ejemplo: nombre@dominio.com).");
        return correo;
    }

    public static void EnLista(string valor, string[] permitidos, string campo)
    {
        if (Array.IndexOf(permitidos, valor) < 0)
            throw new ValidacionException($"El valor de '{campo}' no es válido.");
    }

    public static void Rango(decimal valor, decimal minimo, decimal maximo, string campo)
    {
        if (valor < minimo || valor > maximo)
            throw new ValidacionException($"'{campo}' debe estar entre {minimo} y {maximo}.");
    }

    public static void IdSeleccionado(string? id, string campo)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidacionException($"Debe seleccionar {campo}.");
    }

    public static string NuevoId() => Guid.NewGuid().ToString();
}
