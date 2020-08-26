using System;
using System.Threading.Tasks;
using BuildingBlock.DataAccess.Abstractions;
using UserApplication.Models;

namespace UserApplication.EntityFrameworkDataAccess.Repositories
{
    public interface ICountryRepository : IRepository
    {
        void Add(Country country);

        void Remove(Country country);

        Task<Country> FindByUuidAsync(Guid uuid);

        Task<bool> ExistsByUuidAsync(Guid uuid);
    }
}