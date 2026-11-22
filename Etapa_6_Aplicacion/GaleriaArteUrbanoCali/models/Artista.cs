using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace GaleriaArteUrbanoCali.models
{
    public class Artista
    {
        public string IdArtista { get; set; } = string.Empty;
        public string NombreArtistico { get; set; } = string.Empty;
        public string Nombre1Real { get; set; } = string.Empty;
        public string? Nombre2Real { get; set; }
        public string Apellido1Real { get; set; } = string.Empty;
        public string? Apellido2Real { get; set; }
        public string? Biografia { get; set; }
        public string? PerfilRedSocial { get; set; }
        public bool EstadoArtista { get; set; } = true;
        public string IdPaisFK { get; set; } = string.Empty;
        public string NombrePais { get; set; } = string.Empty;

        public string NombreRealCompleto =>
            $"{Nombre1Real} {Nombre2Real}".Trim() + " " + $"{Apellido1Real} {Apellido2Real}".Trim();

        public List<Artista> GetArtistasActivos()
        {
            List<Artista> lista = new List<Artista>();
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT a.idArtista, a.nombreArtistico, a.nombre1Real, a.nombre2Real,
                       a.apellido1Real, a.apellido2Real, a.biografia, a.perfilRedSocial,
                       a.estadoArtista, a.idPaisFK, p.nombrePais
                FROM artista a
                INNER JOIN pais p ON a.idPaisFK = p.idPais
                WHERE a.estadoArtista = TRUE
                ORDER BY a.nombreArtistico;";

            try
            {
                using MySqlCommand cmd = new MySqlCommand(sql, conexion.Abrir());
                using MySqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    lista.Add(MapearArtista(r));
                }
            }
            finally
            {
                conexion.Cerrar();
            }

            return lista;
        }

        public DataTable ConsultarTablaArtistas()
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT a.idArtista AS 'ID',
                       a.nombreArtistico AS 'Nombre Artístico',
                       CONCAT_WS(' ', a.nombre1Real, a.nombre2Real, a.apellido1Real, a.apellido2Real) AS 'Nombre Real',
                       p.nombrePais AS 'País',
                       IFNULL(a.perfilRedSocial, 'Sin red social') AS 'Red Social',
                       CASE WHEN a.estadoArtista = 1 THEN 'Activo' ELSE 'Inactivo' END AS 'Estado',
                       COUNT(o.idObra) AS 'Obras Registradas'
                FROM artista a
                INNER JOIN pais p ON a.idPaisFK = p.idPais
                LEFT JOIN obra o ON a.idArtista = o.idArtistaFK
                GROUP BY a.idArtista, a.nombreArtistico, a.nombre1Real, a.nombre2Real,
                         a.apellido1Real, a.apellido2Real, p.nombrePais, a.perfilRedSocial, a.estadoArtista
                ORDER BY a.nombreArtistico;";
            return conexion.ExecuteDataTable(sql);
        }

        public Artista? BuscarPorIdONombreArtistico(string criterio)
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                SELECT a.*, p.nombrePais
                FROM artista a
                INNER JOIN pais p ON a.idPaisFK = p.idPais
                WHERE a.idArtista = @c OR a.nombreArtistico = @c
                LIMIT 1;";

            try
            {
                using MySqlCommand cmd = new MySqlCommand(sql, conexion.Abrir());
                cmd.Parameters.AddWithValue("@c", criterio.Trim());
                using MySqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    return MapearArtista(r);
                }
            }
            finally
            {
                conexion.Cerrar();
            }

            return null;
        }

        public bool InsertarArtista(Artista a, out string mensajeError)
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                INSERT INTO artista
                (idArtista, nombreArtistico, nombre1Real, nombre2Real, apellido1Real, apellido2Real,
                 biografia, perfilRedSocial, estadoArtista, idPaisFK)
                VALUES
                (@id, @art, @n1, @n2, @a1, @a2, @bio, @red, @estado, @idPais);";

            var p = new Dictionary<string, object?>
            {
                { "@id", string.IsNullOrWhiteSpace(a.IdArtista) ? Guid.NewGuid().ToString() : a.IdArtista.Trim() },
                { "@art", a.NombreArtistico.Trim() },
                { "@n1", a.Nombre1Real.Trim() },
                { "@n2", string.IsNullOrWhiteSpace(a.Nombre2Real) ? null : a.Nombre2Real.Trim() },
                { "@a1", a.Apellido1Real.Trim() },
                { "@a2", string.IsNullOrWhiteSpace(a.Apellido2Real) ? null : a.Apellido2Real.Trim() },
                { "@bio", string.IsNullOrWhiteSpace(a.Biografia) ? null : a.Biografia.Trim() },
                { "@red", string.IsNullOrWhiteSpace(a.PerfilRedSocial) ? null : a.PerfilRedSocial.Trim() },
                { "@estado", a.EstadoArtista },
                { "@idPais", a.IdPaisFK.Trim() }
            };

            return conexion.ExecuteQuery(sql, p, out mensajeError);
        }

        public bool ActualizarArtista(Artista a, out string mensajeError)
        {
            using var conexion = new ConexionMysql();
            string sql = @"
                UPDATE artista SET
                    nombreArtistico = @art,
                    nombre1Real = @n1,
                    nombre2Real = @n2,
                    apellido1Real = @a1,
                    apellido2Real = @a2,
                    biografia = @bio,
                    perfilRedSocial = @red,
                    estadoArtista = @estado,
                    idPaisFK = @idPais
                WHERE idArtista = @id;";

            var p = new Dictionary<string, object?>
            {
                { "@id", a.IdArtista.Trim() },
                { "@art", a.NombreArtistico.Trim() },
                { "@n1", a.Nombre1Real.Trim() },
                { "@n2", string.IsNullOrWhiteSpace(a.Nombre2Real) ? null : a.Nombre2Real.Trim() },
                { "@a1", a.Apellido1Real.Trim() },
                { "@a2", string.IsNullOrWhiteSpace(a.Apellido2Real) ? null : a.Apellido2Real.Trim() },
                { "@bio", string.IsNullOrWhiteSpace(a.Biografia) ? null : a.Biografia.Trim() },
                { "@red", string.IsNullOrWhiteSpace(a.PerfilRedSocial) ? null : a.PerfilRedSocial.Trim() },
                { "@estado", a.EstadoArtista },
                { "@idPais", a.IdPaisFK.Trim() }
            };

            return conexion.ExecuteQuery(sql, p, out mensajeError);
        }

        public int ContarObrasAsociadas(string idArtista)
        {
            using var conexion = new ConexionMysql();
            string sql = "SELECT COUNT(*) FROM obra WHERE idArtistaFK = @id;";
            var p = new Dictionary<string, object?> { { "@id", idArtista.Trim() } };
            object? res = conexion.ExecuteScalar(sql, p);
            return res != null ? Convert.ToInt32(res) : 0;
        }

        // Un artista con obras se desactiva lógicamente en lugar de eliminarse físicamente
        public bool EliminarODesactivarArtista(string idArtista, out string mensajeResultado)
        {
            int obras = ContarObrasAsociadas(idArtista);
            using var conexion = new ConexionMysql();

            if (obras > 0)
            {
                string sqlDesactivar = "UPDATE artista SET estadoArtista = FALSE WHERE idArtista = @id;";
                var p = new Dictionary<string, object?> { { "@id", idArtista.Trim() } };
                bool ok = conexion.ExecuteQuery(sqlDesactivar, p, out string err);
                mensajeResultado = ok
                    ? $"El artista tiene {obras} obra(s) registrada(s), por lo que fue desactivado sin eliminar su historial."
                    : "Error al desactivar artista: " + err;
                return ok;
            }
            else
            {
                string sqlEliminar = "DELETE FROM artista WHERE idArtista = @id;";
                var p = new Dictionary<string, object?> { { "@id", idArtista.Trim() } };
                bool ok = conexion.ExecuteQuery(sqlEliminar, p, out string err);
                mensajeResultado = ok
                    ? "Artista eliminado de la base de datos (no tenía obras asociadas)."
                    : "Error al eliminar artista: " + err;
                return ok;
            }
        }

        private static Artista MapearArtista(MySqlDataReader r)
        {
            return new Artista
            {
                IdArtista = r["idArtista"].ToString()!,
                NombreArtistico = r.GetString("nombreArtistico"),
                Nombre1Real = r.GetString("nombre1Real"),
                Nombre2Real = r.IsDBNull(r.GetOrdinal("nombre2Real")) ? null : r.GetString("nombre2Real"),
                Apellido1Real = r.GetString("apellido1Real"),
                Apellido2Real = r.IsDBNull(r.GetOrdinal("apellido2Real")) ? null : r.GetString("apellido2Real"),
                Biografia = r.IsDBNull(r.GetOrdinal("biografia")) ? null : r.GetString("biografia"),
                PerfilRedSocial = r.IsDBNull(r.GetOrdinal("perfilRedSocial")) ? null : r.GetString("perfilRedSocial"),
                EstadoArtista = r.GetBoolean("estadoArtista"),
                IdPaisFK = r["idPaisFK"].ToString()!,
                NombrePais = r.GetString("nombrePais")
            };
        }
    }
}
