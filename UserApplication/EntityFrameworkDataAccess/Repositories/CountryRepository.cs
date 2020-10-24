using System;
using System.Threading.Tasks;
using BuildingBlock.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;
using UserApplication.Models;

namespace UserApplication.EntityFrameworkDataAccess.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly UserContext _context;

        private readonly DbSet<Country> _countries;

        public CountryRepository(UserContext context)
        {
            _context = context;

            _countries = _context.Set<Country>();
        }
        
        public IUnitOfWork UnitOfWork => _context;

        public void Add(Country country)
        {
            _countries.Add(country);
        }

        public void Remove(Country country)
        {
            _countries.Remove(country);
        }

        public Task<Country> FindByUuidAsync(Guid uuid)
        {
            return _countries.FirstOrDefaultAsync(e => e.Uuid == uuid);
        }

        public Task<bool> ExistsByUuidAsync(Guid uuid)
        {
            return _countries.AnyAsync(e => e.Uuid == uuid);
        }
    }
}