using System;
using BuildingBlock.Bus.Events;

namespace UserApplication.IntegrationEvents.Events.Country
{
    public class CountryDeletedIntegrationEvent : IntegrationEvent
    {
        public Guid CountryUuid { get; set; }
    }
}