using Customers.Application.DTOs;
using Customers.Application.Services.Persistence;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;

namespace Customers.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController(ICustomerService customerService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AddCustomerDTO customerDto)
        {
            ResponseDTO result = await customerService.AddAsync(customerDto);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetBalance(int id)
        {
            ResponseDTO result = await customerService.GetBalanceById(id);

            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
