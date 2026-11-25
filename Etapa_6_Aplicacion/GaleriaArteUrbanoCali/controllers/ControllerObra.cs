using System;
using System.Collections.Generic;
using System.Data;
using GaleriaArteUrbanoCali.models;

namespace GaleriaArteUrbanoCali.controllers
{
    public class ControllerObra
    {
        private readonly Obra _modeloObra = new Obra();

        public List<Obra> ListarObras()
        {
            return _modeloObra.GetListaObras();
        }

        public DataTable ConsultarCatalogoObras()
        {
            return _modeloObra.ConsultarCatalogoObras();
        }

        public Obra? BuscarObra(string idObra)
        {
            if (string.IsNullOrWhiteSpace(idObra))
            {
                return null;
            }
            return _modeloObra.BuscarPorId(idObra);
        }

        public bool InsertarObra(Obra o, out string mensaje)
        {
            if (string.IsNullOrWhiteSpace(o.NombreObra) ||
                string.IsNullOrWhiteSpace(o.EstadoConservacion) ||
                string.IsNullOrWhiteSpace(o.IdArtistaFK) ||
                string.IsNullOrWhiteSpace(o.IdCategoriaFK) ||
                string.IsNullOrWhiteSpace(o.IdUbicacionFK) ||
                string.IsNullOrWhiteSpace(o.IdUsuarioFK))
            {
                mensaje = "Toda obra debe tener título, estado de conservación, artista, categoría, ubicación y usuario registrador.";
                return false;
            }

            return _modeloObra.InsertarObra(o, out mensaje);
        }

        public bool ActualizarObra(Obra o, out string mensaje)
        {
            if (string.IsNullOrWhiteSpace(o.IdObra) ||
                string.IsNullOrWhiteSpace(o.NombreObra) ||
                string.IsNullOrWhiteSpace(o.EstadoConservacion) ||
                string.IsNullOrWhiteSpace(o.IdArtistaFK) ||
                string.IsNullOrWhiteSpace(o.IdCategoriaFK) ||
                string.IsNullOrWhiteSpace(o.IdUbicacionFK))
            {
                mensaje = "Seleccione una obra válida y complete todos los campos requeridos.";
                return false;
            }

            return _modeloObra.ActualizarObra(o, out mensaje);
        }

        public bool RegistrarObraConRecursoPrincipal(
            Obra o,
            string idFormato,
            string nombreRecurso,
            string rutaArchivoRelativa,
            decimal tamanioMb,
            out string mensaje)
        {
            return _modeloObra.RegistrarObraConRecursoPrincipalSP(
                o, idFormato, nombreRecurso, rutaArchivoRelativa, tamanioMb, out mensaje);
        }
    }
}
