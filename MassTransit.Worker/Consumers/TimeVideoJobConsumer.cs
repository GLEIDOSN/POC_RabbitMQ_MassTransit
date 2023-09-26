using MassTransit.Core.Events;

namespace MassTransit.Worker.Consumers
{
    public class TimeVideoJobConsumer : IJobConsumer<ConvertVideoEvent>
    {
        public async Task Run(JobContext<ConvertVideoEvent> context)
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
        }
    }
}
