using System;
using BuildingBlock.Bus.Abstractions.Events;
using BuildingBlock.Bus.Events;

namespace UserApplication.IntegrationEvents.Events.User
{
    public class UserCompletedIntegrationEvent : IntegrationEvent
    {

        public UserCompletedIntegrationEvent(string userUuid,
            Guid countryUuid)
        {
            UserUuid = userUuid;

            CountryUuid = countryUuid;
        } 
        
        public string UserUuid { get; }
        
        public Guid CountryUuid { get; }
        
    }
}