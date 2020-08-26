using System.Threading;
using System.Threading.Tasks;
using BuildingBlock.Bus.Abstractions.Nats.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using UserApplication.EntityFrameworkDataAccess.Repositories;
using UserApplication.IntegrationEvents.Events.User;
using UserDomain.AggregateModels.Users.Events;

namespace UserApplication.DomainEvents.Handlers
{
    public class UserCreatedDomainEventHandler : INotificationHandler<UserCreatedDomainEvent>
    {
        private readonly ILogger<UserCreatedDomainEventHandler> _logger;

        private readonly INatsIntegrationEventBus _natsIntegrationEventBus;
        private readonly IUserRepository _userRepository;

        public UserCreatedDomainEventHandler(IUserRepository userRepository,
            INatsIntegrationEventBus natsIntegrationEventBus,
            ILogger<UserCreatedDomainEventHandler> logger)
        {
            _userRepository = userRepository;

            _natsIntegrationEventBus = natsIntegrationEventBus;

            _logger = logger;
        }

        public async Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogTrace("[UserCreatedDomainEvent] Starting processing the domain event");

            var user = await _userRepository.FindByUuidAsync(notification.User.Uuid);

            if (user == null)
            {
                _logger.LogInformation("[UserCreatedDomainEvent] End: The user is not found");

                return;
            }

            var userCreatedIntegrationEvent = new UserCreatedIntegrationEvent(user.Uuid,
                user.Username,
                user.Email);

            _logger.LogTrace("[UserCreatedDomainEvent] Domain event processed successfully");

            _natsIntegrationEventBus.Publish("user", "created", userCreatedIntegrationEvent);
        }
    }
}