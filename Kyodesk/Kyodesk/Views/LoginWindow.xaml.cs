using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using SistemaChamadosWpf.Services;
using SistemaChamadosWpf.Models;

namespace SistemaChamadosWpf.Views
{
    /// <summary>
    /// Lógica de interação para LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService;
        public LoginWindow()
        {
            InitializeComponent();
            // Obter instância do serviço de autenticação do DI
            _authService = ((App)Application.Current).Services.GetRequiredService<AuthService>();
        }

        private async void Entrar_Click(object sender, RoutedEventArgs e)
        {
            var identifier = EmailTextBox.Text.Trim(); // pode ser e-mail ou nome
            var senha = SenhaBox.Password;

            var usuario = await _authService.AutenticarAsync(identifier, senha);
            if (usuario != null)
            {
                var main = new MainWindow(usuario);
                Application.Current.MainWindow = main;
                main.Show();

                // >>> dispara a carga logo que a janela existe
                if (main.DataContext is SistemaChamadosWpf.ViewModels.ChamadosViewModel vm)
                    await vm.CarregarChamadosAsync();

                this.Close();
            }
            else
            {
                MessageBox.Show("E-mail/nome ou senha inválidos.",
                                "Erro de login", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void Sair_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void EmailTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}