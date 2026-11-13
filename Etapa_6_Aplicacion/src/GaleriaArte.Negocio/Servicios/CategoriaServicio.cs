using System.Collections.Generic;
using GaleriaArte.Datos.Repositorios;
using GaleriaArte.Entidades;

namespace GaleriaArte.Negocio.Servicios;

public class CategoriaServicio
{
    private readonly CategoriaRepositorio _repo = new();

    public List<Categoria> Listar(string? filtro = null) => _repo.Listar(filtro);

    public List<Categoria> ListarActivas() => _repo.Listar(null, soloActivas: true);

    public void Guardar(Categoria c)
    {
        Sesion.ExigirAdministrador();
        c.Nombre = Validar.Requerido(c.Nombre, "Nombre", 50);
        c.Descripcion = Validar.Opcional(c.Descripcion, "Descripción", 2000);
        Validar.EnLista(c.Estado, Constantes.EstadosBasicos, "Estado");

        if (string.IsNullOrEmpty(c.IdCategoria))
        {
            c.IdCategoria = Validar.NuevoId();
            _repo.Insertar(c);
        }
        else
        {
            _repo.Actualizar(c);
        }
    }

    /// <summary>Alterna ACTIVO ↔ INACTIVO. Las categorías no se borran porque pueden tener obras.</summary>
    public string CambiarEstado(Categoria c)
    {
        Sesion.ExigirAdministrador();
        var nuevo = c.Estado == Constantes.Activo ? Constantes.Inactivo : Constantes.Activo;
        _repo.CambiarEstado(c.IdCategoria, nuevo);
        return nuevo;
    }
}
