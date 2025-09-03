using Microsoft.EntityFrameworkCore;
using SistemaChamadosWpf.Data;
using SistemaChamadosWpf.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace SistemaChamadosWpf.ViewModels


{
    public class ChamadoDetalhesViewModel : INotifyPropertyChanged

    {

        private readonly AppDbContext _context;
        public Usuario UsuarioLogado { get; set; } = new Usuario();
        // Inicializa _chamado para evitar avisos de nulidade. O objeto será
        // substituído quando CarregarChamado for chamado.
        private Chamado _chamado = new Chamado();
        public Chamado Chamado
        {
            get => _chamado;
            set { _chamado = value; OnPropertyChanged(); }
        }
        public ObservableCollection<Comentario> Comentarios { get; set; } = new ObservableCollection<Comentario>();
        private string _novoComentarioTexto = string.Empty;
        public string NovoComentarioTexto
        {
            get => _novoComentarioTexto;
            set { _novoComentarioTexto = value; OnPropertyChanged(); }
        }
        public ICommand AdicionarComentarioCommand { get; }
        public ICommand AssumirCommand { get; }
        public ICommand AlterarStatusCommand { get; }

        public ChamadoDetalhesViewModel(AppDbContext context)
        {
            _context = context;
            AdicionarComentarioCommand = new RelayCommand(_ => AdicionarComentario(), _ => !string.IsNullOrWhiteSpace(NovoComentarioTexto));
            AssumirCommand = new RelayCommand(_ => AssumirChamado(), _ => UsuarioLogado.EhAdmin && Chamado.ResponsavelId == null);
            AlterarStatusCommand = new RelayCommand(status => AlterarStatus((StatusChamado)status!), _ => UsuarioLogado.EhAdmin);
        }

        public void CarregarChamado(int chamadoId)
        {
            Chamado = _context.Chamados
                .Include(c => c.Usuario)
                .Include(c => c.Responsavel)
                .Include(c => c.Comentarios)
                    .ThenInclude(cm => cm.Usuario)
                .First(c => c.Id == chamadoId);
            Comentarios.Clear();
            foreach (var cm in Chamado.Comentarios.OrderBy(c => c.DataCriacao))
            {
                Comentarios.Add(cm);
            }
        }



        private void AdicionarComentario()
        {
            var comentario = new Comentario
            {
                ChamadoId = Chamado.Id,
                UsuarioId = UsuarioLogado.Id,
                Texto = NovoComentarioTexto.Trim(),
                DataCriacao = DateTime.Now
            };
            _context.Comentarios.Add(comentario);
            _context.SaveChanges();
            NovoComentarioTexto = string.Empty;
            CarregarChamado(Chamado.Id);
        }

        private void AssumirChamado()
        {
            Chamado.ResponsavelId = UsuarioLogado.Id;
            Chamado.Status = StatusChamado.EmAndamento;
            Chamado.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();
            CarregarChamado(Chamado.Id);
        }

        private void AlterarStatus(StatusChamado status)
        {
            Chamado.Status = status;
            Chamado.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();
            CarregarChamado(Chamado.Id);
        }
        public record StatusOption(StatusChamado Value, string Label);

        public IReadOnlyList<StatusOption> StatusOptions { get; } =
        [
            new(StatusChamado.Aberto,             "Aberto"),
    new(StatusChamado.EmAndamento,        "Em andamento"),
    new(StatusChamado.AguardandoRetorno,  "Aguardando retorno"),
    new(StatusChamado.Pausado,            "Pausado"),
    new(StatusChamado.Encerrado,          "Encerrado"),
];


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}