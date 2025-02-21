namespace Shared.Events;

public record RefundEvent(int CustomerId, double Amount, int TransactionId);