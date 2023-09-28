using MassTransit.Worker.Consumers;

namespace MassTransit.Worker.ConsumersDefinitions
{
    public class QueueVerifyStatusNfeConsumerDefinition : ConsumerDefinition<QueueVerifyStatusNfeConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<QueueVerifyStatusNfeConsumer> consumerConfigurator,
            IRegistrationContext context)
        {
            consumerConfigurator.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(3)));
            endpointConfigurator.UseInMemoryOutbox(context);
            endpointConfigurator.ConfigureConsumeTopology = false;
            endpointConfigurator.PublishFaults = false;
            //TO-do: Configurar manualmente

        }
    }
}
