namespace Shared.Events;

public record TopUpEvent(int CustomerId, double Amount);