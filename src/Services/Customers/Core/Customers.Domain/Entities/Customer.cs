using Shared.Models;

namespace Customers.Domain.Entities;

public class Customer : BaseEntity<int>
{
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required DateTime Birthdate { get; set; }
    public required string PhoneNumber { get; set; }
    public double Balance { get; set; }
}