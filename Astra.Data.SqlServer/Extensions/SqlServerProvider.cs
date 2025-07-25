using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Astra.Data.SqlServer.Extensions
{
    internal class SqlServerProvider : IProviderConfigurator
    {
        private readonly IConfiguration _configuration;
        private readonly SqlBuilder _sqlBuilder;

        public SqlServerProvider(IConfiguration configuration, SqlBuilder sqlBuilder)
        {
            _configuration = configuration;
            _sqlBuilder = sqlBuilder;
        }

        public void ConfigureDbContext(DbContextOptionsBuilder optionsBuilder)
        {
            var options = _sqlBuilder.ApplyConfigurations(_configuration);
            optionsBuilder.UseSqlServer(options.SqlConnectionString);
        }
    }
}
