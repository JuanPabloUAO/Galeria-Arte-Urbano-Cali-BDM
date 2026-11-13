using System.Collections.Generic;
using GaleriaArte.Datos.Repositorios;
using GaleriaArte.Entidades;

namespace GaleriaArte.Negocio.Servicios;

public class UbicacionServicio
{
    // Límites del área de Cali (iguales a los CHECK de la tabla ubicacion, RN-06)
    public const decimal LatitudMin = 3.25m, LatitudMax = 3.55m;
    public const decimal LongitudMin = -76.60m, LongitudMax = -76.45m;

    private readonly UbicacionRepositorio _repo = new();

    public List<Ubicacion> Listar(string? filtro = null) => _repo.Listar(filtro);

    public List<Barrio> ListarBarrios() => _repo.ListarBarrios();

    public List<int> ListarComunas() => _repo.ListarComunasConBarrios();

    public void Guardar(Ubicacion u)
    {
        Sesion.ExigirAdministrador();
        u.NombreLugar = Validar.Requerido(u.NombreLugar, "Nombre del lugar", 100);
        u.Direccion = Validar.Requerido(u.Direccion, "Dirección", 120);
        if (u.IdBarrio <= 0) throw new ValidacionException("Debe seleccionar el barrio.");
        Validar.Rango(u.Latitud, LatitudMin, LatitudMax, "Latitud");
        Validar.Rango(u.Longitud, LongitudMin, LongitudMax, "Longitud");

        if (string.IsNullOrEmpty(u.IdUbicacion))
        {
            u.IdUbicacion = Validar.NuevoId();
            _repo.Insertar(u);
        }
        else
        {
            _repo.Actualizar(u);
        }
    }

    public Barrio CrearBarrio(string? nombre, int comuna)
    {
        Sesion.ExigirAdministrador();
        var barrio = new Barrio { Nombre = Validar.Requerido(nombre, "Nombre del barrio", 60), Comuna = comuna };
        Validar.Rango(comuna, 1, 22, "Comuna");
        barrio.IdBarrio = _repo.InsertarBarrio(barrio);
        return barrio;
    }
}
