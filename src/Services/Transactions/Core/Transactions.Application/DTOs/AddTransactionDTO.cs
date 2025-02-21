namespace Transactions.Application.DTOs;

public record AddTransactionDTO(int CustomerId,
                                double Amount);