using AutoMapper;
using Customers.Application.DTOs;
using Customers.Application.Repositories;
using Customers.Application.Services.Persistence;
using Customers.Domain.Entities;
using Shared.Common;

namespace Customers.Persistence.Services;

public class CustomerService(ICustomerRepository customerRepository,
                             IMapper mapper) : ICustomerService
{
    public async Task<ResponseDTO> AddAsync(AddCustomerDTO customerDto)
    {
        await customerRepository.AddAsync(mapper.Map<Customer>(customerDto));

        await customerRepository.SaveAsync();

        return ResponseDTO.CreateResponse(message: "You registered successfully.");
    }

    public async Task<ResponseDTO> GetBalanceById(int id)
    {
        Customer customer = await customerRepository.GetByIdAsync(id);

        if (customer is null)
            return ResponseDTO.CreateResponse(message: "Customer not found.", success: false);

        double balance = customer.Balance;

        return ResponseDTO.CreateResponse(data: new { balance }, message: "Ok");
    }
}