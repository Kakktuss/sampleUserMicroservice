using Autofac;
using BuildingBlock.Bus.Abstractions.Events;
using UserApplication.IntegrationEvents.Events.Country;
using UserApplication.IntegrationEvents.Handlers.Country;

namespace UserApplication
{
    public class IntegrationEventBusAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CountryCreatedIntegrationEventHandler>()
                .As<IIntegrationEventHandler<CountryCreatedIntegrationEvent>>()
                .AsSelf();
            
            builder.RegisterType<CountryDeletedIntegrationEventHandler>()
                .As<IIntegrationEventHandler<CountryDeletedIntegrationEvent>>()
                .AsSelf();
        }
    }
}