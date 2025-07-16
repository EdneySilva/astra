using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Astra.Data.SqlServer.Extensions
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddSqlServerRepositories(this IServiceCollection services)
        {

            return services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {

            });
        }

    }
}
