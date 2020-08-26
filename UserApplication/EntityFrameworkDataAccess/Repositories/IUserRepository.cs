using System.Threading.Tasks;
using BuildingBlock.DataAccess.Abstractions;
using UserDomain.AggregateModels.Users;

namespace UserApplication.EntityFrameworkDataAccess.Repositories
{
    public interface IUserRepository : IRepository
    {
        public void Add(User user);

        public Task<bool> ExistsByUuidAsync(string uuid);

        public Task<User> FindByUuidAsync(string uuid);
    }
}