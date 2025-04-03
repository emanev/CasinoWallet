using System;
using CasinoWallet.Interfaces;

namespace CasinoWallet.Services
{
    public class ConsoleService : IConsoleService
    {
        public string? Read() => Console.ReadLine();
        public void Write(string message) => Console.Write(message);
        public void WriteLine(string message) => Console.WriteLine(message);
        public void WriteLine() => Console.WriteLine();
    }
}