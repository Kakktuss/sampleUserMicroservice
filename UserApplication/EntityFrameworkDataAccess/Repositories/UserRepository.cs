using System.Threading.Tasks;
using BuildingBlock.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;
using UserDomain.AggregateModels.Users;

namespace UserApplication.EntityFrameworkDataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserContext _context;

        private readonly DbSet<User> _users;

        public UserRepository(UserContext context)
        {
            _context = context;

            _users = _context.Set<User>();
        }

        public IUnitOfWork UnitOfWork => _context;

        public void Add(User user)
        {
            _users.Add(user);
        }

        public Task<bool> ExistsByUuidAsync(string uuid)
        {
            return _users.AnyAsync(e => e.Uuid == uuid);
        }

        public Task<User> FindByUuidAsync(string uuid)
        {
            return _users.FirstOrDefaultAsync(e => e.Uuid == uuid);
        }
    }
}