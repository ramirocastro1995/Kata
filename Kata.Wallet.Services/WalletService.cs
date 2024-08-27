using AutoMapper;
using Kata.Wallet.Database;
using Kata.Wallet.Dtos;
using Kata.Wallet.Services.Interfaces;
namespace Kata.Wallet.Services
{
    public class WalletService:IWalletService
    {
        private readonly IWalletRepository _walletRepository;

        public WalletService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }

        public async Task<List<Domain.Wallet>> GetAll()
        {
            var allWallets = await _walletRepository.GetAllAsync();
            return allWallets;
        }

        public async Task<Domain.Wallet> GetById(int id)
        {
            var allWallets = await _walletRepository.GetByIdAsync(id);
            return allWallets;
        }

        public async Task<Domain.Wallet> CreateWallet(Domain.Wallet wallet) { 
            await _walletRepository.AddAsync(wallet);
            return wallet;
        }
    }
}
