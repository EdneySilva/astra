using Microsoft.Extensions.Configuration;

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
}
