using Customers.Application.Repositories;
using Customers.Domain.Entities;
using MassTransit;
using Shared.Events;

namespace Customers.Persistence.Messaging.Consumers;

public class PurchaseEventConsumer(ICustomerRepository customerRepository) : IConsumer<PurchaseEvent>
{
    public async Task Consume(ConsumeContext<PurchaseEvent> context)
    {
        PurchaseEvent message = context.Message;
        Customer customer = await customerRepository.GetByIdAsync(message.CustomerId);

        if (customer is null)
        {
            await context.RespondAsync(new PurchaseResultEvent(false, "Customer not found."));
            return;
        }

        if (customer!.Balance < message.Amount)
        {
            await context.RespondAsync(new PurchaseResultEvent(false, "Insufficient balance."));
            return;
        }

        using (var transaction = customerRepository.BeginTransaction())
        {
            try
            {
                customer.Balance -= message.Amount;

                customerRepository.Update(customer);
                await customerRepository.SaveAsync();

                await transaction.CommitAsync();
                await context.RespondAsync(new PurchaseResultEvent(true, "Purchase successful."));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                await context.RespondAsync(new PurchaseResultEvent(false, "Transaction failed"));
            }
        }
    }
}