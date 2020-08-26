using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserDomain.AggregateModels.Users;

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
        }
    }
}