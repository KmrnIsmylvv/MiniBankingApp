namespace Shared.Events;

public record PurchaseEvent(int CustomerId, double Amount, Guid CorrelationId);