using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Astra.Data.GraphQl.Extensions
{
    public static class QueryableRegistrationExtensions
    {
        static MethodInfo DbSetDelegate;

        public static IServiceCollection AddDbSetQueryableFactories<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
        {
            DbSetDelegate = typeof(DbContext).GetMethod("Set", Type.EmptyTypes);
            var dbContextType = typeof(TDbContext);
            var entityTypes = dbContextType.Assembly.GetConfiguredEntityTypes();
            var instance = (DbContext)Activator.CreateInstance(dbContextType, new DbContextOptions<TDbContext>());

            foreach (var entityType in entityTypes)
            {
                var factoryType = typeof(IQueryable<>).MakeGenericType(entityType);
                services.AddScoped(factoryType, CreateFactory(dbContextType, entityType));
            }
            return services;
        }

        private static IEnumerable<Type> GetConfiguredEntityTypes(this Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
                .Select(i => i.GetGenericArguments()[0])
                )
                .Distinct();
        }

        private static Func<IServiceProvider, object> CreateFactory(Type dbContextType, Type entityType)
        {
            return (serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService(dbContextType);
                var method = DbSetDelegate?.MakeGenericMethod(entityType);
                var dbSet = method?.Invoke(context, null);
                return dbSet;
            };
        }
    }
}
