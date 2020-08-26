using BuildingBlock.Bus.Events;

namespace UserApplication.IntegrationEvents.Events.User
{
    public class UserCreatedIntegrationEvent : IntegrationEvent
    {
        public UserCreatedIntegrationEvent(string userUuid,
            string userName,
            string userEmail)
        {
            UserUuid = userUuid;

            UserName = userName;

            UserEmail = userEmail;
        }

        public string UserUuid { get; }

        public string UserName { get; }

        public string UserEmail { get; }
    }
}