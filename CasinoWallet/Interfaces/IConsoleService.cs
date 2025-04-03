namespace CasinoWallet.Interfaces
{
    public interface IConsoleService
    {
        string? Read();
        void Write(string message);
        void WriteLine(string message);
        void WriteLine();
    }
}