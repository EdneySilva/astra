using Astra.Data.SqlServer;
using Astra.Data.SqlServer.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Astra.Api.Tests.Factories
{
    class InMemoryProvider : IProviderConfigurator
    {
        public void ConfigureDbContext(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("InMemoryTestDb");
        }
    }

    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove contexto real
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);
                var descriptor2 = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IProviderConfigurator));
                if (descriptor2 != null)
                    services.Remove(descriptor2);
                services.AddSingleton<IProviderConfigurator, InMemoryProvider>();
                // Adiciona contexto InMemory
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryTestDb");
                });

                // Garante que banco foi criado
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();
            });
        }
    }

}
