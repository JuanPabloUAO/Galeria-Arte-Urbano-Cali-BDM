using System.Collections.Generic;
using GaleriaArte.Datos.Repositorios;
using GaleriaArte.Entidades;

namespace GaleriaArte.Negocio.Servicios;

public class RutaServicio
{
    private readonly RutaRepositorio _repo = new();

    public List<Ruta> Listar(string? filtro = null) => _repo.Listar(filtro);

    public List<ParadaRuta> ObtenerParadas(string idRuta) => _repo.ObtenerParadas(idRuta);

    public void Guardar(Ruta r)
    {
        Sesion.ExigirAdministrador();
        r.Nombre = Validar.Requerido(r.Nombre, "Nombre de la ruta", 100);
        r.Descripcion = Validar.Requerido(r.Descripcion, "Descripción", 5000);
        if (r.DuracionEstimadaMin <= 0) throw new ValidacionException("La duración estimada debe ser mayor que cero.");
        if (r.DistanciaEstimadaKm <= 0) throw new ValidacionException("La distancia estimada debe ser mayor que cero.");
        Validar.EnLista(r.Estado, Constantes.EstadosRuta, "Estado");

        if (string.IsNullOrEmpty(r.IdRuta))
        {
            r.IdRuta = Validar.NuevoId();
            _repo.Insertar(r);
        }
        else
        {
            _repo.Actualizar(r);
        }
    }

    public void Eliminar(Ruta r)
    {
        Sesion.ExigirAdministrador();
        _repo.Eliminar(r.IdRuta);
    }

    public void AgregarObra(string idRuta, string idObra, int tiempoMin, string? recomendacion)
    {
        Sesion.ExigirAdministrador();
        Validar.IdSeleccionado(idRuta, "una ruta");
        Validar.IdSeleccionado(idObra, "una obra");
        if (tiempoMin <= 0) throw new ValidacionException("El tiempo de permanencia debe ser mayor que cero.");
        _repo.AgregarObra(idRuta, idObra, tiempoMin, Validar.Opcional(recomendacion, "Recomendación", 2000));
    }

    public void RetirarObra(ParadaRuta p)
    {
        Sesion.ExigirAdministrador();
        _repo.RetirarObra(p.IdRuta, p.IdObra);
    }

    /// <summary>Sube (-1) o baja (+1) una parada en el recorrido.</summary>
    public void Mover(ParadaRuta p, int desplazamiento, int totalParadas)
    {
        Sesion.ExigirAdministrador();
        int nuevo = p.OrdenVisita + desplazamiento;
        if (nuevo < 1 || nuevo > totalParadas) return; // ya está en el extremo
        _repo.MoverObra(p.IdRuta, p.IdObra, nuevo);
    }

    public void ActualizarParada(ParadaRuta p)
    {
        Sesion.ExigirAdministrador();
        if (p.TiempoEstimadoMin <= 0) throw new ValidacionException("El tiempo de permanencia debe ser mayor que cero.");
        _repo.ActualizarParada(p.IdRutaObra, p.TiempoEstimadoMin, Validar.Opcional(p.Recomendacion, "Recomendación", 2000));
    }
}
