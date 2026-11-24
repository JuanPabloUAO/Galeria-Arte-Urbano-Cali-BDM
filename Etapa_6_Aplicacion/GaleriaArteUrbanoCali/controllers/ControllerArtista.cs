using System;
using System.Collections.Generic;
using System.Data;
using GaleriaArteUrbanoCali.models;

namespace GaleriaArteUrbanoCali.controllers
{
    public class ControllerArtista
    {
        private readonly Artista _modeloArtista = new Artista();
        private readonly Pais _modeloPais = new Pais();

        public List<Pais> ListarPaises()
        {
            return _modeloPais.GetPaises();
        }

        public List<Artista> ListarArtistasActivos()
        {
            return _modeloArtista.GetArtistasActivos();
        }

        public DataTable ConsultarTablaArtistas()
        {
            return _modeloArtista.ConsultarTablaArtistas();
        }

        public Artista? BuscarArtista(string criterio)
        {
            if (string.IsNullOrWhiteSpace(criterio))
            {
                return null;
            }
            return _modeloArtista.BuscarPorIdONombreArtistico(criterio);
        }

        public bool InsertarArtista(Artista a, out string mensaje)
        {
            if (string.IsNullOrWhiteSpace(a.NombreArtistico) ||
                string.IsNullOrWhiteSpace(a.Nombre1Real) ||
                string.IsNullOrWhiteSpace(a.Apellido1Real) ||
                string.IsNullOrWhiteSpace(a.IdPaisFK))
            {
                mensaje = "Nombre artístico, primer nombre real, primer apellido real y país son obligatorios.";
                return false;
            }

            return _modeloArtista.InsertarArtista(a, out mensaje);
        }

        public bool ActualizarArtista(Artista a, out string mensaje)
        {
            if (string.IsNullOrWhiteSpace(a.IdArtista) ||
                string.IsNullOrWhiteSpace(a.NombreArtistico) ||
                string.IsNullOrWhiteSpace(a.Nombre1Real) ||
                string.IsNullOrWhiteSpace(a.Apellido1Real) ||
                string.IsNullOrWhiteSpace(a.IdPaisFK))
            {
                mensaje = "Debe buscar primero un artista y completar los campos obligatorios.";
                return false;
            }

            return _modeloArtista.ActualizarArtista(a, out mensaje);
        }

        public int ContarObrasDeArtista(string idArtista)
        {
            return _modeloArtista.ContarObrasAsociadas(idArtista);
        }

        public bool EliminarODesactivarArtista(string idArtista, out string mensaje)
        {
            if (string.IsNullOrWhiteSpace(idArtista))
            {
                mensaje = "Seleccione o busque un artista válido.";
                return false;
            }

            return _modeloArtista.EliminarODesactivarArtista(idArtista, out mensaje);
        }
    }
}
