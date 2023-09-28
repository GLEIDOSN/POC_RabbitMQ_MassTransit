using MassTransit;
using MassTransit.Core;
using MassTransit.Core.Events;
using MassTransit.Core.Extensions;
using MassTransit.Core.Models;
using MassTransit.Worker.Consumers;
using MassTransit.Worker.ConsumersDefinitions;
using Microsoft.AspNetCore.Builder;
using RabbitMQ.Client;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.AddSerilog("Worker MassTransit");
    Log.Information("Starting Worker");

    var host = Host.CreateDefaultBuilder(args)
        .UseSerilog(Log.Logger)
        .ConfigureServices((context, collection) =>
        {
            var appSettings = new AppSettings();
            appSettings.connectionStringsRabbitMq = context.Configuration.GetSection("ConnectionStringsRabbitMq:RabbitMq").Value;

            //collection.AddScoped<INfeRepository, NfeRepository>();
            //collection.AddDbContext<MassTransitDbContext>();

            collection.AddHttpContextAccessor();

            collection.AddMassTransit(x =>
            {
                x.AddDelayedMessageScheduler();
                //x.AddConsumer<TimeVideoJobConsumer>(typeof(TimerVideoConsumerDefinition));
                x.AddConsumer<QueueNFeInsertedConsumer>(typeof(QueueNfeConsumerDefinition));
                x.AddConsumer<QueueVerifyStatusNfeConsumer>(typeof(QueueVerifyStatusNfeConsumerDefinition));
                //x.AddRequestClient<ConvertVideoEvent>();

                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(appSettings.connectionStringsRabbitMq);
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
                        e.ConfigureConsumer<QueueNFeInsertedConsumer>(ctx);
                        e.ExchangeType = ExchangeType.Direct;
                        e.BindQueue = false;
                        e.ConfigureConsumeTopology = false;
                        e.PublishFaults = false;
                        e.Bind("medical_reports.direct.exchange", config =>
                        {
                            config.ExchangeType = ExchangeType.Direct;
                        });
                    });

                    cfg.ReceiveEndpoint("medical_reports.retry.xpto", e =>
                    {
                        e.ConfigureConsumer<QueueVerifyStatusNfeConsumer>(ctx);
                        e.ExchangeType = ExchangeType.Direct;
                        e.BindQueue = false;
                        e.ConfigureConsumeTopology = false;
                        e.PublishFaults = false;
                        e.Bind("medical_reports.retry.exchange", config =>
                        {
                            config.ExchangeType = ExchangeType.Direct;
                        });
                        e.UseMessageRetry(config =>
                        {
                            config.Incremental(3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
                        });
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