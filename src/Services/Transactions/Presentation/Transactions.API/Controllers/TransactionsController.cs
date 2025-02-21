using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Transactions.Application.DTOs;
using Transactions.Application.Services.Persistence;

namespace Transactions.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController(ITransactionService transactionService) : ControllerBase
    {
        [HttpPost("topup")]
        public async Task<IActionResult> TopUp([FromBody] AddTransactionDTO transactionDto)
        {
            ResponseDTO result = await transactionService.TopUpAsync(transactionDto);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> Purchase([FromBody] AddTransactionDTO transactionDto)
        {
            ResponseDTO result = await transactionService.PurchaseAsync(transactionDto);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("refund")]
        public async Task<IActionResult> Refund([FromBody] AddTransactionDTO transactionDto)
        {
            ResponseDTO result = await transactionService.RefundAsync(transactionDto);

            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
