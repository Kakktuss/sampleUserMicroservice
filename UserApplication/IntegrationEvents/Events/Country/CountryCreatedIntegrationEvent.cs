using System;
using BuildingBlock.Bus.Events;

namespace UserApplication.IntegrationEvents.Events.Country
{
    public class CountryCreatedIntegrationEvent : IntegrationEvent
    {
        public Guid CountryUuid { get; }

        public string CountryCode { get; }
    }
}