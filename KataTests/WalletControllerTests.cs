using AutoMapper;
using Kata.Wallet.API.Controllers;
using Kata.Wallet.Domain;
using Kata.Wallet.Dtos;
using Kata.Wallet.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace KataTests
{
    public class WalletControllerTests
    {
        private readonly Mock<IWalletService> _walletServiceMock;
        private readonly Mock<ITransactionService> _transactionServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly WalletController _walletController;

        public WalletControllerTests()
        {
            _walletServiceMock = new Mock<IWalletService>();
            _transactionServiceMock = new Mock<ITransactionService>();
            _mapperMock = new Mock<IMapper>();

            _walletController = new WalletController(
                _walletServiceMock.Object,
                _transactionServiceMock.Object,
                _mapperMock.Object);
        }

        #region Create Wallet

        [Fact]
        public async Task CreateValidWallet()
        {
            var walletDto = new WalletDto { Id = 1, UserDocument = "12345" };
            var wallet = new Wallet { Id = 1, UserDocument = "12345" };

            _mapperMock.Setup(m => m.Map<Wallet>(walletDto)).Returns(wallet);
            _walletServiceMock.Setup(s => s.CreateWallet(wallet)).ReturnsAsync(wallet);

            var result = await _walletController.Create(walletDto);


            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<WalletDto>(okResult.Value);
            Assert.Equal(walletDto.Id, returnValue.Id);
            Assert.Equal(walletDto.UserDocument, returnValue.UserDocument);

            _walletServiceMock.Verify(s => s.CreateWallet(wallet), Times.Once);

        }
        #endregion

        #region Get Wallet tests
        [Fact]
        public async Task GetAllWithId()
        {
            var userId = 1;
            var wallet = new Wallet { Id = userId, UserDocument = "12345" };
            var walletDto = new WalletDto { Id = userId, UserDocument = "12345" };

            _walletServiceMock.Setup(x => x.GetById(userId)).ReturnsAsync(wallet);
            _mapperMock.Setup(x => x.Map<List<WalletDto>>(It.IsAny<List<Wallet>>()))
                       .Returns(new List<WalletDto> { walletDto });

            var result = await _walletController.GetAll(userId, null);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<WalletDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(walletDto.Id, returnValue.First().Id);

            _walletServiceMock.Verify(s => s.GetById(userId), Times.Once);
        }

        [Fact]
        public async Task GetAllReturnsAllWallets()
        {
            // Arrange
            var wallets = new List<Wallet>
            {
                new Wallet { Id = 1, UserDocument = "12345" },
                new Wallet { Id = 2, UserDocument = "67890" }
            };
            var walletDtos = new List<WalletDto>
            {
                new WalletDto { Id = 1, UserDocument = "12345" },
                new WalletDto { Id = 2, UserDocument = "67890" }
            };

            _walletServiceMock.Setup(s => s.GetAll()).ReturnsAsync(wallets);
            _mapperMock.Setup(m => m.Map<List<WalletDto>>(wallets)).Returns(walletDtos);

            var result = await _walletController.GetAll(null, null);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<WalletDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);

            _walletServiceMock.Verify(s => s.GetAll(), Times.Once);
        }

        [Fact]
        public async Task GetAllReturnsFilterWallets()
        {
            var userDocument = "12345";
            var wallets = new List<Wallet>
            {
                new Wallet { Id = 1, UserDocument = userDocument },
                new Wallet { Id = 2, UserDocument = "67890" }
            };
            var filteredWallets = wallets.Where(x => x.UserDocument == userDocument).ToList();
            var walletDtos = new List<WalletDto>
            {
                new WalletDto { Id = 1, UserDocument = userDocument }
            };

            _walletServiceMock.Setup(s => s.GetAll()).ReturnsAsync(wallets);
            _mapperMock.Setup(x => x.Map<List<WalletDto>>(filteredWallets)).Returns(walletDtos);

            // Act
            var result = await _walletController.GetAll(null, userDocument);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<WalletDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(userDocument, returnValue.First().UserDocument);

            _walletServiceMock.Verify(x => x.GetAll(), Times.Once);
        }

        [Fact]
        public async Task GetAllReturnsNoContent()
        {
            _walletServiceMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Wallet>());

            var result = await _walletController.GetAll(null, null);

            Assert.IsType<NoContentResult>(result.Result);

            _walletServiceMock.Verify(x => x.GetAll(), Times.Once);
        }
        #endregion

        #region Create Transaction Tests
        [Fact]
        public async Task CreateTransaction_WalletIncomingNotFound_ReturnsNotFound()
        {
            var newTransaction = new TransactionCreateDto { WalletIncoming = 1, WalletOutgoing = 2, Amount = 100 };
            _walletServiceMock.Setup(s => s.GetById(newTransaction.WalletIncoming)).ReturnsAsync((Wallet)null);

            var result = await _walletController.CreateTransaction(newTransaction);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Wallet Incoming not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task CreateTransaction_WalletOutgoingNotFound_ReturnsNotFound()
        {
            var newTransaction = new TransactionCreateDto { WalletIncoming = 1, WalletOutgoing = 2, Amount = 100 };
            var walletIn = new Wallet { Id = 1, Balance = 200, Currency = Currency.USD };

            _walletServiceMock.Setup(s => s.GetById(newTransaction.WalletIncoming)).ReturnsAsync(walletIn);
            _walletServiceMock.Setup(s => s.GetById(newTransaction.WalletOutgoing)).ReturnsAsync((Wallet)null);

            var result = await _walletController.CreateTransaction(newTransaction);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Wallet Out not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task CreateTransaction_InsufficientFunds_ReturnsBadRequest()
        {

            var newTransaction = new TransactionCreateDto { WalletIncoming = 1, WalletOutgoing = 2, Amount = 300 };
            var walletIn = new Wallet { Id = 1, Balance = 200, Currency = Currency.USD };
            var walletOut = new Wallet { Id = 2, Balance = 100, Currency = Currency.USD };

            _walletServiceMock.Setup(s => s.GetById(newTransaction.WalletIncoming)).ReturnsAsync(walletIn);
            _walletServiceMock.Setup(s => s.GetById(newTransaction.WalletOutgoing)).ReturnsAsync(walletOut);


            var result = await _walletController.CreateTransaction(newTransaction);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Insufficient money in your account.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateTransaction_CurrencyMismatch_ReturnsBadRequest()
        {
            var newTransaction = new TransactionCreateDto { WalletIncoming = 1, WalletOutgoing = 2, Amount = 100 };
            var walletIn = new Wallet { Id = 1, Balance = 200, Currency = Currency.USD };
            var walletOut = new Wallet { Id = 2, Balance = 200, Currency = Currency.EUR };

            _walletServiceMock.Setup(s => s.GetById(newTransaction.WalletIncoming)).ReturnsAsync(walletIn);
            _walletServiceMock.Setup(s => s.GetById(newTransaction.WalletOutgoing)).ReturnsAsync(walletOut);

            var result = await _walletController.CreateTransaction(newTransaction);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Not matching currency", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateTransaction_ValidTransaction_ReturnsOk()
        {
            var newTransaction = new TransactionCreateDto { WalletIncoming = 1, WalletOutgoing = 2, Amount = 100 };
            var walletIn = new Wallet { Id = 1, Balance = 200, Currency = Currency.USD };
            var walletOut = new Wallet { Id = 2, Balance = 200, Currency = Currency.USD };
            var transaction = new Transaction { Amount = 100, WalletIncoming = walletIn, WalletOutgoing = walletOut };

            _walletServiceMock.Setup(s => s.GetById(newTransaction.WalletIncoming)).ReturnsAsync(walletIn);
            _walletServiceMock.Setup(s => s.GetById(newTransaction.WalletOutgoing)).ReturnsAsync(walletOut);
            _transactionServiceMock.Setup(s => s.CreateTransaction(It.IsAny<Transaction>())).ReturnsAsync(transaction);
            _mapperMock.Setup(m => m.Map<TransactionDto>(transaction)).Returns(new TransactionDto { Amount = 100 });

            var result = await _walletController.CreateTransaction(newTransaction);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<TransactionDto>(okResult.Value);
            Assert.Equal(100, returnValue.Amount);

            _walletServiceMock.Verify(s => s.GetById(newTransaction.WalletIncoming), Times.Once);
            _walletServiceMock.Verify(s => s.GetById(newTransaction.WalletOutgoing), Times.Once);
            _transactionServiceMock.Verify(s => s.CreateTransaction(It.IsAny<Transaction>()), Times.Once);
            #endregion
        }
    }

}