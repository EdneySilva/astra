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
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired();
            builder.Property(c => c.Province).IsRequired();
            builder.Property(c => c.Deleted).IsRequired();
            builder
                .HasOne(c => c.Country)
                .WithMany(c => c.Cities)
                .HasForeignKey("CountryId")
                .HasPrincipalKey(c => c.Id);
            builder
                .HasIndex("Name", "Province", "CountryId")
                .IsUnique()
                .HasDatabaseName("UQ_City_Name_Province_Country");
        }
    }
}
