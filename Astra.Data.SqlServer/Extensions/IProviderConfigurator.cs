using Microsoft.EntityFrameworkCore;

namespace Astra.Data.SqlServer.Extensions
{
    public interface IProviderConfigurator
    {
        void ConfigureDbContext(DbContextOptionsBuilder optionsBuilder);
    }
}
