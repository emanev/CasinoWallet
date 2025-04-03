using CasinoWallet.Configuration;
using CasinoWallet.Interfaces;
using System.Runtime;

namespace CasinoWallet.Domain
{
    public class Wallet(GameSettings settings) : IWallet
    {
        public decimal Balance { get; private set; }
        public readonly GameSettings _settings = settings;

        public void Deposit(decimal amount)
        {
            Balance += amount;
        }

        public bool Withdraw(decimal amount)
        {
            if (Balance >= amount)
            {
                Balance -= amount;
                return true;
            }

            return false;
        }
        
        public bool CanBet(decimal amount)
        {
            if (amount < _settings.MinBet || amount > _settings.MaxBet)
            {
                return false;
            }

            return amount > 0 && Balance >= amount;
        }

        public void ApplyGameResult(decimal betAmount, decimal winAmount)
        {
            Balance -= betAmount;
            Balance += winAmount;
        }
    }
}