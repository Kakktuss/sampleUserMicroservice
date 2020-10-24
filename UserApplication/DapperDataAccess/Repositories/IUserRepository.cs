using System.Collections.Generic;
using System.Threading.Tasks;
using UserApplication.ViewModels;

namespace UserApplication.DapperDataAccess.Repositories
{
    public interface IUserRepository
    {
        public Task<IEnumerable<UserViewModel>> GetUsersAsync();

        public Task<UserViewModel> FindUserByUuidAsync(string uuid);
    }
}