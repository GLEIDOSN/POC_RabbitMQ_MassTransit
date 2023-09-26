using MassTransit.Core.Enums;
using MassTransit.Core.Events;
using MassTransit.Metadata;
using System.Diagnostics;

namespace MassTransit.Worker.Consumers
{
    public class QueueNFeInsertedConsumer : IConsumer<NFeInsertedEvent>
    {
        private readonly ILogger<QueueNFeInsertedConsumer> _logger;

        public QueueNFeInsertedConsumer(ILogger<QueueNFeInsertedConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<NFeInsertedEvent> context)
        {
            var timer = Stopwatch.StartNew();

            try
            {
                var nfe = new NFeInsertedEvent(context.Message.Id, context.Message.ClientName, context.Message.Date, context.Message.Total, context.Message.Status);

                if (context.Message.Status == EnumStatusNFe.Pending)
                {
                    await context.Publish(new VerifyStatusNFeEvent(nfe.Id, nfe.Status));
                }

                _logger.LogInformation($"Receive NFe: {nfe.Id} - {nfe.ClientName}");
                await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<NFeInsertedEvent>.ShortName);
            }
            catch (Exception ex)
            {
                await context.NotifyFaulted(timer.Elapsed, TypeMetadataCache<NFeInsertedEvent>.ShortName, ex);
            }
        }
    }
}
