using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaChamadosWpf.Models
{
    public class Comentario
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ChamadoId { get; set; }
        public virtual Chamado? Chamado { get; set; }
        [Required]
        public int UsuarioId { get; set; }
        public virtual Usuario? Usuario { get; set; }
        [Required]
        public string Texto { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
}