using Astra.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Astra.Data.SqlServer
{
    public class CityConfiguration : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.ToTable("City");
            builder.HasKey(k => k.Id);
            builder.Property(p => p.Name).IsRequired();
            builder.HasOne(fk => fk.Country).WithMany();
        }
    }
}
