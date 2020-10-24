using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserApplication.Models;

namespace UserApplication.EntityFrameworkDataAccess.Configurations
{
    public class CountryEntityTypeConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.ToTable("Countries");

            builder.Property(e => e.Uuid)
                .IsRequired();

            builder.HasIndex(e => e.Uuid)
                .IsUnique()
                .HasName("UIX_Countries_Uuid");
        }
    }
}