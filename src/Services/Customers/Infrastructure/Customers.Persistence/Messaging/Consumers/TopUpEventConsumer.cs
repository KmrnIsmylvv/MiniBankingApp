using Customers.Application.Repositories;
using Customers.Domain.Entities;
using Customers.Persistence.Repositories;
using MassTransit;
using Shared.Events;

namespace Customers.Persistence.Messaging.Consumers;

public class TopUpEventConsumer(ICustomerRepository customerRepository) : IConsumer<TopUpEvent>
{

    public async Task Consume(ConsumeContext<TopUpEvent> context)
    {
        TopUpEvent message = context.Message;
        Customer customer = await customerRepository.GetByIdAsync(message.CustomerId);

        if (customer is null)
        {
            //logging
            Console.WriteLine($"");
            return;
        }

        using (var transaction = customerRepository.BeginTransaction())
        {
            try
            {
                customer.Balance += message.Amount;

                customerRepository.Update(customer);

                await customerRepository.SaveAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
            }
        }
    }
}