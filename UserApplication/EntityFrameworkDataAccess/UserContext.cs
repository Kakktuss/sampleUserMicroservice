using System.Threading;
using System.Threading.Tasks;
using BuildingBlock.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;
using UserApplication.EntityFrameworkDataAccess.Configurations;
using UserApplication.Models;

namespace UserApplication.EntityFrameworkDataAccess
{
    public class UserContext : DbContext, IUnitOfWork
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());

            modelBuilder.ApplyConfiguration(new CountryEntityTypeConfiguration());
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            var result = await SaveChangesAsync(cancellationToken);

            return result != 0;
        }
    }
}