using Astra.Data.SqlServer.Repository;
using Astra.Domain.Abstractions.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Astra.Data.SqlServer.Extensions
{
    internal class SqlBuilder : IDisposable
    {
        Action<IConfiguration, AstraSqlOptions>? _configure;

        public SqlBuilder(Action<IConfiguration, AstraSqlOptions> configure)
        {
            _configure = configure;
        }

        public AstraSqlOptions ApplyConfigurations(IConfiguration configuration)
        {
            var options = new AstraSqlOptions { SqlConnectionString = string.Empty };
            _configure?.Invoke(configuration, options);
            return options;
        }

        public void Dispose()
        {
            _configure = null;
        }
    }

    public class AstraSqlOptions
    {
        public required string SqlConnectionString { get; set; }
    }

    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddSqlServerRepositories(this IServiceCollection services, Action<IConfiguration, AstraSqlOptions> configure)
        {
            services.AddSingleton(new SqlBuilder(configure));
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            return services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var builder = sp.GetRequiredService<SqlBuilder>();
                var astraOptions = builder.ApplyConfigurations(configuration);
                options.UseSqlServer(astraOptions.SqlConnectionString);
            });
        }
    }
}
