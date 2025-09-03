using Microsoft.Extensions.DependencyInjection;
using SistemaChamadosWpf.Data;
using SistemaChamadosWpf.Models;
using SistemaChamadosWpf.Services;
using SistemaChamadosWpf.ViewModels;
using SistemaChamadosWpf.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;




namespace SistemaChamadosWpf
{
    public partial class MainWindow : Window
    {
        private readonly ChamadosViewModel _vm;
        private readonly Usuario _usuario; // guarda o usuário passado no ctor

        public MainWindow(Usuario usuario)
        {
            InitializeComponent();

            _usuario = usuario;

            // Mostrar/esconder a aba do Cofre SOMENTE após InitializeComponent
    var tabCofre = FindName("TabCofre") as TabItem;
    if (tabCofre != null)
        tabCofre.Visibility = _usuario.EhAdmin ? Visibility.Visible : Visibility.Collapsed;

    LoadCofre();

            // Resolve o VM via DI e define DataContext UMA vez
            _vm = ((App)Application.Current).Services.GetRequiredService<ChamadosViewModel>();
            DataContext = _vm;
        }


        private async void AbrirNovoChamado_Click(object sender, RoutedEventArgs e)
        {
            // Diagnóstico: confirma que o click está chegando aqui
            // (pode remover depois)
            // MessageBox.Show("Clique no Novo recebido.");

            try
            {
                // Use o NAMESPACE EXATO onde está sua janela.
                // Se preferir, use o nome totalmente qualificado:
                // var win = new SistemaChamadosWpf.Views.NovoChamadoWindow();
                var win = new NovoChamadoWindow
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                bool? ok = win.ShowDialog(); // modal; troque por Show() se quiser só testar
                if (ok == true)
                {
                    // recarrega o grid após fechar o form
                    await _vm.CarregarChamadosAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Falha ao abrir a janela de novo chamado:\n\n" + ex,
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AbrirNovoArtigo_Click(object sender, RoutedEventArgs e)
        {
            
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // opcional: se tiver algo para o botão Detalhes no code-behind
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // informa o usuário ao VM e carrega a lista de chamados
            _vm.DefinirUsuario(_usuario);          // método síncrono do VM
            await _vm.CarregarChamadosAsync();     // popula o grid
        }

        private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // opcional: manipular seleção
        }

        private void Deslogar_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;

            var login = new SistemaChamadosWpf.Views.LoginWindow();
            Application.Current.MainWindow = login;
            login.Show();

            Close();
        }

        // ===== Cofre de senhas (SEM criar novas classes no projeto) =====

        // DTO interno para persistir/ler do JSON
        private sealed class CofreItemDto
        {
            public int Id { get; set; }
            public string Nome { get; set; } = "";
            public string Usuario { get; set; } = "";
            public string? Url { get; set; }
            public string? Observacoes { get; set; }
            public string SenhaCipherB64 { get; set; } = "";  // senha protegida (DPAPI) em Base64
            public DateTime CriadoEm { get; set; } = DateTime.Now;
        }

        private List<CofreItemDto> _cofre = new();
        private string CofrePath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                         "Kyodesk", "cofre.json");

        private void EnsureCofreDir()
        {
            var dir = Path.GetDirectoryName(CofrePath)!;
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        }

        private void LoadCofre()
        {
            EnsureCofreDir();
            if (File.Exists(CofrePath))
            {
                try
                {
                    var json = File.ReadAllText(CofrePath);
                    _cofre = JsonSerializer.Deserialize<List<CofreItemDto>>(json) ?? new();
                }
                catch
                {
                    _cofre = new();
                }
            }
            UpdateCofreGrid();
        }

        private void UpdateCofreGrid()
        {
            // Mostra só campos públicos; senha fica oculta
            if (CofreGrid != null)
            {
                CofreGrid.ItemsSource = _cofre
                    .OrderByDescending(x => x.CriadoEm)
                    .Select(x => new { x.Id, x.Nome, Usuario = x.Usuario, x.Url })
                    .ToList();
            }
        }

