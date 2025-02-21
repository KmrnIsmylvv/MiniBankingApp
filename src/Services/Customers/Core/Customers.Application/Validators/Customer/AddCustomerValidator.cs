using Customers.Application.DTOs;
using FluentValidation;

namespace Customers.Application.Validators.Customer;

public class AddCustomerValidator : AbstractValidator<AddCustomerDTO>
{
    public AddCustomerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(2, 50).WithMessage("Name must be between 2 and 50 characters.");

        RuleFor(x => x.Surname)
           .NotEmpty().WithMessage("Surname is required.")
           .Length(2, 50).WithMessage("Surname must be between 2 and 50 characters.");

        RuleFor(x => x.PhoneNumber)
           .Matches(@"^\+994[0-9]{9}$")
           .WithMessage("Invalid phone number format. Please use a valid number");

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("BirthDate is required.")
            .Must(BeAtLeast18YearsOld).WithMessage("Customer must be at least 18 years old.");
    }
    private bool BeAtLeast18YearsOld(DateTime birthDate)
    {
        int age = DateTime.Today.Year - birthDate.Year;

        if (birthDate > DateTime.Today.AddYears(-age))
            age--;

        return age >= 18;
    }
}