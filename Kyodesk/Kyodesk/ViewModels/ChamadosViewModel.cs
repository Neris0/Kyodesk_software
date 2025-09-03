using Microsoft.EntityFrameworkCore;
using SistemaChamadosWpf.Data;
using SistemaChamadosWpf.Models;
using SistemaChamadosWpf.Services;
using SistemaChamadosWpf.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

using System.Windows.Input;


namespace SistemaChamadosWpf.ViewModels
{
    public class ChamadosViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _context;
        private readonly AuthService _auth;

        public ChamadosViewModel(AppDbContext context, AuthService auth)
        {
            _context = context;
            _auth = auth;

            // Se o AuthService já tiver o usuário (após login), capturamos aqui:
            if (_auth.UsuarioAtual is not null)
                DefinirUsuario(_auth.UsuarioAtual);

            AtualizarCommand = new RelayCommand(async _ => await CarregarChamadosAsync());
            NovoCommand = new RelayCommand(_ => AbrirNovoChamado());
            DetalhesCommand = new RelayCommand(_ => AbrirDetalhes(), _ => ChamadoSelecionado != null);
        }

        // ===== USUÁRIO LOGADO =====
        private Usuario? _usuarioLogado;
        public Usuario? UsuarioLogado
        {
            get => _usuarioLogado;
            private set
            {
                if (_usuarioLogado != value)
                {
                    _usuarioLogado = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(NomeUsuario));
                    OnPropertyChanged(nameof(EmailUsuario));
                }
            }
        }

        // Propriedades para o binding no topo da janela
        public string NomeUsuario => UsuarioLogado?.NomeCompleto ?? "";
        public string EmailUsuario => UsuarioLogado?.Email ?? "";

        public void DefinirUsuario(Usuario usuario)
        {
            UsuarioLogado = usuario;
        }

        // ===== LISTA DE CHAMADOS =====
        public ObservableCollection<Chamado> Chamados { get; } = new();

        private Chamado? _chamadoSelecionado;
        public Chamado? ChamadoSelecionado
        {
            get => _chamadoSelecionado;
            set
            {
                if (_chamadoSelecionado != value)
                {
                    _chamadoSelecionado = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public ICommand AtualizarCommand { get; }
        public ICommand NovoCommand { get; }
        public ICommand DetalhesCommand { get; }

        public async Task CarregarChamadosAsync()
        {
            var query = _context.Chamados
                                .Include(c => c.Responsavel)
                                .Include(c => c.Usuario)
                                .AsQueryable();

            if (UsuarioLogado is not null && !UsuarioLogado.EhAdmin)
                query = query.Where(c => c.UsuarioId == UsuarioLogado.Id);

            var itens = await query.OrderByDescending(c => c.DataCriacao).ToListAsync();

            Chamados.Clear();
            foreach (var c in itens) Chamados.Add(c);
        }

        private void AbrirNovoChamado()
        {
            var win = new NovoChamadoWindow(UsuarioLogado!)
            {
                Owner = Application.Current.MainWindow,                 // <<< importante
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ShowInTaskbar = false
            };

            win.ShowDialog();          // abrir modal e centralizado
            _ = CarregarChamadosAsync();
        }
        


        private void AbrirDetalhes()
        {
            if (ChamadoSelecionado is null) return;

            var win = new ChamadoDetalhesWindow(ChamadoSelecionado, UsuarioLogado!)
            {
                Owner = Application.Current.MainWindow,                 // <<< define o dono
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ShowInTaskbar = false
            };

            win.ShowDialog();          // abre modal, centralizado na MainWindow
            _ = CarregarChamadosAsync();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
