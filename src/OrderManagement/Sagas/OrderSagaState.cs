using MassTransit;

namespace OrderManagement.Sagas;

public class OrderSagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = null!;

    public Guid OrderId { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? PlacedAt { get; set; }
    public DateTime? FilledAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime? ExpiredAt { get; set; }
    public DateTime? FailedAt { get; set; }
}

public class OrderSaga : MassTransitStateMachine<OrderSagaState>
{
    public OrderSaga()
    {
        Event(() => OrderSubmitted, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => OrderPlaced, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => OrderFilled, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => OrderCancelled, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => OrderExpired, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => OrderFailed, x => x.CorrelateById(m => m.Message.OrderId));

        InstanceState(x => x.CurrentState);
        
        Initially(
            When(OrderSubmitted)
                .SetSubmissionDetails()
                .SendNotification()
                .TransitionTo(Submitted));
        
        During(Submitted,
            When(OrderPlaced)
                .SetPlacingDetails()
                .SendNotification()
                .TransitionTo(Active),
            When(OrderCancelled)
                .SetCancellationDetails()
                .SendNotification()
                .TransitionTo(Cancelled)
                .Finalize(),
            When(OrderFailed)
                .SetFailureDetails()
                .SendNotification()
                .TransitionTo(Failed)
                .Finalize());

        During(Active,
            When(OrderFilled)
                .SetFillingDetails()
                .SendNotification()
                .TransitionTo(Filled)
                .Finalize(),
            When(OrderCancelled)
                .SetCancellationDetails()
                .SendNotification()
                .TransitionTo(Cancelled)
                .Finalize(),
            When(OrderExpired)
                .SetExpirationDetails()
                .SendNotification()
                .TransitionTo(Expired)
                .Finalize(),
            When(OrderFailed)
                .SetFailureDetails()
                .SendNotification()
                .TransitionTo(Failed)
                .Finalize());
        
        SetCompletedWhenFinalized();
    }
    
    public State Submitted { get; private set; } = null!;
    public State Active { get; private set; } = null!;
    public State Filled { get; private set; } = null!;
    public State Cancelled { get; private set; } = null!;
    public State Expired { get; private set; } = null!;
    public State Failed { get; private set; } = null!;

    public Event<OrderSubmitted> OrderSubmitted { get; private set; } = null!;
    public Event<OrderPlaced> OrderPlaced { get; private set; } = null!;
    public Event<OrderFilled> OrderFilled { get; private set; } = null!;
    public Event<OrderCancelled> OrderCancelled { get; private set; } = null!;
    public Event<OrderExpired> OrderExpired { get; private set; } = null!;
    public Event<OrderFailed> OrderFailed { get; private set; } = null!;
}

public static class OrderSagaExtensions
{
    public static EventActivityBinder<OrderSagaState, OrderSubmitted> SetSubmissionDetails(
        this EventActivityBinder<OrderSagaState, OrderSubmitted> binder)
    {
        return binder.Then(context =>
        {
            Console.WriteLine($"Order Submitted: {context.Message.OrderId}");
            context.Saga.OrderId = context.Message.OrderId;
            context.Saga.SubmittedAt = context.Message.SubmittedAt;
        });
    }
    
    public static EventActivityBinder<OrderSagaState, OrderPlaced> SetPlacingDetails(
        this EventActivityBinder<OrderSagaState, OrderPlaced> binder)
    {
        return binder.Then(context =>
        {
            Console.WriteLine($"Order Placed: {context.Message.OrderId}");
            context.Saga.PlacedAt = context.Message.PlacedAt;
        });
    }
    
    public static EventActivityBinder<OrderSagaState, OrderFilled> SetFillingDetails(
        this EventActivityBinder<OrderSagaState, OrderFilled> binder)
    {
        return binder.Then(context =>
        {
            Console.WriteLine($"Order Filled: {context.Message.OrderId}");
            context.Saga.FilledAt = context.Message.FilledAt;
        });
    }
    
    public static EventActivityBinder<OrderSagaState, OrderCancelled> SetCancellationDetails(
        this EventActivityBinder<OrderSagaState, OrderCancelled> binder)
    {
        return binder.Then(context =>
        {
            Console.WriteLine($"Order Cancelled: {context.Message.OrderId}");
            context.Saga.CancelledAt = context.Message.CancelledAt;
        });
    }
    
    public static EventActivityBinder<OrderSagaState, OrderExpired> SetExpirationDetails(
        this EventActivityBinder<OrderSagaState, OrderExpired> binder)
    {
        return binder.Then(context =>
        {
            Console.WriteLine($"Order Expired: {context.Message.OrderId}");
            context.Saga.ExpiredAt = context.Message.ExpiredAt;
        });
    }
    
    public static EventActivityBinder<OrderSagaState, OrderFailed> SetFailureDetails(
        this EventActivityBinder<OrderSagaState, OrderFailed> binder)
    {
        return binder.Then(context =>
        {
            Console.WriteLine($"Order Failed: {context.Message.OrderId}");
            context.Saga.FailedAt = context.Message.FailedAt;
        });
    }

    public static EventActivityBinder<OrderSagaState, OrderSubmitted> SendNotification(
        this EventActivityBinder<OrderSagaState, OrderSubmitted> binder)
    {
        var response = binder.PublishAsync(context => context.Init<ProcessNotificationMessage>(new ProcessNotificationMessage
        {
            MessageId = Guid.NewGuid(),
            UserId = context.Message.UserId,
            Text = "Order submitted"
        }));

        return response;
    }
    
    public static EventActivityBinder<OrderSagaState, OrderPlaced> SendNotification(
        this EventActivityBinder<OrderSagaState, OrderPlaced> binder)
    {
        var response = binder.PublishAsync(context => context.Init<ProcessNotificationMessage>(new ProcessNotificationMessage
        {
            MessageId = Guid.NewGuid(),
            UserId = context.Message.UserId,
            Text = "Order placed"
        }));

        return response;
    }
    
    public static EventActivityBinder<OrderSagaState, OrderFilled> SendNotification(
        this EventActivityBinder<OrderSagaState, OrderFilled> binder)
    {
        return binder.PublishAsync(context => context.Init<ProcessNotificationMessage>(new ProcessNotificationMessage
        {
            MessageId = Guid.NewGuid(),
            UserId = context.Message.UserId,
            Text = "Order filled"
        }));
    }
    
    public static EventActivityBinder<OrderSagaState, OrderCancelled> SendNotification(
        this EventActivityBinder<OrderSagaState, OrderCancelled> binder)
    {
        return binder.PublishAsync(context => context.Init<ProcessNotificationMessage>(new ProcessNotificationMessage
        {
            MessageId = Guid.NewGuid(),
            UserId = context.Message.UserId,
            Text = "Order cancelled"
        }));
    }
    
    public static EventActivityBinder<OrderSagaState, OrderExpired> SendNotification(
        this EventActivityBinder<OrderSagaState, OrderExpired> binder)
    {
        return binder.PublishAsync(context => context.Init<ProcessNotificationMessage>(new ProcessNotificationMessage
        {
            MessageId = Guid.NewGuid(),
            UserId = context.Message.UserId,
            Text = "Order expired"
        }));
    }
    
    public static EventActivityBinder<OrderSagaState, OrderFailed> SendNotification(
        this EventActivityBinder<OrderSagaState, OrderFailed> binder)
    {
        return binder.PublishAsync(context => context.Init<ProcessNotificationMessage>(new ProcessNotificationMessage
        {
            MessageId = Guid.NewGuid(),
            UserId = context.Message.UserId,
            Text = "Order failed"
        }));
    }
}
