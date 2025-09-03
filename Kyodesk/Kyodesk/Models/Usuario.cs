using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaChamadosWpf.Models
{
    /// <summary>
    /// Representa um usuário da aplicação WPF.  Inclui informações básicas
    /// necessárias para autenticação e autorização local (sem ASP.NET Identity).
    /// </summary>
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string NomeCompleto { get; set; } = string.Empty;
        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Senha armazenada em hash para maior segurança.
        /// </summary>
        public string SenhaHash { get; set; } = string.Empty;
        /// <summary>
        /// Indica se o usuário é administrador (técnico de TI).  Administradores
        /// podem ver todos os chamados e alterar status.
        /// </summary>
        public bool EhAdmin { get; set; }
        public virtual ICollection<CofreEntrada> CofreEntradas { get; set; } = new List<CofreEntrada>();
        public virtual ICollection<KnowledgeArticle> Artigos { get; set; } = new List<KnowledgeArticle>();

        public virtual ICollection<Chamado> ChamadosCriados { get; set; } = new List<Chamado>();
        public virtual ICollection<Chamado> ChamadosResponsavel { get; set; } = new List<Chamado>();
        public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
    }
}