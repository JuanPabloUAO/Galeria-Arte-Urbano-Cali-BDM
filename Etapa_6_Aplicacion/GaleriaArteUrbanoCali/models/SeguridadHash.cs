using System;
using System.Security.Cryptography;
using System.Text;

namespace GaleriaArteUrbanoCali.models
{
    public static class SeguridadHash
    {
        public static string GenerarHashSha256(string textoPlano)
        {
            if (string.IsNullOrEmpty(textoPlano))
            {
                return string.Empty;
            }

            using SHA256 sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(textoPlano);
            byte[] hash = sha256.ComputeHash(bytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        public static bool VerificarContrasena(string contrasenaIngresada, string hashAlmacenado)
        {
            if (string.IsNullOrWhiteSpace(contrasenaIngresada) || string.IsNullOrWhiteSpace(hashAlmacenado))
            {
                return false;
            }

            string sha256Ingresada = GenerarHashSha256(contrasenaIngresada);
            return string.Equals(sha256Ingresada, hashAlmacenado, StringComparison.OrdinalIgnoreCase);
        }
    }
}
