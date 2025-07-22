using Astra.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Astra.Data.SqlServer
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext( DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions)
        {   
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CountryConfiguration());
            modelBuilder.ApplyConfiguration(new CityConfiguration());
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.SaveChangesAsync(cancellationToken);
            var dispatcher = this.GetService<IMediator>();

            var domainEventEntities = ChangeTracker.Entries<DomainObject>().Select(s => s.Entity).ToArray();

            foreach (var entity in domainEventEntities)
            {
                var events = entity.Notify();

                foreach (var domainEvent in events)
                {
                    await dispatcher.Publish(domainEvent);
                }
            }
            return result;
        }
    }

}