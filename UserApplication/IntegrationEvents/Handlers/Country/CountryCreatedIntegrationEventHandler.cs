using System.Threading.Tasks;
using BuildingBlock.Bus.Abstractions.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserApplication.EntityFrameworkDataAccess.Repositories;
using UserApplication.IntegrationEvents.Events.Country;

namespace UserApplication.IntegrationEvents.Handlers.Country
{
    public class CountryCreatedIntegrationEventHandler : IIntegrationEventHandler<CountryCreatedIntegrationEvent>
    {
        private readonly ICountryRepository _countryRepository;

        private readonly ILogger<CountryCreatedIntegrationEventHandler> _logger;

        public CountryCreatedIntegrationEventHandler(ICountryRepository countryRepository,
            ILogger<CountryCreatedIntegrationEventHandler> logger)
        {
            _countryRepository = countryRepository;

            _logger = logger;
        }

        public async Task<bool> Handle(CountryCreatedIntegrationEvent @event)
        {
            // Check if the country already exists
            if (await _countryRepository.ExistsByUuidAsync(@event.CountryUuid))
            {
                return true;
            }

            // Add the country
            _countryRepository.Add(
                new Models.Country(@event.CountryUuid));

            try
            {
                // Save changes into the database
                var result = await _countryRepository.UnitOfWork.SaveEntitiesAsync();

                if (!result)
                {
                    _logger.LogInformation(
                        $"[CountryCreatedIntegrationEvent] End: An error happened while trying to add the country {@event.CountryUuid} in the database");

                    return false;
                }
            }
            catch (DbUpdateException e)
            {
                _logger.LogError(
                    $"[CountryCreatedIntegrationEvent] Error: An error happened while trying to add the country {@event.CountryUuid} in the database",
                    e);
                
                return false;
            }
            
            _logger.LogTrace("[CountryCreatedIntegrationEvent] Integration event processed successfully.");

            return true;
        }
    }
}