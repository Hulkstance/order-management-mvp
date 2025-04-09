using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderManagement;
using OrderManagement.Sagas;

var builder = Host.CreateApplicationBuilder(args);

builder.AddEventBus();

var host = builder.Build();

await host.StartAsync();

var bus = host.Services.GetRequiredService<IBus>();

var orderId = Guid.NewGuid();
var userId = Guid.NewGuid();

await bus.Publish(new OrderSubmitted
{
    OrderId = orderId,
    SubmittedAt = DateTime.UtcNow,
    UserId = userId
});

await Task.Delay(100);

await bus.Publish(new OrderPlaced
{
    OrderId = orderId,
    PlacedAt = DateTime.UtcNow,
    UserId = userId
});

await Task.Delay(100);

await bus.Publish(new OrderFilled
{
    OrderId = orderId,
    FilledAt = DateTime.UtcNow,
    UserId = userId
});

await Task.Delay(100);

await bus.Publish(new OrderCancelled
{
    OrderId = orderId,
    CancelledAt = DateTime.UtcNow,
    UserId = userId
});

Console.ReadLine();

await host.StopAsync();