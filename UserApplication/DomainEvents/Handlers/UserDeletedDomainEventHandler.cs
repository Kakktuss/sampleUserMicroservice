using System.Threading;
using System.Threading.Tasks;
using BuildingBlock.Bus.Abstractions.Nats.Events;
using MediatR;
using UserApplication.EntityFrameworkDataAccess.Repositories;
using UserApplication.IntegrationEvents.Events.User;
using UserDomain.AggregateModels.Users.Events;

namespace UserApplication.DomainEvents.Handlers
{
    public class UserDeletedDomainEventHandler : INotificationHandler<UserCreatedDomainEvent>
    {
        private readonly INatsIntegrationEventBus _natsIntegrationEventBus;
        private readonly IUserRepository _userRepository;

        public UserDeletedDomainEventHandler(IUserRepository userRepository,
            INatsIntegrationEventBus natsIntegrationEventBus)
        {
            _userRepository = userRepository;

            _natsIntegrationEventBus = natsIntegrationEventBus;
        }

        public async Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindByUuidAsync(notification.User.Uuid);

            if (user == null)
                return;

            var userCreatedIntegrationEvent = new UserDeletedIntegrationEvent(user.Uuid);

            _natsIntegrationEventBus.Publish("user", "deleted", userCreatedIntegrationEvent);
        }
    }
}