using MassTransit.Worker.Consumers;

namespace MassTransit.Worker.ConsumersDefinitions
{
    public class QueueNfeConsumerDefinition : ConsumerDefinition<QueueNFeInsertedConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, 
            IConsumerConfigurator<QueueNFeInsertedConsumer> consumerConfigurator,
            IRegistrationContext context)
        {
            consumerConfigurator.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(3)));
            endpointConfigurator.UseInMemoryOutbox(context);
        }
    }
}
