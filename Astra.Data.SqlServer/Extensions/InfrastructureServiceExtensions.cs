using Astra.Data.SqlServer.Repository;
using Astra.Domain.Abstractions.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Astra.Data.SqlServer.Extensions
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddSqlServerRepositories(this IServiceCollection services, Action<IConfiguration, AstraSqlOptions> configure)
        {
            services.AddSingleton(new SqlBuilder(configure));
            services.AddSingleton<IProviderConfigurator, SqlServerProvider>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            return services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                var dbContextProvider = sp.GetRequiredService<IProviderConfigurator>();
                dbContextProvider.ConfigureDbContext(options);
            });
        }
    }
}
