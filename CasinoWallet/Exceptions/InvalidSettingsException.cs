using System;

namespace CasinoWallet.Exceptions
{
    public class InvalidSettingsException : Exception
    {
        public InvalidSettingsException(string message) : base(message) { }
    }
}
