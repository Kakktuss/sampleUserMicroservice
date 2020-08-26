using System.Threading;
using System.Threading.Tasks;
using BuildingBlock.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;
using UserApplication.EntityFrameworkDataAccess.Configurations;
using UserApplication.Models;
using User = UserDomain.AggregateModels.Users.User;

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
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            var result = await SaveChangesAsync(cancellationToken);

            return result != 0;
        }
    }
}