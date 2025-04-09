using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Hosting;

namespace OrderManagement;

public static class ServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddEventBus(
        this IHostApplicationBuilder builder,
        Action<IBusRegistrationConfigurator>? massTransitConfiguration = null) =>
        AddEventBus<IBus>(builder, massTransitConfiguration);
    
    public static IHostApplicationBuilder AddEventBus<TBus>(
        this IHostApplicationBuilder builder,
        Action<IBusRegistrationConfigurator>? massTransitConfiguration = null)
        where TBus : class, IBus
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.Services.AddMassTransit<TBus>(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.SetInMemorySagaRepositoryProvider();

            var entryAssembly = Assembly.GetEntryAssembly();
            x.AddSagaStateMachines(entryAssembly);
            x.AddSagas(entryAssembly);
            x.AddActivities(entryAssembly);

            massTransitConfiguration?.Invoke(x);

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return builder;
    }
}