        private static byte[] Protect(string plain)
        {
            var bytes = Encoding.UTF8.GetBytes(plain ?? "");
            return ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
        }

        private static string UnprotectToString(byte[] cipher)
        {
            var bytes = ProtectedData.Unprotect(cipher, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(bytes);
        }

        // ===== Botões do Cofre =====

        private void CofreLimpar_Click(object sender, RoutedEventArgs e)
        {
            CofreLimparCampos();
        }

        private void CofreLimparCampos()
        {
            CofreNomeText.Text = string.Empty;
            CofreUsuarioText.Text = string.Empty;
            CofreSenhaBox.Password = string.Empty;
            CofreUrlText.Text = string.Empty;
            CofreObsText.Text = string.Empty;
        }

        private void CofreSalvar_Click(object sender, RoutedEventArgs e)
        {
            var nome = CofreNomeText.Text?.Trim();
            var usuario = CofreUsuarioText.Text?.Trim();
            var senha = CofreSenhaBox.Password ?? string.Empty;
            var url = string.IsNullOrWhiteSpace(CofreUrlText.Text) ? null : CofreUrlText.Text.Trim();
            var obs = string.IsNullOrWhiteSpace(CofreObsText.Text) ? null : CofreObsText.Text.Trim();

            if (string.IsNullOrWhiteSpace(nome) ||
                string.IsNullOrWhiteSpace(usuario) ||
                string.IsNullOrEmpty(senha))
            {
                MessageBox.Show("Preencha Nome, Usuário e Senha.", "Campos obrigatórios",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Criptografa a senha com DPAPI do usuário atual do Windows
            var cipher = Protect(senha);

            var item = new CofreItemDto
            {
                Id = _cofre.Count == 0 ? 1 : _cofre.Max(i => i.Id) + 1,
                Nome = nome!,
                Usuario = usuario!,
                Url = url,
                Observacoes = obs,
                SenhaCipherB64 = Convert.ToBase64String(cipher),
                CriadoEm = DateTime.Now
            };

            _cofre.Add(item);

            EnsureCofreDir();
            var json = JsonSerializer.Serialize(_cofre, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(CofrePath, json);

            CofreLimparCampos();
            UpdateCofreGrid();

            MessageBox.Show("Registro salvo no Cofre.", "Sucesso",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CofreCopiarSenha_Click(object sender, RoutedEventArgs e)
        {
            // Tenta pegar o Id pelo Tag do botão (recomendado no XAML: Tag="{Binding Id}")
            int? id = null;
            if (sender is FrameworkElement fe && fe.Tag != null)
            {
                if (fe.Tag is int i) id = i;
                else if (int.TryParse(fe.Tag.ToString(), out var parsed)) id = parsed;
            }

            // Fallback: se não tiver Tag, tenta o item selecionado no grid
            if (id == null && CofreGrid?.SelectedItem != null)
            {
                var prop = CofreGrid.SelectedItem.GetType().GetProperty("Id");
                if (prop != null)
                {
                    var val = prop.GetValue(CofreGrid.SelectedItem);
                    if (val != null && int.TryParse(val.ToString(), out var parsedSel))
                        id = parsedSel;
                }
            }

            if (id == null)
            {
                MessageBox.Show("Não foi possível identificar o registro.", "Cofre",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var item = _cofre.FirstOrDefault(x => x.Id == id.Value);
            if (item == null)
            {
                MessageBox.Show("Registro não encontrado.", "Cofre",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var cipher = Convert.FromBase64String(item.SenhaCipherB64);
                var plain = UnprotectToString(cipher);

                MessageBox.Show($"Senha de \"{item.Nome}\":\n\n{plain}",
                    "Exibir senha", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Não foi possível descriptografar a senha.",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
