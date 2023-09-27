using MassTransit.Core.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace MassTransit.Core.Extensions
{
    public static class MasstransitExtension
    {
        public static void AddMassTransitExtension(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.AddDelayedMessageScheduler();
                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(configuration.GetSection("ConnectionStringsRabbitMq:RabbitMq").Value);

                    cfg.UseDelayedMessageScheduler();

                    cfg.Message<NFeInsertedEvent>(x => x.SetEntityName("medical_reports.direct.exchange"));
                    cfg.Publish<NFeInsertedEvent>(x =>
                    {
                        x.ExchangeType = ExchangeType.Direct;
                        x.BindQueue(
                            "medical_reports.direct.exchange",
                            "medical_reports.created.xpto",
                            config =>
                            {
                                config.ExchangeType = ExchangeType.Direct;
                            });
                    });

                    //cfg.ConfigureEndpoints(ctx, new KebabCaseEndpointNameFormatter("dev", false));
                    //cfg.UseMessageRetry(retry => { retry.Interval(3, TimeSpan.FromSeconds(5)); });
                });
            });
        }
    }
}
