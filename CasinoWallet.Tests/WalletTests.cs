using Xunit;
using Moq;
using CasinoWallet.Interfaces;
using CasinoWallet.Services;
using CasinoWallet.Configuration;
using CasinoWallet.Domain;

namespace CasinoWallet.Tests
{
    public class WalletTests
    {
        private GameSettings GetDefaultSettings()
        {
            return new GameSettings
            {
                MinBet = 1,
                MaxBet = 10
            };
        }

        [Fact]
        public void Deposit_IncreasesBalance()
        {
            IWallet wallet = new Wallet(GetDefaultSettings());
            wallet.Deposit(50);
            Assert.Equal(50, wallet.Balance);
        }

        [Fact]
        public void Withdraw_DecreasesBalance_WhenEnoughFunds()
        {
            IWallet wallet = new Wallet(GetDefaultSettings());
            wallet.Deposit(50);
            bool result = wallet.Withdraw(20);
            Assert.True(result);
            Assert.Equal(30, wallet.Balance);
        }

        [Fact]
        public void Withdraw_Fails_WhenInsufficientFunds()
        {
            IWallet wallet = new Wallet(GetDefaultSettings());
            wallet.Deposit(10);
            bool result = wallet.Withdraw(20);
            Assert.False(result);
            Assert.Equal(10, wallet.Balance);
        }

        [Fact]
        public void ApplyGameResult_UpdatesBalanceCorrectly()
        {
            IWallet wallet = new Wallet(GetDefaultSettings());
            wallet.Deposit(100);
            wallet.ApplyGameResult(10, 15);
            Assert.Equal(105, wallet.Balance);
        }

        [Fact]
        public void CanBet_ValidatesBetLimits()
        {
            IWallet wallet = new Wallet(GetDefaultSettings());
            wallet.Deposit(10);
            Assert.True(wallet.CanBet(5));
            Assert.False(wallet.CanBet(11));
            Assert.False(wallet.CanBet(0));
        }
    }
}