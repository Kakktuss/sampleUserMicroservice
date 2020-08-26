using BuildingBlock.Bus.Events;

namespace UserApplication.IntegrationEvents.Events.User
{
    public class UserDeletedIntegrationEvent : IntegrationEvent
    {
        public UserDeletedIntegrationEvent(string userUuid)
        {
            UserUuid = userUuid;
        }

        public string UserUuid { get; }
    }
}