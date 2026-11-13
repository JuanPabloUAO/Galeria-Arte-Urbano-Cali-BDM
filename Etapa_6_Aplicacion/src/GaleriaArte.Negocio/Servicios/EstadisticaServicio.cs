using System.Collections.Generic;
using GaleriaArte.Datos.Repositorios;
using GaleriaArte.Entidades;

namespace GaleriaArte.Negocio.Servicios;

public class EstadisticaServicio
{
    private readonly EstadisticaRepositorio _repo = new();

    public static readonly string[] Disponibles =
    {
        "Obras por comuna",
        "Obras por categoría",
        "Artistas con más de N obras",
        "Recursos multimedia por tipo",
        "Obras por estado de conservación",
    };

    public List<Estadistica> Obtener(int indice, int n = 1) => indice switch
    {
        0 => _repo.ObrasPorComuna(),
        1 => _repo.ObrasPorCategoria(),
        2 => _repo.ArtistasConMasDe(n < 0 ? 0 : n),
        3 => _repo.RecursosPorTipo(),
        _ => _repo.ObrasPorEstadoConservacion(),
    };
}
