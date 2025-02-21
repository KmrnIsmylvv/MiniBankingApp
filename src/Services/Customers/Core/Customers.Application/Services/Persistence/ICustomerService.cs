using Customers.Application.DTOs;
using Shared.Common;

namespace Customers.Application.Services.Persistence;

public interface ICustomerService
{
    Task<ResponseDTO> AddAsync(AddCustomerDTO customerDto);
    Task<ResponseDTO> GetBalanceById(int id);
}