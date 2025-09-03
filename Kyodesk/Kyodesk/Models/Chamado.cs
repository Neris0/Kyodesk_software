using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SistemaChamadosWpf.Models
{
    /// <summary>
    /// Enumera os possíveis estados de um chamado.
    /// Declarado como public para ser acessível em bindings XAML.
    /// </summary>


    public enum StatusChamado 
    { 
        Aberto, 
        EmAndamento, 
        Resolvido, 
        Fechado,
        AguardandoRetorno,
        Pausado,
        Encerrado
    }



    public class Chamado
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;
        [Required]
        public string Descricao { get; set; } = string.Empty;
        public string Solicitante { get; set; }
        public string Classificacao { get; set; }
        public string Categoria { get; set; }
        public StatusChamado Status { get; set; } = StatusChamado.Aberto;
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public DateTime? DataAtualizacao { get; set; }
        public int UsuarioId { get; set; }
        public virtual Usuario? Usuario { get; set; }
        public int? ResponsavelId { get; set; }
        public virtual Usuario? Responsavel { get; set; }
        public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
    }


    
}