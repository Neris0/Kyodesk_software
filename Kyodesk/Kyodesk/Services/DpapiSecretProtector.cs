using System.Security.Cryptography;
using System.Text;

namespace SistemaChamadosWpf.Services
{
    public class DpapiSecretProtector : ISecretProtector
    {
        public byte[] Protect(string plainText)
        {
            var bytes = Encoding.UTF8.GetBytes(plainText ?? string.Empty);
            return ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
        }

        public string UnprotectToString(byte[] cipher)
        {
            if (cipher == null || cipher.Length == 0) return string.Empty;
            var bytes = ProtectedData.Unprotect(cipher, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
