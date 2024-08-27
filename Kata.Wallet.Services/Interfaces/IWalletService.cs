using Kata.Wallet.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kata.Wallet.Services.Interfaces
{
    public interface IWalletService
    {
        public Task<List<Domain.Wallet>> GetAll();
        public Task<Domain.Wallet> CreateWallet(Domain.Wallet wallet);
        public Task<Domain.Wallet> GetById(int id);
    }
}
