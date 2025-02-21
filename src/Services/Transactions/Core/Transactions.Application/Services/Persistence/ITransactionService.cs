using Shared.Common;
using Transactions.Application.DTOs;

namespace Transactions.Application.Services.Persistence;

public interface ITransactionService
{
    Task<ResponseDTO> TopUpAsync(AddTransactionDTO transactionDto);
    Task<ResponseDTO> PurchaseAsync(AddTransactionDTO transactionDto);
    Task<ResponseDTO> RefundAsync(AddTransactionDTO transactionDto);
}