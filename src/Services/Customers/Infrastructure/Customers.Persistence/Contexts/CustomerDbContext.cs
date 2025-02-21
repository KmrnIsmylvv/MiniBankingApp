using Customers.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Customers.Persistence.Contexts;

public class CustomerDbContext(DbContextOptions<CustomerDbContext> context) : DbContext(context)
{
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>()
                    .Property(c => c.Balance)
                    .HasDefaultValue(50);
    }
}