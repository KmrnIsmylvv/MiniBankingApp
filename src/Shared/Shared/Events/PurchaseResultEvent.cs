namespace Shared.Events;

public record PurchaseResultEvent(bool Success, string Message)
{
    public PurchaseResultEvent() : this(false, string.Empty) { }
}
