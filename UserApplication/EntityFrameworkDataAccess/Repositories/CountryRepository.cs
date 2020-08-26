using System;
using System.Threading.Tasks;
using BuildingBlock.DataAccess.Abstractions;
using UserApplication.Models;

namespace UserApplication.EntityFrameworkDataAccess.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        public IUnitOfWork UnitOfWork { get; }

        public void Add(Country country)
        {
            throw new NotImplementedException();
        }

        public void Remove(Country country)
        {
            throw new NotImplementedException();
        }

        public Task<Country> FindByUuidAsync(Guid uuid)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsByUuidAsync(Guid uuid)
        {
            throw new NotImplementedException();
        }
    }
}