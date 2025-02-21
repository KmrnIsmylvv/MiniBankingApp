using AutoMapper;
using MassTransit;
using Shared.Common;
using Shared.Events;
using Transactions.Application.DTOs;
using Transactions.Application.Repositories;
using Transactions.Application.Services.Persistence;
using Transactions.Domain;
using Transactions.Domain.Entities;

namespace Transactions.Persistence.Services;

public class TransactionService(ITransactionRepository transactionRepository,
                                IPublishEndpoint publishEndpoint,
                                IRequestClient<PurchaseEvent> purchaseClient,
                                IMapper mapper) : ITransactionService
{
    public async Task<ResponseDTO> PurchaseAsync(AddTransactionDTO transactionDto)
    {
        Guid correlationId = Guid.NewGuid();

        Transaction transaction = mapper.Map<Transaction>(transactionDto);
        transaction.Type = TransactionTypes.Purchase;

        await transactionRepository.AddAsync(transaction);
        await transactionRepository.SaveAsync();

        Response<PurchaseResultEvent> response = await purchaseClient.GetResponse<PurchaseResultEvent>(
                new PurchaseEvent(transactionDto.CustomerId, transactionDto.Amount, correlationId)
            );

        return response.Message.Success
            ? ResponseDTO.CreateResponse(message: "Operation completed successfully.", success: true)
            : ResponseDTO.CreateResponse(message: response.Message.Message);
    }

    public async Task<ResponseDTO> RefundAsync(AddTransactionDTO transactionDto)
    {
        Transaction? lastPurchase = transactionRepository
             .GetWhere(t => t.CustomerId == transactionDto.CustomerId &&
                             t.Type == TransactionTypes.Purchase)
             .OrderByDescending(d => d.Date)
             .FirstOrDefault();

        if (lastPurchase is null)
        {
            return ResponseDTO.CreateResponse(
                message: "No previous purchase found for this customer.",
                success: false
            );
        }

        if (transactionDto.Amount > lastPurchase.Amount)
            return ResponseDTO.CreateResponse(success: false, message: "Invalid refund amount.");

        Transaction transaction = mapper.Map<Transaction>(transactionDto);
        transaction.Type = TransactionTypes.Refund;

        await transactionRepository.AddAsync(transaction);
        await transactionRepository.SaveAsync();

        await publishEndpoint.Publish(new RefundEvent(transactionDto.CustomerId, transactionDto.Amount, lastPurchase.Id));

        return ResponseDTO.CreateResponse(message: "Operation completed successfully.");
    }

    public async Task<ResponseDTO> TopUpAsync(AddTransactionDTO transactionDto)
    {
        Transaction transaction = mapper.Map<Transaction>(transactionDto);

        await transactionRepository.AddAsync(transaction);
        await transactionRepository.SaveAsync();

        await publishEndpoint.Publish(new TopUpEvent(transactionDto.CustomerId, transactionDto.Amount));

        return ResponseDTO.CreateResponse(message: "Operation completed successfully.");
    }
}