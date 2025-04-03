namespace CasinoWallet.Configuration
{
    public class GameSettings
    {
        public decimal MinBet { get; set; }
        public decimal MaxBet { get; set; }
        public WinChanceSettings WinChances { get; set; } = new();
        public MultiplierSettings Multipliers { get; set; } = new();
    }

    public class WinChanceSettings
    {
        public int Lose { get; set; }
        public int WinLow { get; set; }
        public int WinHigh { get; set; }
    }

    public class MultiplierSettings
    {
        public double WinLowMin { get; set; }
        public double WinLowMax { get; set; }
        public double WinHighMin { get; set; }
        public double WinHighMax { get; set; }
    }
}