using System;
using System.Collections.Generic;
using System.Data;
using GaleriaArteUrbanoCali.models;

namespace GaleriaArteUrbanoCali.controllers
{
    public class ControllerRuta
    {
        private readonly Ruta _modeloRuta = new Ruta();
        private readonly RutaObra _modeloRutaObra = new RutaObra();

        public List<Ruta> ListarRutas()
        {
            return _modeloRuta.GetRutas();
        }

        public DataTable ConsultarTablaRutas()
        {
            return _modeloRuta.ConsultarTablaRutas();
        }

        public DataTable ConsultarParadasPorRuta(string idRuta)
        {
            if (string.IsNullOrWhiteSpace(idRuta))
            {
                return new DataTable();
            }
            return _modeloRutaObra.ConsultarParadasPorRuta(idRuta);
        }

        public bool CrearRuta(Ruta r, out string mensaje)
        {
            if (string.IsNullOrWhiteSpace(r.NombreRuta) || r.DuracionEstimada <= 0 || r.DistanciaEstimada <= 0)
            {
                mensaje = "El nombre de la ruta, la duración estimada (> 0) y la distancia (> 0) son obligatorios.";
                return false;
            }

            return _modeloRuta.InsertarRuta(r, out mensaje);
        }

        public bool AgregarObraARuta(RutaObra ro, out string mensaje)
        {
            if (string.IsNullOrWhiteSpace(ro.IdRutaFK) ||
                string.IsNullOrWhiteSpace(ro.IdObraFK) ||
                ro.OrdenVisita <= 0 ||
                ro.TiempoPermanencia <= 0)
            {
                mensaje = "Seleccione la ruta, la obra, un orden de visita válido (> 0) y tiempo de permanencia (> 0).";
                return false;
            }

            return _modeloRutaObra.AgregarObraARuta(ro, out mensaje);
        }

        public bool ActualizarOrdenParada(string idRutaObra, string idRuta, int nuevoOrden, int nuevoTiempo, string? recomendacion, out string mensaje)
        {
            if (string.IsNullOrWhiteSpace(idRutaObra) || string.IsNullOrWhiteSpace(idRuta) || nuevoOrden <= 0 || nuevoTiempo <= 0)
            {
                mensaje = "Seleccione una parada válida e indique un orden (> 0) y tiempo (> 0).";
                return false;
            }
            return _modeloRutaObra.ActualizarOrdenParada(idRutaObra, idRuta, nuevoOrden, nuevoTiempo, recomendacion, out mensaje);
        }

        public bool RetirarParadaDeRuta(string idRutaObra, out string mensaje)
        {
            if (string.IsNullOrWhiteSpace(idRutaObra))
            {
                mensaje = "Seleccione una parada válida de la ruta.";
                return false;
            }
            return _modeloRutaObra.RetirarParadaDeRuta(idRutaObra, out mensaje);
        }
    }
}
