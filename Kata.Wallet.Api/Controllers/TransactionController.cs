using AutoMapper;
using Kata.Wallet.Dtos;
using Kata.Wallet.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Kata.Wallet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IWalletService _walletService;
        private readonly IMapper _mapper;
        public TransactionController(ITransactionService transactionService,IWalletService walletService,IMapper mapper)
        {
            _transactionService = transactionService;
            _walletService = walletService;
            _mapper = mapper;
        }

        [HttpGet("GetTransactions")]
        public async Task<ActionResult<List<TransactionDto>>> GetTransactionByWallet(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var getWallet = await _walletService.GetById(id);
            var getTransactionById = await _transactionService.GetTransactionByWallet(getWallet);
            foreach(var transaction in getTransactionById)
            {
                if(transaction.WalletIncoming != null)
                {
                    transaction.Description = "Incoming Transaction";
                }
                else 
                {
                    transaction.Description = "Outgoing Transaction";
                }
            }
            return Ok(_mapper.Map<List<TransactionDto>>(getTransactionById));
        }
    }
}
