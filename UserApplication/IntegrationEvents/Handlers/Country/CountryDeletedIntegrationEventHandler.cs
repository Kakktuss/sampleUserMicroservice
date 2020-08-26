using System.Threading.Tasks;
using BuildingBlock.Bus.Abstractions.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserApplication.EntityFrameworkDataAccess.Repositories;
using UserApplication.IntegrationEvents.Events.Country;

namespace UserApplication.IntegrationEvents.Handlers.Country
{
    public class CountryDeletedIntegrationEventHandler : IIntegrationEventHandler<CountryDeletedIntegrationEvent>
    {
        private readonly ICountryRepository _countryRepository;

        private readonly ILogger<CountryDeletedIntegrationEventHandler> _logger;

        public CountryDeletedIntegrationEventHandler(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task Handle(CountryDeletedIntegrationEvent @event)
        {
            // Check if the country exists
            if (!await _countryRepository.ExistsByUuidAsync(@event.CountryUuid)) return;

            // Retrieve the country by it's uuid
            var country = await _countryRepository.FindByUuidAsync(@event.CountryUuid);

            // Remove the country
            _countryRepository.Remove(country);

            try
            {
                // Save changes into the database
                var result = await _countryRepository.UnitOfWork.SaveEntitiesAsync();

                if (!result)
                {
                }
            }
            catch (DbUpdateException e)
            {
            }
        }
    }
}