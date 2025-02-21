using FluentValidation;
using Transactions.Application.DTOs;
using Transactions.Domain;

namespace Transactions.Application.Validators;

public class AddTransactionValidator : AbstractValidator<AddTransactionDTO>
{
    public AddTransactionValidator()
    {
        RuleFor(x => x.CustomerId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be positive.");
    }
}