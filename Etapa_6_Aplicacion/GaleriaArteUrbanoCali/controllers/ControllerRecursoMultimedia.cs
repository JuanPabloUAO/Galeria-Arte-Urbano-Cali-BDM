using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using GaleriaArteUrbanoCali.models;

namespace GaleriaArteUrbanoCali.controllers
{
    public class ControllerRecursoMultimedia
    {
        private readonly RecursoMultimedia _modeloRecurso = new RecursoMultimedia();
        private readonly TipoRecurso _modeloTipoRecurso = new TipoRecurso();
        private readonly FormatoRecurso _modeloFormato = new FormatoRecurso();

        public List<TipoRecurso> ListarTiposRecurso()
        {
            return _modeloTipoRecurso.GetTiposRecurso();
        }

        public List<FormatoRecurso> ListarFormatos()
        {
            return _modeloFormato.GetFormatos();
        }

        public List<FormatoRecurso> ListarFormatosPorTipo(string idTipoRecurso)
        {
            if (string.IsNullOrWhiteSpace(idTipoRecurso))
            {
                return _modeloFormato.GetFormatos();
            }
            return _modeloFormato.GetFormatosPorTipo(idTipoRecurso);
        }

        public DataTable ConsultarRecursosPorObra(string? idObra = null)
        {
            return _modeloRecurso.ConsultarRecursosPorObra(idObra);
        }

        public bool CopiarYRegistrarArchivoMultimedia(
            string rutaOrigenPC,
            FormatoRecurso formatoSeleccionado,
            RecursoMultimedia rm,
            out string mensaje)
        {
            if (string.IsNullOrWhiteSpace(rm.NombreRecurso) ||
                string.IsNullOrWhiteSpace(rm.IdObraFK) ||
                string.IsNullOrWhiteSpace(rm.IdFormatoFK))
            {
                mensaje = "Nombre del recurso, obra asociada y formato son obligatorios.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(rutaOrigenPC) && File.Exists(rutaOrigenPC))
            {
                string extArchivo = Path.GetExtension(rutaOrigenPC).TrimStart('.').ToUpperInvariant();
                if (!string.Equals(extArchivo, formatoSeleccionado.Extension, StringComparison.OrdinalIgnoreCase) &&
                    !(extArchivo == "JPEG" && formatoSeleccionado.Extension == "JPG"))
                {
                    mensaje = $"La extensión del archivo seleccionado (.{extArchivo}) no coincide con el formato elegido ({formatoSeleccionado.Extension}).";
                    return false;
                }

                string nombreArchivoLimpio = Path.GetFileName(rutaOrigenPC).Replace(" ", "_");
                string rutaDestinoFisica = ConexionMysql.ResolverRutaFisicaMultimedia(
                    formatoSeleccionado.CarpetaBase,
                    nombreArchivoLimpio);

                string? dirDestino = Path.GetDirectoryName(rutaDestinoFisica);
                if (!string.IsNullOrEmpty(dirDestino))
                {
                    Directory.CreateDirectory(dirDestino);
                }

                if (!string.Equals(Path.GetFullPath(rutaOrigenPC), rutaDestinoFisica, StringComparison.OrdinalIgnoreCase))
                {
                    File.Copy(rutaOrigenPC, rutaDestinoFisica, overwrite: true);
                }

                FileInfo info = new FileInfo(rutaDestinoFisica);
                decimal mb = Math.Round((decimal)info.Length / (1024m * 1024m), 2);
                if (mb <= 0m)
                {
                    mb = 0.01m;
                }

                rm.RutaArchivo = nombreArchivoLimpio;
                rm.Tamanio = mb;
            }
            else if (string.IsNullOrWhiteSpace(rm.RutaArchivo))
            {
                mensaje = "Debe seleccionar un archivo desde el computador o indicar el nombre relativo en la carpeta.";
                return false;
            }

            if (rm.RutaArchivo.Contains(':') || rm.RutaArchivo.StartsWith('/') || rm.RutaArchivo.StartsWith('\\'))
            {
                mensaje = "La ruta almacenada en MySQL debe ser relativa a RecursosMultimedia, nunca absoluta.";
                return false;
            }

            if (rm.Tamanio <= 0)
            {
                rm.Tamanio = 0.10m;
            }

            return _modeloRecurso.InsertarRecurso(rm, out mensaje);
        }

        public bool EliminarRecurso(string idRecurso, out string mensaje)
        {
            if (string.IsNullOrWhiteSpace(idRecurso))
            {
                mensaje = "Seleccione un recurso multimedia válido.";
                return false;
            }
            return _modeloRecurso.EliminarRecurso(idRecurso, out mensaje);
        }

        public string ResolverRutaFisica(string carpetaBase, string rutaArchivo)
        {
            return ConexionMysql.ResolverRutaFisicaMultimedia(carpetaBase, rutaArchivo);
        }
    }
}
