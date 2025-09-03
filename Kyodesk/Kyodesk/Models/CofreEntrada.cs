using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaChamadosWpf.Models
{
    public class CofreEntrada
    {
        public int Id { get; set; }

        [Required] public string Nome { get; set; } = string.Empty;
        [Required] public string UsuarioLogin { get; set; } = string.Empty;
        [Required] public byte[] SenhaProtegida { get; set; } = Array.Empty<byte>();

        public string? Url { get; set; }
        public string? Observacoes { get; set; }

        public DateTime CriadoEm { get; set; } = DateTime.Now;
        public DateTime? AtualizadoEm { get; set; }

        public int? UsuarioId { get; set; }       // use se quiser atrelar ao usuário logado
        public virtual Usuario? Usuario { get; set; }
    }
}
