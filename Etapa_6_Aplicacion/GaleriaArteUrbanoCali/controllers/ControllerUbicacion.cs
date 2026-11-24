using System;
using System.Collections.Generic;
using System.Data;
using GaleriaArteUrbanoCali.models;

namespace GaleriaArteUrbanoCali.controllers
{
    public class ControllerUbicacion
    {
        private readonly Ubicacion _modeloUbicacion = new Ubicacion();
        private readonly Comuna _modeloComuna = new Comuna();
        private readonly Barrio _modeloBarrio = new Barrio();

        public List<Comuna> ListarComunas()
        {
            return _modeloComuna.GetComunas();
        }

        public List<Barrio> ListarBarrios()
        {
            return _modeloBarrio.GetBarrios();
        }

        public List<Barrio> ListarBarriosPorComuna(string idComuna)
        {
            if (string.IsNullOrWhiteSpace(idComuna))
            {
                return _modeloBarrio.GetBarrios();
            }
            return _modeloBarrio.GetBarriosPorComuna(idComuna);
        }

        public List<Ubicacion> ListarUbicaciones()
        {
            return _modeloUbicacion.GetUbicaciones();
        }

        public DataTable ConsultarTablaUbicaciones()
        {
            return _modeloUbicacion.ConsultarTablaUbicaciones();
        }

        public Ubicacion? BuscarUbicacion(string idUbicacion)
        {
            if (string.IsNullOrWhiteSpace(idUbicacion))
            {
                return null;
            }
            return _modeloUbicacion.BuscarPorId(idUbicacion);
        }

        public bool ValidarCoordenadasCali(decimal latitud, decimal longitud, out string mensaje)
        {
            if (latitud < 3.25000000m || latitud > 3.55000000m)
            {
                mensaje = "La latitud debe estar dentro del rango geográfico de Cali: [3.25000000 a 3.55000000].";
                return false;
            }

            if (longitud < -76.60000000m || longitud > -76.45000000m)
            {
                mensaje = "La longitud debe estar dentro del rango geográfico de Cali: [-76.60000000 a -76.45000000].";
                return false;
            }

            mensaje = string.Empty;
            return true;
        }

        public bool InsertarUbicacion(Ubicacion u, out string mensaje)
        {
            if (string.IsNullOrWhiteSpace(u.NombreLugar) ||
                string.IsNullOrWhiteSpace(u.Direccion) ||
                string.IsNullOrWhiteSpace(u.IdBarrioFK))
            {
                mensaje = "El nombre del lugar, la dirección y el barrio son obligatorios.";
                return false;
            }

            if (!ValidarCoordenadasCali(u.Latitud, u.Longitud, out mensaje))
            {
                return false;
            }

            return _modeloUbicacion.InsertarUbicacion(u, out mensaje);
        }

        public bool ActualizarUbicacion(Ubicacion u, out string mensaje)
        {
            if (string.IsNullOrWhiteSpace(u.IdUbicacion) ||
                string.IsNullOrWhiteSpace(u.NombreLugar) ||
                string.IsNullOrWhiteSpace(u.Direccion) ||
                string.IsNullOrWhiteSpace(u.IdBarrioFK))
            {
                mensaje = "Seleccione una ubicación válida y complete todos los campos obligatorios.";
                return false;
            }

            if (!ValidarCoordenadasCali(u.Latitud, u.Longitud, out mensaje))
            {
                return false;
            }

            return _modeloUbicacion.ActualizarUbicacion(u, out mensaje);
        }
    }
}
