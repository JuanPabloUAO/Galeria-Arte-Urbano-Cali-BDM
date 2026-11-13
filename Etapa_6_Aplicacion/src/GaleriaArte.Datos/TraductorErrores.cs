using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace GaleriaArte.Datos;

/// <summary>
/// Convierte los códigos de error de MySQL en mensajes en español.
/// Gracias a que todas las restricciones tienen nombre (uq_..., ck_..., fk_...),
/// se puede decir exactamente qué regla se incumplió.
/// </summary>
internal static class TraductorErrores
{
    private static readonly Dictionary<string, string> Mensajes = new()
    {
        // UNIQUE
        ["uq_usuario_correo"] = "Ya existe un usuario con ese correo.",
        ["uq_artista_nombre"] = "Ya existe un artista con ese nombre artístico.",
        ["uq_categoria_nombre"] = "Ya existe una categoría con ese nombre.",
        ["uq_barrio_nombre"] = "Ya existe un barrio con ese nombre.",
        ["uq_ubicacion_lugar_dir"] = "Ya existe una ubicación con ese nombre y dirección.",
        ["uq_obra_ubicacion_nombre"] = "Ya existe una obra con ese nombre en esa ubicación.",
        ["uq_recurso_ruta"] = "Ya hay un recurso registrado con ese archivo.",
        ["uq_ruta_nombre"] = "Ya existe una ruta con ese nombre.",
        ["uq_ruta_obra_obra"] = "La obra ya forma parte de esta ruta.",
        ["uq_ruta_obra_orden"] = "Ya hay una obra en esa posición de la ruta.",
        // CHECK
        ["ck_usuario_correo"] = "El correo no tiene un formato válido.",
        ["ck_usuario_hash"] = "La contraseña debe guardarse cifrada (hash BCrypt).",
        ["ck_ubicacion_latitud"] = "La latitud debe estar entre 3.25 y 3.55 (área de Cali).",
        ["ck_ubicacion_longitud"] = "La longitud debe estar entre -76.60 y -76.45 (área de Cali).",
        ["ck_barrio_comuna"] = "La comuna debe estar entre 1 y 22.",
        ["ck_obra_fechas"] = "La fecha de creación no es válida (debe ser posterior a 1950 y no futura).",
        ["ck_recurso_ruta_rel"] = "La ruta del archivo debe ser relativa a la carpeta RecursosMultimedia.",
        ["ck_recurso_tamano"] = "El tamaño del archivo debe ser mayor que cero.",
        ["ck_recurso_duracion"] = "La duración debe ser mayor que cero.",
        ["ck_ruta_duracion"] = "La duración de la ruta debe ser mayor que cero.",
        ["ck_ruta_distancia"] = "La distancia de la ruta debe ser mayor que cero.",
        ["ck_artista_red"] = "La red social debe ser un enlace (http...) o un usuario (@...).",
    };

    public static string Traducir(MySqlException ex)
    {
        switch (ex.Number)
        {
            case 0:
            case 1042:
                return "No se pudo conectar con el servidor MySQL. Verifique que el servicio esté iniciado y la cadena de conexión en appsettings.json.";
            case 1045:
                return "Usuario o contraseña de MySQL incorrectos. Revise la cadena de conexión en appsettings.json.";
            case 1049:
                return "La base de datos 'galeria_arte_urbano_cali' no existe. Ejecute primero los scripts SQL de la Etapa 5.";
            case 1062: // UNIQUE
            case 3819: // CHECK
                foreach (var par in Mensajes)
                    if (ex.Message.Contains(par.Key))
                        return par.Value;
                return ex.Number == 1062 ? "El registro ya existe (dato duplicado)." : "Algún dato no cumple las reglas de la base de datos.";
            case 1451:
                return "No se puede eliminar: el registro tiene información asociada (por ejemplo, obras o rutas que dependen de él).";
            case 1452:
                return "El registro relacionado (artista, categoría, ubicación, formato...) no existe.";
            case 1644: // SIGNAL de los procedimientos almacenados: el mensaje ya viene en español
                return ex.Message;
            case 1406:
                return "Uno de los textos es demasiado largo para el campo.";
            case 1048:
                return "Falta un dato obligatorio.";
            default:
                return $"Error de base de datos ({ex.Number}): {ex.Message}";
        }
    }
}
