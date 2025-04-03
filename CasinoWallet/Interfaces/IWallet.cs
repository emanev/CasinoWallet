namespace CasinoWallet.Interfaces
{
    public interface IWallet
    {
        decimal Balance { get; }
        void Deposit(decimal amount);
        bool Withdraw(decimal amount);
        bool CanBet(decimal amount);
        void ApplyGameResult(decimal betAmount, decimal winAmount);
    }
}