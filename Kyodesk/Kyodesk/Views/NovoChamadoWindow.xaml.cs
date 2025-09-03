using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SistemaChamadosWpf.Data;
using SistemaChamadosWpf.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SistemaChamadosWpf.Views
{
    /// <summary>
    /// Code-behind da janela de novo chamado.
    /// </summary>
    public partial class NovoChamadoWindow : Window
    {
        private readonly AppDbContext _context;
        private readonly Usuario? _usuario;   // pode ser nulo no designer

        /// <summary>
        /// Construtor usado pelo designer do VS. Em runtime, prefira o outro.
        /// </summary>
        public NovoChamadoWindow()
        {
            InitializeComponent();

            // Tenta resolver o DbContext via DI (se App.Services existir).
            var sp = (Application.Current as App)?.Services;
            if (sp == null)
                throw new InvalidOperationException("ServiceProvider (App.Services) não disponível.");

            _context = sp.GetRequiredService<AppDbContext>();
        }

        /// <summary>
        /// Construtor correto para uso em runtime.
        /// </summary>
        public NovoChamadoWindow(Usuario usuario) : this()
        {
            _usuario = usuario ?? throw new ArgumentNullException(nameof(usuario));
        }

        private async void Salvar_Click(object sender, RoutedEventArgs e)
        {
            // 1) Ler dos controles
            var titulo = TituloTextBox.Text?.Trim();
            var descricao = DescricaoTextBox.Text?.Trim();
            var solicitante = SolicitanteTextBox.Text?.Trim();
            var classificacao = (ClassificacaoComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            var categoria = (CategoriaComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();

            // 2) Validação mínima
            if (string.IsNullOrWhiteSpace(titulo) ||
                string.IsNullOrWhiteSpace(descricao) ||
                string.IsNullOrWhiteSpace(solicitante))
            {
                MessageBox.Show("Preencha Título, Descrição e Solicitante.", "Campos obrigatórios",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_usuario == null)
            {
                MessageBox.Show("Usuário não definido para criar o chamado.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 3) Montar entidade
            var novoChamado = new Chamado
            {
                Titulo = titulo!,
                Descricao = descricao!,
                Solicitante = solicitante!,
                Classificacao = classificacao ?? string.Empty,
                Categoria = categoria ?? string.Empty,
                Status = StatusChamado.Aberto,
                DataCriacao = DateTime.Now,
                UsuarioId = _usuario.Id   // criador
                // ResponsavelId = null (opcional)
            };

            // 4) Persistir
            _context.Chamados.Add(novoChamado);
            await _context.SaveChangesAsync();

            // 5) Fechar informando sucesso
            DialogResult = true;
            Close();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
