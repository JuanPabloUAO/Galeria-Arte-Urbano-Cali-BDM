using System;
using System.Collections.Generic;
using System.Data;
using GaleriaArteUrbanoCali.models;

namespace GaleriaArteUrbanoCali.controllers
{
    public class ControllerCategoria
    {
        private readonly Categoria _modeloCategoria = new Categoria();

        public List<Categoria> ListarCategoriasActivas()
        {
            return _modeloCategoria.GetCategoriasActivas();
        }

        public DataTable ConsultarTablaCategorias()
        {
            return _modeloCategoria.ConsultarTablaCategorias();
        }

        public Categoria? BuscarCategoria(string idCategoria)
        {
            if (string.IsNullOrWhiteSpace(idCategoria))
            {
                return null;
            }
            return _modeloCategoria.BuscarPorId(idCategoria);
        }

        public bool InsertarCategoria(Categoria c, out string mensaje)
        {
            if (string.IsNullOrWhiteSpace(c.NombreCategoria))
            {
                mensaje = "El nombre de la categoría es obligatorio.";
                return false;
            }
            return _modeloCategoria.InsertarCategoria(c, out mensaje);
        }

        public bool ActualizarCategoria(Categoria c, out string mensaje)
        {
            if (string.IsNullOrWhiteSpace(c.IdCategoria) || string.IsNullOrWhiteSpace(c.NombreCategoria))
            {
                mensaje = "Seleccione una categoría y especifique su nombre.";
                return false;
            }
            return _modeloCategoria.ActualizarCategoria(c, out mensaje);
        }

        public bool CambiarEstado(string idCategoria, bool nuevoEstado, out string mensaje)
        {
            if (string.IsNullOrWhiteSpace(idCategoria))
            {
                mensaje = "Seleccione una categoría válida.";
                return false;
            }
            return _modeloCategoria.CambiarEstadoCategoria(idCategoria, nuevoEstado, out mensaje);
        }
    }
}
