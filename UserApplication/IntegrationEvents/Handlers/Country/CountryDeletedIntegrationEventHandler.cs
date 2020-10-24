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

        public CountryDeletedIntegrationEventHandler(ICountryRepository countryRepository,
            ILogger<CountryDeletedIntegrationEventHandler> logger)
        {
            _countryRepository = countryRepository;

            _logger = logger;
        }

        public async Task<bool> Handle(CountryDeletedIntegrationEvent @event)
        {
            _logger.LogTrace("[CountryDeletedIntegrationEvent] Starting processing the integration event.");
            
            // Check if the country exists
            if (!await _countryRepository.ExistsByUuidAsync(@event.CountryUuid))
            {
                return true;
            }

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
                    _logger.LogInformation(
                        $"[CountryDeletedIntegrationEvent] Error: An error happened while trying to remove the country {@event.CountryUuid} in the database");

                    return false;
                }
            }
            catch (DbUpdateException e)
            {
                _logger.LogError(
                    $"[CountryDeletedIntegrationEvent] Error: An error happened while trying to remove the country {@event.CountryUuid} in the database", e);

                return false;
            }
            
            _logger.LogTrace("[CountryDeletedIntegrationEvent] Integration event processed successfully.");

            return true;
        }
    }
}