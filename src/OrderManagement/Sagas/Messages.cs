namespace OrderManagement.Sagas;

// Ordering
public class OrderSubmitted
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public string CurrencyId { get; init; } = null!;
    public decimal Price { get; init; }
    public OrderType OrderType { get; init; }
    public decimal Quantity { get; init; }
    public DateTime SubmittedAt { get; init; }
}

public class OrderFilled
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public string CurrencyId { get; init; } = null!;
    public decimal Price { get; init; }
    public OrderType OrderType { get; init; }
    public decimal Quantity { get; init; }
    public DateTime FilledAt { get; init; }
}

public class OrderExpired
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public string CurrencyId { get; init; } = null!;
    public decimal Price { get; init; }
    public OrderType OrderType { get; init; }
    public decimal Quantity { get; init; }
    public DateTime ExpiredAt { get; init; }
}

public class OrderPlaced
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public string CurrencyId { get; init; } = null!;
    public decimal Price { get; init; }
    public OrderType OrderType { get; init; }
    public decimal Quantity { get; init; }
    public DateTime PlacedAt { get; init; }

}

public class OrderCancelled
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public string CurrencyId { get; init; } = null!;
    public decimal Price { get; init; }
    public OrderType OrderType { get; init; }
    public decimal Quantity { get; init; }
    public DateTime CancelledAt { get; init; }
}

public class OrderFailed
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public string Reason { get; init; } = string.Empty;
    public DateTime FailedAt { get; init; }
}

public enum OrderType
{
    Buy,
    Sell
}

// Notifications
public class ProcessNotificationMessage
{
    public Guid MessageId { get; init; }
    public Guid UserId { get; init; }
    public DeliveryType DeliveryType { get; init; }
    public string Text { get; init; } = null!;
    public DateTime? ScheduledFor { get; init; }

}

public enum DeliveryType
{
    Direct,
    Daily
}
