using System.Linq;
using System;
using System.IO; // <-- novo
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
// Microsoft.Extensions.Hosting não é necessário quando usamos ServiceCollection
using SistemaChamadosWpf.Data;
using SistemaChamadosWpf.Services;

namespace SistemaChamadosWpf

{
    /// <summary>
    /// App inicializa os serviços necessários, incluindo DbContext e serviços
    /// auxiliares. Utiliza DI manual (ServiceCollection) para WPF.
    /// </summary>
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider = default!;

        public App()
        {
            // Configurar DI manualmente
            var services = new ServiceCollection();

            // >>> Caminho ABSOLUTO do banco (evita múltiplos arquivos em locais diferentes)
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dbFolder = Path.Combine(appData, "Kyodesk");
            Directory.CreateDirectory(dbFolder);
            var dbPath = Path.Combine(dbFolder, "chamados.db");

            // Configurar EF Core com SQLite usando o caminho absoluto
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            // Registrar serviços
            services.AddSingleton<AuthService>();

            // Registrar view models
            services.AddTransient<ViewModels.ChamadosViewModel>();
            services.AddTransient<ViewModels.ChamadoDetalhesViewModel>();

            // Construir provider
            _serviceProvider = services.BuildServiceProvider();

            // Criar banco de dados e aplicar schema
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            try
            {
                // Se existirem migrações e houver pendentes, aplica.
                var temMigracoes = dbContext.Database.GetMigrations().Any();
                var pendentes = dbContext.Database.GetPendingMigrations().Any();

                if (temMigracoes && pendentes)
                {
                    dbContext.Database.Migrate();
                }
                else if (!temMigracoes)
                {
                    // Sem migrações definidas: cria o schema com base nos modelos
                    dbContext.Database.EnsureCreated();
                }
                // (Se tem migrações mas nenhuma pendente, não precisa fazer nada)
            }
            catch
            {
                // Fallback: em qualquer falha, garante que o schema exista
                dbContext.Database.EnsureCreated();
            }

            // Seed de dados iniciais (admin/usuário de teste etc.)
            SeedData.Initialize(dbContext);
        }

        /// <summary>
        /// Provedor de serviços exposto para resolver dependências nas views.
        /// </summary>
        public IServiceProvider Services => _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            // Abrir a janela de login na inicialização
            var login = new Views.LoginWindow();
            login.Show();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
            base.OnExit(e);
        }
    }
}
