using Astra.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Astra.Data.SqlServer
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.ToTable("Country");
            builder.HasKey(k => k.Id);
            builder.Property(p => p.Name).IsRequired();
            builder.HasMany(fk => fk.Cities).WithOne().HasPrincipalKey(c => c.Id);
        }
    }
}
