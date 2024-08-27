using Kata.Wallet.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kata.Wallet.Services.Interfaces
{
    public interface ITransactionService
    {
        public Task<Domain.Transaction> CreateTransaction(Domain.Transaction transaction);
        public Task<List<Domain.Transaction>> GetTransactionByWallet(Domain.Wallet wallet);
    }
}
