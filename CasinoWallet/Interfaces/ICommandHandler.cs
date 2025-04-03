namespace CasinoWallet.Interfaces
{
    public interface ICommandHandler
    {
        bool Handle(string input);
    }
}