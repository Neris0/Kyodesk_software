using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Kyodesk.Views
{
    /// <summary>
    /// Lógica interna para NovoArtigoWindow.xaml
    /// </summary>
    public partial class NovoArtigoWindow : Window
    {
        public NovoArtigoWindow()
        {
            InitializeComponent();
            TxtTitulo.Focus();
        }

        // Exponha os dados preenchidos para quem abriu a janela
        public string NovoArtigoTitulo => (TxtTitulo.Text ?? "").Trim();
        public string NovoArtigoCategoria => (CmbCategoria.Text ?? "").Trim();
        public string NovoArtigoConteudo => (TxtConteudo.Text ?? "").Trim();

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // fecha sem salvar
        }

        private void Salvar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NovoArtigoTitulo))
            {
                MessageBox.Show("Informe um título.", "Novo Artigo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtTitulo.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(NovoArtigoConteudo))
            {
                MessageBox.Show("Informe o conteúdo do artigo.", "Novo Artigo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtConteudo.Focus();
                return;
            }

            DialogResult = true; // fecha e sinaliza sucesso
        }
    }
}
