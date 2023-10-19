using MassTransit.Core.Events;
using MassTransit.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System.Security.Authentication;

namespace MassTransit.Core.Extensions
{
    public static class MasstransitExtension
    {
        public static void AddMassTransitExtension(this IServiceCollection services, IConfiguration configuration)
        {
            var projectRootPath = Directory.GetCurrentDirectory(); // Este é o diretório atual do seu aplicativo

            services.AddMassTransit(x =>
            {
                x.AddDelayedMessageScheduler();
                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq((ctx, cfg) =>
                {
                    var rabbitMqCfg = configuration.GetSection("RabbitMqConnection").Get<RabbitMqConnection>();

                    cfg.Host(rabbitMqCfg.HostName, (rabbitMqCfg.UseSSL ? rabbitMqCfg.PortSSL : rabbitMqCfg.Port), "/", h =>
                    {
                        h.Username(rabbitMqCfg.UserName);
                        h.Password(rabbitMqCfg.Password);

                        if (rabbitMqCfg.UseSSL)
                        {
                            h.UseSsl(s =>
                            {
                                var certificatePath = Path.Combine(projectRootPath, rabbitMqCfg.CertificatePath);

                                s.ServerName = rabbitMqCfg.HostName;
                                s.CertificatePath = certificatePath;
                                s.CertificatePassphrase = rabbitMqCfg.CertificatePassphrase;
                                s.Protocol = SslProtocols.Tls13;
                            });
                        }
                    });

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
                });
            });
        }
    }
}
