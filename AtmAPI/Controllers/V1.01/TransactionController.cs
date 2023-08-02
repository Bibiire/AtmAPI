using Application.DTOs.ApplicationUser;
using Application.DTOs;
using Application.Services.Implementations.Auth;
using Application.Services.Interfaces;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.DTOs.Transaction;

namespace AtmAPI.Controllers.V1._01
{
    //[Authorize]
    [ApiController]
    [Route("v{version:apiVersion}/api/[controller]")]
    [ApiVersion("1.01")]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly ITransactionService _transactionService;
        public TransactionController(ILogger<TransactionController> logger, ITransactionService transactionService)
        {
            _logger = logger;
            _transactionService = transactionService;
        }

        /// <summary>
        /// Register User
        /// </summary>
        /// <param name="registerUser"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPost("[action]")]
        [ProducesResponseType(400, Type = typeof(BaseResponse))]
        [ProducesResponseType(200, Type = typeof(BaseResponse))]
        public async Task<IActionResult> Deposit(DepositDTO request)
        {
            if (!ModelState.IsValid) return BadRequest(ResponseHelper.BuildResponse("30", ModelState));
            var response = await _transactionService.Deposit(request);
            if (response.Code == "00") return Ok(response);
            return BadRequest(response);

        }

        /// <summary>
        /// Register User
        /// </summary>
        /// <param name="registerUser"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPost("[action]")]
        [ProducesResponseType(400, Type = typeof(BaseResponse))]
        [ProducesResponseType(200, Type = typeof(BaseResponse))]
        public async Task<IActionResult> Withdraw(DepositDTO request)
        {
            if (!ModelState.IsValid) return BadRequest(ResponseHelper.BuildResponse("30", ModelState));
            var response = await _transactionService.Withdraw(request);
            if (response.Code == "00") return Ok(response);
            return BadRequest(response);

        }

    }
}
