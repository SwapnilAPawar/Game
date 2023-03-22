using System.Reflection;
using Game.Common.Settings;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Game.Common.MassTransit
{
    public static class Extensions
    {
        public static IServiceCollection AddMassTransitWithRabbitMQ(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddMassTransit(configure =>
            {
                //Registered Assembly that will have all the consumers defined. It will be which ever assembly that is invoking this class.
                configure.AddConsumers(Assembly.GetEntryAssembly());

                configure.UsingRabbitMq((context, configurator) =>
                {
                    var configuration = context.GetService<IConfiguration>();
                    var serviceSettings = configuration!.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                    var rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                    configurator.Host(rabbitMQSettings!.Host);
                    configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings!.ServiceName, false));
                });
            });

            //to start MassTransit service
            // builder.Services.AddMassTransitHostedService();
            // serviceCollection.AddOptions<MassTransitHostOptions>()
            // .Configure(options =>
            // {
            //     // if specified, waits until the bus is started before
            //     // returning from IHostedService.StartAsync
            //     // default is false
            //     options.WaitUntilStarted = true;

            //     // if specified, limits the wait time when starting the bus
            //     options.StartTimeout = TimeSpan.FromSeconds(10);

            //     // if specified, limits the wait time when stopping the bus
            //     options.StopTimeout = TimeSpan.FromSeconds(30);
            // });
            return serviceCollection;
        }
    }
}