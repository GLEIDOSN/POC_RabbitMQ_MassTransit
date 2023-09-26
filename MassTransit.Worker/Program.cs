using MassTransit;
using MassTransit.Core;
using MassTransit.Core.Events;
using MassTransit.Core.Extensions;
using MassTransit.Worker.Consumers;
using MassTransit.Worker.ConsumersDefinitions;
using Microsoft.AspNetCore.Builder;
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
                x.AddConsumer<TimeVideoJobConsumer>(typeof(TimerVideoConsumerDefinition));
                x.AddConsumer<QueueNFeInsertedConsumer>(typeof(QueueNfeConsumerDefinition));
                x.AddConsumer<QueueVerifyStatusNfeConsumer>(typeof(QueueVerifyStatusNfeConsumerDefinition));
                x.AddRequestClient<ConvertVideoEvent>();

                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(appSettings.connectionStringsRabbitMq);
                    cfg.UseDelayedMessageScheduler();
                    cfg.ServiceInstance(instance =>
                    {
                        instance.ConfigureJobServiceEndpoints();
                        instance.ConfigureEndpoints(ctx, new KebabCaseEndpointNameFormatter("dev", false));
                    });
                    cfg.Publish<VerifyStatusNFeEvent>(x =>
                    {
                        x.Durable = true;
                        x.AutoDelete = true;
                        x.ExchangeType = "direct";
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