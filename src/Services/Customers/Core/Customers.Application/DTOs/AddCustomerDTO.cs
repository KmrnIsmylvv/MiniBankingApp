namespace Customers.Application.DTOs;

public record AddCustomerDTO(string Name,
                             string Surname,
                             DateTime BirthDate,
                             string PhoneNumber);