using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserApplication.Models;

namespace UserApplication.EntityFrameworkDataAccess.Configurations
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .UseIdentityColumn()
                .Metadata
                .SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

            builder.HasIndex(e => e.Uuid)
                .IsUnique();

            builder.HasIndex(e => e.Uuid)
                .IsUnique()
                .HasName("UIX_Users_Uuid");
            
            builder.Property(e => e.Username)
                .IsRequired();
            
            builder.HasIndex(e => e.Username)
                .IsUnique()
                .HasName("UIX_Users_Username");

            builder.Property(e => e.Email)
                .IsRequired();
            
            builder.HasIndex(e => e.Email)
                .IsUnique()
                .HasName("UIX_Users_Email");

            builder.Property(e => e.IsCompleted);

            builder.HasOne(e => e.Country)
                .WithMany(e => e.Users)
                .HasForeignKey(e => e.CountryId);

            builder.HasIndex(e => e.CountryId)
                .HasName("IX_Users_CountryId");
        }
    }
}