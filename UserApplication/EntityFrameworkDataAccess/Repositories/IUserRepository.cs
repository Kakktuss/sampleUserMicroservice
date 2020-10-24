using System.Threading.Tasks;
using BuildingBlock.DataAccess.Abstractions;
using UserApplication.Models;

namespace UserApplication.EntityFrameworkDataAccess.Repositories
{
    public interface IUserRepository : IRepository
    {
        public void Add(User user);

        public void Remove(User user);

        public void Update(User user);

        public Task<bool> ExistsByUuidAsync(string uuid);

        public Task<bool> ExistsByUsernameAsync(string username);

        public Task<bool> ExistsByEmailAsync(string email);

        public Task<User> FindByUuidAsync(string uuid);
    }
}