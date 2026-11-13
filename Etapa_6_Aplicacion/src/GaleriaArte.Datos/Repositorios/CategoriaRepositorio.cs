using System.Collections.Generic;
using System.Data;
using GaleriaArte.Entidades;

namespace GaleriaArte.Datos.Repositorios;

public class CategoriaRepositorio : RepositorioBase
{
    private static Categoria Mapear(IDataRecord r) => new()
    {
        IdCategoria = Texto(r, "id_categoria"),
        Nombre = Texto(r, "nombre"),
        Descripcion = TextoNulo(r, "descripcion"),
        Estado = Texto(r, "estado"),
        TotalObras = Entero(r, "total_obras"),
    };

    public List<Categoria> Listar(string? filtro = null, bool soloActivas = false) => Consultar(
        @"SELECT c.id_categoria, c.nombre, c.descripcion, c.estado,
                 (SELECT COUNT(*) FROM obra o WHERE o.id_categoriaFK = c.id_categoria) AS total_obras
          FROM categoria c
          WHERE (@filtro IS NULL OR c.nombre LIKE CONCAT('%', @filtro, '%'))
            AND (@soloActivas = 0 OR c.estado = 'ACTIVO')
          ORDER BY c.nombre",
        Mapear, P("@filtro", string.IsNullOrWhiteSpace(filtro) ? null : filtro.Trim()), P("@soloActivas", soloActivas ? 1 : 0));

    public void Insertar(Categoria c) => EjecutarComando(
        "INSERT INTO categoria (id_categoria, nombre, descripcion, estado) VALUES (@id, @nombre, @desc, @estado)",
        P("@id", c.IdCategoria), P("@nombre", c.Nombre), P("@desc", c.Descripcion), P("@estado", c.Estado));

    public void Actualizar(Categoria c) => EjecutarComando(
        "UPDATE categoria SET nombre = @nombre, descripcion = @desc, estado = @estado WHERE id_categoria = @id",
        P("@nombre", c.Nombre), P("@desc", c.Descripcion), P("@estado", c.Estado), P("@id", c.IdCategoria));

    public void CambiarEstado(string idCategoria, string estado) => EjecutarComando(
        "UPDATE categoria SET estado = @estado WHERE id_categoria = @id", P("@estado", estado), P("@id", idCategoria));
}
