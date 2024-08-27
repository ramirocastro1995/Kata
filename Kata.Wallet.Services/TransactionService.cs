using AutoMapper;
using Kata.Wallet.Database;
using Kata.Wallet.Domain;
using Kata.Wallet.Dtos;
using Kata.Wallet.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kata.Wallet.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        public TransactionService(ITransactionRepository transactionRepository,IMapper mapper)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<Transaction> CreateTransaction(Transaction transaction)
        {
            await _transactionRepository.AddAsync(transaction);
            return transaction;
        }

        public async Task<List<Transaction>> GetTransactionByWallet(Domain.Wallet wallet)
        {
            var allTransactions = await _transactionRepository.GetAllAsync();

            var finalTransaction = allTransactions.Where(x =>
            (x.WalletIncoming != null && x.WalletIncoming.Id == wallet.Id) ||
            (x.WalletOutgoing != null && x.WalletOutgoing.Id == wallet.Id)).ToList();
            return finalTransaction;
        }
    }
}
