using System.Security.Cryptography;
using System.Text;

namespace ApiCartao.Helpers
{
    public static class SenhaHash
    {
        public static string GerarHash(string senha)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytesSenha = Encoding.UTF8.GetBytes(senha);

                byte[] hashBytes = sha256.ComputeHash(bytesSenha);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}
