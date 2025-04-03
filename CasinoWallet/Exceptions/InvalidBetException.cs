using System;

namespace CasinoWallet.Exceptions
{
    public class InvalidBetException : Exception
    {
        public InvalidBetException(string message) : base(message) { }
    }
}
