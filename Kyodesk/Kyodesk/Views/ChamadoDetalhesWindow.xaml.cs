using Microsoft.Extensions.DependencyInjection;
using SistemaChamadosWpf.Models;
using SistemaChamadosWpf.ViewModels;
using System.Windows;
using System.Windows.Navigation;

namespace SistemaChamadosWpf.Views
{
    /// <summary>
    /// Lógica de interação para ChamadoDetalhesWindow.xaml
    /// </summary>
    public partial class ChamadoDetalhesWindow : Window
    {
        private readonly ChamadoDetalhesViewModel _viewModel;
        public ChamadoDetalhesWindow(Chamado chamado, Usuario usuario)
        {
            InitializeComponent();
            _viewModel = ((App)Application.Current).Services.GetRequiredService<ChamadoDetalhesViewModel>();
            _viewModel.UsuarioLogado = usuario;
            _viewModel.CarregarChamado(chamado.Id);
            Loaded += (s, e) => WindowState = WindowState.Maximized;
            DataContext = _viewModel;


        }

        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            // Se estiver hospedado em um Frame/NavigationWindow, tenta voltar
            var nav = NavigationService.GetNavigationService(this);
            if (nav != null && nav.CanGoBack)
            {
                nav.GoBack();
            }
            else
            {
                // Caso seja uma Window comum, apenas fecha a janela
                this.Close();
            }
        }
    }
}