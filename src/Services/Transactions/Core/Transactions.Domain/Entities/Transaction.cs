using Shared.Models;

namespace Transactions.Domain.Entities;

public class Transaction : BaseEntity<int>
{
    public int CustomerId { get; set; }
    public double Amount { get; set; }
    public TransactionTypes Type { get; set; }
    public DateTime Date { get; set; }
}