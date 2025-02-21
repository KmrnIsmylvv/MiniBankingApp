using Microsoft.EntityFrameworkCore;
using Transactions.Domain.Entities;

namespace Transactions.Persistence.Contexts;

public class TransactionDbContext(DbContextOptions<TransactionDbContext> context) : DbContext(context)
{
    public DbSet<Transaction> Transactions { get; set; }
}