using MassTransit.Core.Enums;
using MassTransit.Core.Events;
using MassTransit.Metadata;
using System.Diagnostics;

namespace MassTransit.Worker.Consumers
{
    public class QueueVerifyStatusNfeConsumer : IConsumer<VerifyStatusNFeEvent>
    {
        private readonly ILogger<QueueVerifyStatusNfeConsumer> _logger;

        public QueueVerifyStatusNfeConsumer(ILogger<QueueVerifyStatusNfeConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<VerifyStatusNFeEvent> context)
        {
            var timer = Stopwatch.StartNew();

            var nfeStatus = new VerifyStatusNFeEvent(context.Message.Id, context.Message.Status);

            if (nfeStatus.Status == EnumStatusNFe.Pending)
            {
                throw new ArgumentException("NFe status is not 'Completed' for sending Email.");
            }

            // To-do: To Send email
            _logger.LogInformation($"Email sent: 123 - Client Test");
            await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<VerifyStatusNFeEvent>.ShortName);
        }
    }
}
