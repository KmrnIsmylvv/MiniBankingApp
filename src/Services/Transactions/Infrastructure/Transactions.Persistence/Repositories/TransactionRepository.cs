using Transactions.Application.Repositories;
using Transactions.Domain.Entities;
using Transactions.Persistence.Contexts;

namespace Transactions.Persistence.Repositories;

public class TransactionRepository(TransactionDbContext context) : Repository<Transaction>(context), ITransactionRepository
{
}