using System.Collections.Generic;
using GaleriaArte.Datos.Repositorios;
using GaleriaArte.Entidades;

namespace GaleriaArte.Negocio.Servicios;

public class ArtistaServicio
{
    private readonly ArtistaRepositorio _repo = new();

    public List<Artista> Listar(string? filtro = null) => _repo.Listar(filtro);

    public List<Artista> ListarActivos() => _repo.Listar(null, soloActivos: true);

    public void Guardar(Artista a)
    {
        Sesion.ExigirAdministrador();
        a.NombreArtistico = Validar.Requerido(a.NombreArtistico, "Nombre artístico", 100);
        a.NombreReal = Validar.Opcional(a.NombreReal, "Nombre real", 100);
        a.Biografia = Validar.Opcional(a.Biografia, "Biografía", 5000);
        a.PaisOrigen = Validar.Requerido(a.PaisOrigen, "País de origen", 60);
        a.RedSocial = Validar.Opcional(a.RedSocial, "Red social", 150);
        if (a.RedSocial != null && !a.RedSocial.StartsWith("http") && !a.RedSocial.StartsWith("@"))
            throw new ValidacionException("La red social debe ser un enlace (https://...) o un usuario (@usuario).");
        Validar.EnLista(a.Estado, Constantes.EstadosBasicos, "Estado");

        if (string.IsNullOrEmpty(a.IdArtista))
        {
            a.IdArtista = Validar.NuevoId();
            _repo.Insertar(a);
        }
        else
        {
            _repo.Actualizar(a);
        }
    }

    /// <summary>RN-02: si tiene obras se desactiva, si no se elimina. Devuelve un mensaje para el usuario.</summary>
    public string EliminarODesactivar(Artista a)
    {
        Sesion.ExigirAdministrador();
        var resultado = _repo.EliminarODesactivar(a.IdArtista);
        return resultado == "ELIMINADO"
            ? $"El artista '{a.NombreArtistico}' no tenía obras y fue eliminado."
            : $"El artista '{a.NombreArtistico}' tiene obras registradas, por eso se DESACTIVÓ en lugar de eliminarse.";
    }
}
