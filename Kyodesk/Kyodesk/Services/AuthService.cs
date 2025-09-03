using Microsoft.EntityFrameworkCore;
using SistemaChamadosWpf.Data;
using SistemaChamadosWpf.Models;
using System.Threading.Tasks;
using BCrypt.Net;

namespace SistemaChamadosWpf.Services
{
    /// <summary>
    /// Serviço responsável pela autenticação de usuários.  Utiliza
    /// PasswordHasher para verificar a senha armazenada em hash.
    /// </summary>
    public class AuthService
    {
        private readonly AppDbContext _context;
        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public Usuario? UsuarioAtual { get; internal set; }

        public async Task<Usuario?> AutenticarAsync(string email, string senha)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (usuario == null)
                return null;
            // Verifica a senha usando BCrypt
            var ok = BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash);
            return ok ? usuario : null;
        }
    }
}