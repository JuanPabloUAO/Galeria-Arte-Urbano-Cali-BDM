using System;
using System.Data;
using GaleriaArteUrbanoCali.models;

namespace GaleriaArteUrbanoCali.controllers
{
    public class ControllerConsultas
    {
        private readonly ConsultasReportes _modeloConsultas = new ConsultasReportes();

        public DataTable ObtenerFichaCompletaObras()
        {
            return _modeloConsultas.ConsultarFichaCompletaObras();
        }

        public DataTable ObtenerObrasPorArtista(string idArtista)
        {
            return _modeloConsultas.ConsultarObrasPorArtista(idArtista);
        }

        public DataTable ObtenerObrasPorCategoria(string idCategoria)
        {
            return _modeloConsultas.ConsultarObrasPorCategoria(idCategoria);
        }

        public DataTable ObtenerObrasPorComunaOBarrio(string? idComuna, string? idBarrio)
        {
            return _modeloConsultas.ConsultarObrasPorComunaOBarrio(idComuna, idBarrio);
        }

        public DataTable ObtenerItinerarioRuta(string? idRuta = null)
        {
            return _modeloConsultas.ConsultarItinerarioRuta(idRuta);
        }

        public DataTable ObtenerEstadisticasPorComuna()
        {
            return _modeloConsultas.ConsultarEstadisticasPorComuna();
        }

        public DataTable ObtenerEstadisticasPorCategoria()
        {
            return _modeloConsultas.ConsultarEstadisticasPorCategoria();
        }

        public DataTable ObtenerArtistasMasProlificos(int minimoObras = 2)
        {
            return _modeloConsultas.ConsultarArtistasMasProlificos(minimoObras);
        }
    }
}
