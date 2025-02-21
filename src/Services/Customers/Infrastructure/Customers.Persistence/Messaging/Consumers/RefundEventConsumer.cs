using Customers.Application.Repositories;
using Customers.Domain.Entities;
using MassTransit;
using Shared.Events;

namespace Customers.Persistence.Messaging.Consumers;

public class RefundEventConsumer(ICustomerRepository customerRepository) : IConsumer<RefundEvent>
{
    public async Task Consume(ConsumeContext<RefundEvent> context)
    {
        RefundEvent message = context.Message;
        Customer customer = await customerRepository.GetByIdAsync(message.CustomerId);

        customer.Balance += message.Amount;
        customerRepository.Update(customer);
        await customerRepository.SaveAsync();
    }
}