namespace SistemaChamadosWpf.Services
{
    public interface ISecretProtector
    {
        byte[] Protect(string plainText);
        string UnprotectToString(byte[] cipher);
    }
}
