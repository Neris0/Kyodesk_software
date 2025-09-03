using System.Linq;
using Microsoft.EntityFrameworkCore;
using SistemaChamadosWpf.Data;
using SistemaChamadosWpf.Models;

namespace SistemaChamadosWpf
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            // Admin: pode logar por "mario" (email) ou "Mario Neris" (nome) – senha 4357
            UpsertUsuario(context,
                nomeCompleto: "Mario Neris",
                email: "admin",
                ehAdmin: true,
                senhaPlano: "4357");

            // Usuário comum: pode logar por "usuario@exemplo.com" (email) ou "paulo" (nome) – senha 4357
            UpsertUsuario(context,
                nomeCompleto: "paulo",
                email: "usuario@exemplo.com",
                ehAdmin: false,
                senhaPlano: "4357");

            context.SaveChanges();
        }

        private static void UpsertUsuario(
            AppDbContext ctx,
            string nomeCompleto,
            string email,
            bool ehAdmin,
            string senhaPlano)
        {
            var nome = nomeCompleto.Trim();
            var mail = email.Trim();

            // Procura por email OU nome completo (case-insensitive no SQLite)
            var user = ctx.Usuarios.FirstOrDefault(u =>
                EF.Functions.Collate(u.Email, "NOCASE") == mail ||
                EF.Functions.Collate(u.NomeCompleto, "NOCASE") == nome);

            if (user == null)
            {
                user = new Usuario
                {
                    NomeCompleto = nome,
                    Email = mail,
                    EhAdmin = ehAdmin,
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword(senhaPlano)
                };
                ctx.Usuarios.Add(user);
            }
            else
            {
                // Atualiza dados principais; não sobrescreve senha existente
                user.NomeCompleto = nome;
                user.Email = mail;
                user.EhAdmin = ehAdmin;

                if (string.IsNullOrWhiteSpace(user.SenhaHash))
                    user.SenhaHash = BCrypt.Net.BCrypt.HashPassword(senhaPlano);

                ctx.Usuarios.Update(user);
            }
        }
    }
}
