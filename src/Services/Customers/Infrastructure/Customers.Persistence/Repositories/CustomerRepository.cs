using Customers.Application.Repositories;
using Customers.Domain.Entities;
using Customers.Persistence.Contexts;

namespace Customers.Persistence.Repositories;

public class CustomerRepository(CustomerDbContext context) : Repository<Customer>(context), ICustomerRepository
{
}