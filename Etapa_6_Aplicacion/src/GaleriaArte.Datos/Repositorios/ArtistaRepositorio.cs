using System.Collections.Generic;
using System.Data;
using GaleriaArte.Entidades;
using MySql.Data.MySqlClient;

namespace GaleriaArte.Datos.Repositorios;

public class ArtistaRepositorio : RepositorioBase
{
    private static Artista Mapear(IDataRecord r) => new()
    {
        IdArtista = Texto(r, "id_artista"),
        NombreArtistico = Texto(r, "nombre_artistico"),
        NombreReal = TextoNulo(r, "nombre_real"),
        Biografia = TextoNulo(r, "biografia"),
        PaisOrigen = Texto(r, "pais_origen"),
        RedSocial = TextoNulo(r, "red_social"),
        Estado = Texto(r, "estado"),
        TotalObras = Entero(r, "total_obras"),
    };

    /// <summary>Lista artistas filtrando por nombre artístico o real. filtro vacío = todos.</summary>
    public List<Artista> Listar(string? filtro = null, bool soloActivos = false) => Consultar(
        @"SELECT a.id_artista, a.nombre_artistico, a.nombre_real, a.biografia, a.pais_origen, a.red_social, a.estado,
                 (SELECT COUNT(*) FROM obra o WHERE o.id_artistaFK = a.id_artista) AS total_obras
          FROM artista a
          WHERE (@filtro IS NULL OR a.nombre_artistico LIKE CONCAT('%', @filtro, '%') OR a.nombre_real LIKE CONCAT('%', @filtro, '%'))
            AND (@soloActivos = 0 OR a.estado = 'ACTIVO')
          ORDER BY a.nombre_artistico",
        Mapear, P("@filtro", string.IsNullOrWhiteSpace(filtro) ? null : filtro.Trim()), P("@soloActivos", soloActivos ? 1 : 0));

    public void Insertar(Artista a) => EjecutarComando(
        @"INSERT INTO artista (id_artista, nombre_artistico, nombre_real, biografia, pais_origen, red_social, estado)
          VALUES (@id, @nombreArt, @nombreReal, @bio, @pais, @red, @estado)",
        P("@id", a.IdArtista), P("@nombreArt", a.NombreArtistico), P("@nombreReal", a.NombreReal), P("@bio", a.Biografia),
        P("@pais", a.PaisOrigen), P("@red", a.RedSocial), P("@estado", a.Estado));

    public void Actualizar(Artista a) => EjecutarComando(
        @"UPDATE artista SET nombre_artistico = @nombreArt, nombre_real = @nombreReal, biografia = @bio,
                 pais_origen = @pais, red_social = @red, estado = @estado
          WHERE id_artista = @id",
        P("@nombreArt", a.NombreArtistico), P("@nombreReal", a.NombreReal), P("@bio", a.Biografia),
        P("@pais", a.PaisOrigen), P("@red", a.RedSocial), P("@estado", a.Estado), P("@id", a.IdArtista));

    /// <summary>
    /// Llama al procedimiento sp_eliminar_o_desactivar_artista (RN-02).
    /// Devuelve "ELIMINADO" o "DESACTIVADO".
    /// </summary>
    public string EliminarODesactivar(string idArtista)
    {
        var resultado = new MySqlParameter("@p_resultado", MySqlDbType.VarChar, 15) { Direction = ParameterDirection.Output };
        EjecutarProcedimiento("sp_eliminar_o_desactivar_artista", P("@p_id_artista", idArtista), resultado);
        return System.Convert.ToString(resultado.Value) ?? string.Empty;
    }
}
