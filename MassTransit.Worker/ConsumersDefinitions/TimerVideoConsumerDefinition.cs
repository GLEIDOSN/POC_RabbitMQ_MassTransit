using MassTransit.Core.Events;
using MassTransit.Worker.Consumers;

namespace MassTransit.Worker.ConsumersDefinitions
{
    public class TimerVideoConsumerDefinition : ConsumerDefinition<TimeVideoJobConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, 
            IConsumerConfigurator<TimeVideoJobConsumer> consumerConfigurator,
            IRegistrationContext context)
        {
            consumerConfigurator.Options<JobOptions<ConvertVideoEvent>>(options =>
                options.SetRetry(r => r.Interval(3, TimeSpan.FromSeconds(30))).SetJobTimeout(TimeSpan.FromMinutes(1)).SetConcurrentJobLimit(10));

            endpointConfigurator.UseInMemoryOutbox(context);
        }
    }
}
