using System.Collections.Generic;
using System.Data;
using GaleriaArte.Entidades;

namespace GaleriaArte.Datos.Repositorios;

public class RutaRepositorio : RepositorioBase
{
    private static Ruta Mapear(IDataRecord r) => new()
    {
        IdRuta = Texto(r, "id_ruta"),
        Nombre = Texto(r, "nombre"),
        Descripcion = Texto(r, "descripcion"),
        DuracionEstimadaMin = Entero(r, "duracion_estimada_min"),
        DistanciaEstimadaKm = Numero(r, "distancia_estimada_km"),
        Estado = Texto(r, "estado"),
        TotalObras = Entero(r, "total_obras"),
    };

    public List<Ruta> Listar(string? filtro = null) => Consultar(
        @"SELECT r.id_ruta, r.nombre, r.descripcion, r.duracion_estimada_min, r.distancia_estimada_km, r.estado,
                 COUNT(ro.id_ruta_obra) AS total_obras
          FROM ruta r
          LEFT JOIN ruta_obra ro ON ro.id_rutaFK = r.id_ruta
          WHERE (@filtro IS NULL OR r.nombre LIKE CONCAT('%', @filtro, '%'))
          GROUP BY r.id_ruta, r.nombre, r.descripcion, r.duracion_estimada_min, r.distancia_estimada_km, r.estado
          ORDER BY r.nombre",
        Mapear, P("@filtro", string.IsNullOrWhiteSpace(filtro) ? null : filtro.Trim()));

    public void Insertar(Ruta r) => EjecutarComando(
        @"INSERT INTO ruta (id_ruta, nombre, descripcion, duracion_estimada_min, distancia_estimada_km, estado)
          VALUES (@id, @nombre, @desc, @duracion, @distancia, @estado)",
        P("@id", r.IdRuta), P("@nombre", r.Nombre), P("@desc", r.Descripcion),
        P("@duracion", r.DuracionEstimadaMin), P("@distancia", r.DistanciaEstimadaKm), P("@estado", r.Estado));

    public void Actualizar(Ruta r) => EjecutarComando(
        @"UPDATE ruta SET nombre = @nombre, descripcion = @desc, duracion_estimada_min = @duracion,
                 distancia_estimada_km = @distancia, estado = @estado
          WHERE id_ruta = @id",
        P("@nombre", r.Nombre), P("@desc", r.Descripcion), P("@duracion", r.DuracionEstimadaMin),
        P("@distancia", r.DistanciaEstimadaKm), P("@estado", r.Estado), P("@id", r.IdRuta));

    /// <summary>Elimina la ruta; sus paradas se borran por ON DELETE CASCADE. Las obras no se tocan.</summary>
    public void Eliminar(string idRuta) =>
        EjecutarComando("DELETE FROM ruta WHERE id_ruta = @id", P("@id", idRuta));

    // ---------- Paradas (tabla asociativa ruta_obra) ----------

    /// <summary>Consulta requerida: obras de la ruta en orden, con su ubicación (vista vw_ruta_detalle).</summary>
    public List<ParadaRuta> ObtenerParadas(string idRuta) => Consultar(
        @"SELECT id_ruta_obra, id_ruta, ruta, id_obra, orden_visita, obra, artista, lugar, direccion, barrio, comuna,
                 tiempo_estimado_min, recomendacion
          FROM vw_ruta_detalle
          WHERE id_ruta = @ruta
          ORDER BY orden_visita",
        r => new ParadaRuta
        {
            IdRutaObra = Texto(r, "id_ruta_obra"),
            IdRuta = Texto(r, "id_ruta"),
            Ruta = Texto(r, "ruta"),
            IdObra = Texto(r, "id_obra"),
            OrdenVisita = Entero(r, "orden_visita"),
            Obra = Texto(r, "obra"),
            Artista = Texto(r, "artista"),
            Lugar = Texto(r, "lugar"),
            Direccion = Texto(r, "direccion"),
            Barrio = Texto(r, "barrio"),
            Comuna = Entero(r, "comuna"),
            TiempoEstimadoMin = Entero(r, "tiempo_estimado_min"),
            Recomendacion = TextoNulo(r, "recomendacion"),
        },
        P("@ruta", idRuta));

    /// <summary>Procedimiento sp_agregar_obra_a_ruta: agrega al final calculando el orden.</summary>
    public void AgregarObra(string idRuta, string idObra, int tiempoMin, string? recomendacion) =>
        EjecutarProcedimiento("sp_agregar_obra_a_ruta",
            P("@p_id_ruta", idRuta), P("@p_id_obra", idObra), P("@p_tiempo_min", tiempoMin), P("@p_recomendacion", recomendacion));

    /// <summary>Procedimiento sp_retirar_obra_de_ruta: retira y compacta el orden.</summary>
    public void RetirarObra(string idRuta, string idObra) =>
        EjecutarProcedimiento("sp_retirar_obra_de_ruta", P("@p_id_ruta", idRuta), P("@p_id_obra", idObra));

    /// <summary>Procedimiento sp_mover_obra_en_ruta: cambia la posición y desplaza las demás.</summary>
    public void MoverObra(string idRuta, string idObra, int nuevoOrden) =>
        EjecutarProcedimiento("sp_mover_obra_en_ruta", P("@p_id_ruta", idRuta), P("@p_id_obra", idObra), P("@p_nuevo_orden", nuevoOrden));

    public void ActualizarParada(string idRutaObra, int tiempoMin, string? recomendacion) => EjecutarComando(
        "UPDATE ruta_obra SET tiempo_estimado_min = @tiempo, recomendacion = @rec WHERE id_ruta_obra = @id",
        P("@tiempo", tiempoMin), P("@rec", recomendacion), P("@id", idRutaObra));
}
