using AutoMapper;
using Kata.Wallet.Domain;
using Kata.Wallet.Dtos;
using Kata.Wallet.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;

namespace Kata.Wallet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;
    private readonly ITransactionService _transactionService;
    private readonly IMapper _mapper;
    public WalletController(IWalletService walletService, ITransactionService transactionService, IMapper mapper)
    {
        _walletService = walletService;
        _transactionService = transactionService;
        _mapper = mapper;
    }

    [HttpGet("GetWallets")]
    public async Task<ActionResult<List<WalletDto>>> GetAll([FromQuery] int? userId, [FromQuery] string? userDocument)
    {

        List<Domain.Wallet> allWallets;

        if (userId.HasValue)
        {
            var wallet = await _walletService.GetById(userId.Value);
            if (wallet == null)
            {
                return NotFound("Wallet not found.");
            }

            allWallets = new List<Domain.Wallet> { wallet };
        }
        else
        {
            allWallets = await _walletService.GetAll();
        }

        if (!string.IsNullOrEmpty(userDocument))
        {
            allWallets = allWallets.Where(x => x.UserDocument.Equals(userDocument)).ToList();
        }
        if (!allWallets.Any())
        {
            return NoContent();
        }

        return Ok(_mapper.Map<List<WalletDto>>(allWallets));
    }

    [HttpPost("CreateWallet")]
    public async Task<ActionResult> Create([FromBody] WalletDto wallet)
    {

        await _walletService.CreateWallet(_mapper.Map<Domain.Wallet>(wallet));
        return Ok(wallet);

    }
    [HttpPost("CreateTransaction")]
    public async Task<ActionResult> CreateTransaction([FromBody]TransactionCreateDto newTransaction)
    {
        Domain.Wallet walletIn = await _walletService.GetById(newTransaction.WalletIncoming);
        Domain.Wallet walletOut = await _walletService.GetById(newTransaction.WalletOutgoing);


        if (walletIn == null)
        {
            return NotFound("Wallet Incoming not found.");
        }
        if (walletOut == null)
        {
            return NotFound("Wallet Out not found.");
        }

        if (walletOut.Balance < newTransaction.Amount)
        {
            return BadRequest("Insufficient money in the your account.");
        }

        if (walletOut.Currency != walletIn.Currency)
        {
            return BadRequest("Not matching currency");
        }
        walletIn.Balance = walletIn.Balance + newTransaction.Amount;
        walletOut.Balance = walletOut.Balance - newTransaction.Amount;

        var transaction = new Transaction
        {
            Amount = newTransaction.Amount,
            WalletIncoming = walletIn,
            WalletOutgoing = walletOut,
            Date = DateTime.UtcNow
        };
        await _transactionService.CreateTransaction(transaction);
        return Ok(_mapper.Map<TransactionDto>(transaction));

    }

}
