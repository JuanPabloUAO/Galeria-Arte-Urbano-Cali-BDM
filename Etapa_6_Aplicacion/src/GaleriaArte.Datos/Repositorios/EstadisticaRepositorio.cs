using System.Collections.Generic;
using System.Data;
using GaleriaArte.Entidades;

namespace GaleriaArte.Datos.Repositorios;

/// <summary>Consultas con agregación (GROUP BY, COUNT, SUM, HAVING) para el módulo de estadísticas.</summary>
public class EstadisticaRepositorio : RepositorioBase
{
    private static Estadistica Mapear(IDataRecord r) => new()
    {
        Etiqueta = Texto(r, "etiqueta"),
        Valor = Numero(r, "valor"),
        Detalle = TextoNulo(r, "detalle"),
    };

    public List<Estadistica> ObrasPorComuna() => Consultar(
        @"SELECT CONCAT('Comuna ', b.comuna) AS etiqueta, COUNT(o.id_obra) AS valor,
                 GROUP_CONCAT(DISTINCT b.nombre ORDER BY b.nombre SEPARATOR ', ') AS detalle
          FROM barrio b
          INNER JOIN ubicacion u ON u.id_barrioFK = b.id_barrio
          INNER JOIN obra o ON o.id_ubicacionFK = u.id_ubicacion
          GROUP BY b.comuna
          ORDER BY valor DESC, b.comuna", Mapear);

    public List<Estadistica> ObrasPorCategoria() => Consultar(
        @"SELECT c.nombre AS etiqueta, COUNT(o.id_obra) AS valor, c.estado AS detalle
          FROM categoria c
          LEFT JOIN obra o ON o.id_categoriaFK = c.id_categoria
          GROUP BY c.id_categoria, c.nombre, c.estado
          ORDER BY valor DESC, etiqueta", Mapear);

    /// <summary>Artistas con MÁS de <paramref name="minimo"/> obras (HAVING).</summary>
    public List<Estadistica> ArtistasConMasDe(int minimo) => Consultar(
        @"SELECT a.nombre_artistico AS etiqueta, COUNT(o.id_obra) AS valor, a.pais_origen AS detalle
          FROM artista a
          INNER JOIN obra o ON o.id_artistaFK = a.id_artista
          GROUP BY a.id_artista, a.nombre_artistico, a.pais_origen
          HAVING COUNT(o.id_obra) > @minimo
          ORDER BY valor DESC, etiqueta", Mapear, P("@minimo", minimo));

    public List<Estadistica> RecursosPorTipo() => Consultar(
        @"SELECT f.tipo_recurso AS etiqueta, COUNT(*) AS valor,
                 CONCAT(ROUND(SUM(rm.tamano_bytes) / 1048576, 2), ' MB') AS detalle
          FROM recurso_multimedia rm
          INNER JOIN formato_multimedia f ON f.codigo_formato = rm.codigo_formatoFK
          GROUP BY f.tipo_recurso
          ORDER BY valor DESC", Mapear);

    public List<Estadistica> ObrasPorEstadoConservacion() => Consultar(
        @"SELECT estado_conservacion AS etiqueta, COUNT(*) AS valor, NULL AS detalle
          FROM obra
          GROUP BY estado_conservacion
          ORDER BY FIELD(estado_conservacion, 'EXCELENTE', 'BUENO', 'DETERIORADO', 'DESTRUIDO')", Mapear);
}
