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
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired();
            builder.Property(c => c.Code).IsRequired();
            builder.Property(c => c.Deleted).IsRequired();
            builder
                .HasMany(c => c.Cities)
                .WithOne(c => c.Country)
                .HasForeignKey("CountryId");
        }
    }
}
