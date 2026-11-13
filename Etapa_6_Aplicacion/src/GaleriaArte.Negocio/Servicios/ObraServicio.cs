using System;
using System.Collections.Generic;
using GaleriaArte.Datos.Repositorios;
using GaleriaArte.Entidades;

namespace GaleriaArte.Negocio.Servicios;

public class ObraServicio
{
    private readonly ObraRepositorio _repo = new();
    private readonly RecursoRepositorio _recursos = new();

    public List<Obra> Buscar(FiltroObra? filtro = null) => _repo.Buscar(filtro);

    public Obra? ObtenerPorId(string idObra) => _repo.ObtenerPorId(idObra);

    public void Guardar(Obra o)
    {
        Sesion.ExigirAdministrador();
        o.Nombre = Validar.Requerido(o.Nombre, "Nombre de la obra", 120);
        o.Descripcion = Validar.Requerido(o.Descripcion, "Descripción", 5000);
        Validar.EnLista(o.EstadoConservacion, Constantes.EstadosConservacion, "Estado de conservación");
        // RN-01: artista, categoría y ubicación obligatorios
        Validar.IdSeleccionado(o.IdArtista, "un artista");
        Validar.IdSeleccionado(o.IdCategoria, "una categoría");
        Validar.IdSeleccionado(o.IdUbicacion, "una ubicación");
        if (o.FechaCreacion.HasValue)
        {
            if (o.FechaCreacion.Value.Date > DateTime.Today)
                throw new ValidacionException("La fecha de creación no puede ser futura.");
            if (o.FechaCreacion.Value.Year < 1950)
                throw new ValidacionException("La fecha de creación debe ser posterior a 1950.");
        }

        if (string.IsNullOrEmpty(o.IdObra))
        {
            o.IdObra = Validar.NuevoId();
            o.IdUsuario = Sesion.UsuarioActual!.IdUsuario; // trazabilidad: quién la registró
            _repo.Insertar(o);
        }
        else
        {
            _repo.Actualizar(o);
        }
    }

    /// <summary>
    /// Elimina la obra y sus archivos. Si pertenece a alguna ruta se informa cuál
    /// (la FK ruta_obra → obra es RESTRICT). Los registros de recursos se borran por CASCADE.
    /// </summary>
    public void Eliminar(Obra o)
    {
        Sesion.ExigirAdministrador();
        var rutas = _repo.RutasQueContienen(o.IdObra);
        if (rutas.Count > 0)
            throw new ValidacionException(
                $"La obra forma parte de: {string.Join(", ", rutas)}.\nRetírela primero de esas rutas (módulo Rutas).");

        var archivos = _recursos.ListarPorObra(o.IdObra); // se leen ANTES de borrar
        _repo.Eliminar(o.IdObra);
        foreach (var r in archivos)
        {
            try { GestorArchivos.Eliminar(r.RutaArchivo); }
            catch (Exception) { /* si un archivo está abierto no se impide borrar la obra */ }
        }
    }
}
