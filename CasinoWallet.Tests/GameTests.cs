using Xunit;
using CasinoWallet.Services;
using CasinoWallet.Configuration;
using CasinoWallet.Interfaces;
using CasinoWallet.Domain;

namespace CasinoWallet.Tests
{
    public class GameRangeTest
    {
        [Fact]
        public void Game_Play_ReturnsExpectedRange()
        {
            var settings = new GameSettings
            {
                MinBet = 1,
                MaxBet = 10,
                WinChances = new WinChanceSettings { Lose = 50, WinLow = 40, WinHigh = 10 },
                Multipliers = new MultiplierSettings
                {
                    WinLowMin = 1.0,
                    WinLowMax = 2.0,
                    WinHighMin = 2.01,
                    WinHighMax = 10.0
                }
            };

            IGame game = new Game(settings);
            decimal bet = 5;

            for (int i = 0; i < 100; i++)
            {
                decimal result = game.Play(bet);
                Assert.InRange(result, 0, 50);
            }
        }
    }
}