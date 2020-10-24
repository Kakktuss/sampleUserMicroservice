using System.Threading.Tasks;
using BuildingBlock.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;
using UserApplication.Models;

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

        public void Update(User user)
        {
            _users.Update(user);
        }
        
        public void Remove(User user)
        {
            _users.Remove(user);
        }

        public Task<bool> ExistsByUuidAsync(string uuid)
        {
            return _users.AnyAsync(e => e.Uuid == uuid);
        }

        public Task<bool> ExistsByUsernameAsync(string username)
        {
            return _users.AnyAsync(e => e.Username == username);
        }

        public Task<bool> ExistsByEmailAsync(string email)
        {
            return _users.AnyAsync(e => e.Email == email);
        }

        public Task<User> FindByUuidAsync(string uuid)
        {
            return _users.FirstOrDefaultAsync(e => e.Uuid == uuid);
        }
    }
}