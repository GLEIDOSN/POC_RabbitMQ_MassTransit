using MassTransit;
using MassTransit.Core.Events;
using MassTransit.Core.Extensions;
using MassTransit.Core.Models;
using MassTransit.Worker.Consumers;
using MassTransit.Worker.ConsumersDefinitions;
using Microsoft.AspNetCore.Builder;
using RabbitMQ.Client;
using Serilog;
using System.Security.Authentication;

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.AddSerilog("Worker MassTransit");
    Log.Information("Starting Worker");

    var host = Host.CreateDefaultBuilder(args)
        .UseSerilog(Log.Logger)
        .ConfigureServices((context, collection) =>
        {
            var rabbitMqCfg = context.Configuration
                .GetSection("RabbitMqConnection")
                .Get<RabbitMqConnection>();
            var projectRootPath = Directory.GetCurrentDirectory(); // This is the current path of your application

            collection.AddHttpContextAccessor();

            collection.AddMassTransit(x =>
            {
                x.AddDelayedMessageScheduler();
                x.AddConsumer<QueueNFeInsertedConsumer>(typeof(QueueNfeConsumerDefinition));
                x.AddConsumer<QueueVerifyStatusNfeConsumer>(typeof(QueueVerifyStatusNfeConsumerDefinition));

                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq((ctx, cfg) =>
                {
                    Console.WriteLine("Hostname: {0}", rabbitMqCfg.HostName);
                    Console.WriteLine("Username: {0}", rabbitMqCfg.UserName);
                    Console.WriteLine("Password: {0}", rabbitMqCfg.Password);
                    Console.WriteLine("PortSSL: {0}", rabbitMqCfg.PortSSL);

                    cfg.Host(rabbitMqCfg.HostName, (rabbitMqCfg.UseSSL ? rabbitMqCfg.PortSSL : rabbitMqCfg.Port), "/", h =>
                    {
                        h.Username(rabbitMqCfg.UserName);
                        h.Password(rabbitMqCfg.Password);

                        if (rabbitMqCfg.UseSSL)
                        {
                            h.UseSsl(s =>
                            {
                                var certificatePath = Path.Combine(projectRootPath, rabbitMqCfg.CertificatePath);

                                Console.WriteLine("Path: {0}", certificatePath);
                                Console.WriteLine("File: {0}", rabbitMqCfg.CertificatePath);

                                s.ServerName = rabbitMqCfg.HostName;
                                s.CertificatePath = certificatePath;
                                s.CertificatePassphrase = rabbitMqCfg.CertificatePassphrase;
                                s.Protocol = SslProtocols.Tls13;
                            });
                        }
                    });

                    cfg.UseDelayedMessageScheduler();

                    cfg.Message<VerifyStatusNFeEvent>(x => x.SetEntityName("medical_reports.retry.exchange"));
                    cfg.Publish<VerifyStatusNFeEvent>(x =>
                    {
                        x.ExchangeType = ExchangeType.Direct;
                        x.BindQueue(
                            "medical_reports.retry.exchange",
                            "medical_reports.retry.xpto",
                            config =>
                            {
                                config.ExchangeType = ExchangeType.Direct;
                            });
                    });

                    cfg.SendTopology.ErrorQueueNameFormatter = new ErrorNameFormater(
                                queueErrorBinds: new Dictionary<string, string>() {
                                    //{ "medical_reports.created.xpto", "medical_reports.retry.exchange"},
                                    { "medical_reports.retry.xpto", "medical_reports.failed.xpto"},
                                });

                    cfg.ReceiveEndpoint("medical_reports.created.xpto", e =>
                    {
                        e.ExchangeType = ExchangeType.Direct;
                        e.ConfigureConsumeTopology = false;
                        e.PublishFaults = false;
                        e.BindQueue = true;
                        e.Bind("medical_reports.direct.exchange", config =>
                        {
                            config.ExchangeType = ExchangeType.Direct;
                        });
                        e.ConfigureConsumer<QueueNFeInsertedConsumer>(ctx);
                    });

                    cfg.ReceiveEndpoint("medical_reports.retry.xpto", e =>
                    {
                        e.ExchangeType = ExchangeType.Direct;
                        e.ConfigureConsumeTopology = false;
                        e.PublishFaults = false;
                        e.BindQueue = true;
                        e.Bind("medical_reports.retry.exchange", config =>
                        {
                            config.ExchangeType = ExchangeType.Direct;
                        });
                        //e.BindDeadLetterQueue("medical_reports.failed.exchange");
                        e.UseMessageRetry(config =>
                        {
                            config.Incremental(3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
                        });
                        e.ConfigureConsumer<QueueVerifyStatusNfeConsumer>(ctx);
                    });

                });
            });
        })
        .Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.Information("Server Shutting down...");
    Log.CloseAndFlush();
}