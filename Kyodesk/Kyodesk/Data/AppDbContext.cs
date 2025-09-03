using Microsoft.EntityFrameworkCore;
using SistemaChamadosWpf.Models;

namespace SistemaChamadosWpf.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<CofreEntrada> CofreEntradas => Set<CofreEntrada>();
        public DbSet<KnowledgeArticle> Conhecimentos => Set<KnowledgeArticle>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Chamado> Chamados => Set<Chamado>();
        public DbSet<Comentario> Comentarios => Set<Comentario>();
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Chamado>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.ChamadosCriados)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Chamado>()
                .HasOne(c => c.Responsavel)
                .WithMany(u => u.ChamadosResponsavel)
                .HasForeignKey(c => c.ResponsavelId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<Comentario>()
                .HasOne(cm => cm.Chamado)
                .WithMany(c => c.Comentarios)
                .HasForeignKey(cm => cm.ChamadoId);
            builder.Entity<Comentario>()
                .HasOne(cm => cm.Usuario)
                .WithMany(u => u.Comentarios)
                .HasForeignKey(cm => cm.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}