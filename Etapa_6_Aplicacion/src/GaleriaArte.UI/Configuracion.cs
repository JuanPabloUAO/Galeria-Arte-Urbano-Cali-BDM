using System;
using System.IO;
using System.Text.Json;

namespace GaleriaArte.UI;

/// <summary>Lee appsettings.json (copiado junto al .exe al compilar).</summary>
internal static class Configuracion
{
    public static string CadenaConexion { get; private set; } = string.Empty;
    public static string? RutaRecursos { get; private set; }

    public static void Cargar()
    {
        var ruta = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        if (!File.Exists(ruta))
            throw new FileNotFoundException("No se encontró appsettings.json junto al ejecutable.");

        using var documento = JsonDocument.Parse(File.ReadAllText(ruta));
        var raiz = documento.RootElement;
        CadenaConexion = raiz.GetProperty("ConnectionStrings").GetProperty("GaleriaDB").GetString() ?? string.Empty;
        if (raiz.TryGetProperty("RutaRecursosMultimedia", out var recursos))
            RutaRecursos = recursos.GetString();
    }
}
