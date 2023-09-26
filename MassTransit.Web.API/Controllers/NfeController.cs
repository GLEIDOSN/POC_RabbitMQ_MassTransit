using MassTransit.Core.Events;
using Microsoft.AspNetCore.Mvc;

namespace MassTransit.Web.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NfeController : ControllerBase
    {
        private readonly IPublishEndpoint _publisher;
        private readonly IMessageScheduler _publisherScheduler;
        private readonly ILogger<NfeController> _logger;
        public NfeController(IPublishEndpoint publisher, IMessageScheduler publisherScheduler, ILogger<NfeController> logger)
        {
            _publisher = publisher;
            _publisherScheduler = publisherScheduler;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NFeInsertedEvent insertedEvent)
        {
            await _publisher.Publish(insertedEvent);
            _logger.LogInformation($"Send Nfe inserted: {insertedEvent.Id} - {insertedEvent.ClientName}");

            return Ok();
        }

        [HttpPost("schedule")]
        public async Task<IActionResult> PostSchedule([FromBody] NFeInsertedEvent insertedEvent)
        {
            await _publisherScheduler.SchedulePublish(DateTime.UtcNow + TimeSpan.FromSeconds(10), insertedEvent);

            _logger.LogInformation($"Send Nfe: {insertedEvent.Id} - {insertedEvent.ClientName}");

            return Ok();
        }
    }
}
