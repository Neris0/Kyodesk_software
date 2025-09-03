using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaChamadosWpf.Models
{
    public class KnowledgeArticle
    {
        public int Id { get; set; }

        [Required] public string Titulo { get; set; } = string.Empty;
        [Required] public string Categoria { get; set; } = "Geral";
        [Required] public string Conteudo { get; set; } = string.Empty;

        public DateTime CriadoEm { get; set; } = DateTime.Now;
        public DateTime? AtualizadoEm { get; set; }

        [Required] public int AutorId { get; set; }
        public virtual Usuario? Autor { get; set; }
    }
}
