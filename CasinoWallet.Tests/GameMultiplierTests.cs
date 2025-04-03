using Xunit;
using CasinoWallet.Configuration;
using CasinoWallet.Domain;

namespace CasinoWallet.Tests
{
    public class GameMultiplierTests
    {
        private Game CreateGame(int lose, int winLow, int winHigh)
        {
            return new Game(new GameSettings
            {
                MinBet = 1,
                MaxBet = 10,
                WinChances = new WinChanceSettings
                {
                    Lose = lose,
                    WinLow = winLow,
                    WinHigh = winHigh
                },
                Multipliers = new MultiplierSettings
                {
                    WinLowMin = 1.0,
                    WinLowMax = 2.0,
                    WinHighMin = 2.01,
                    WinHighMax = 10.0
                }
            });
        }

        [Fact]
        public void Play_WithLoseChance_ReturnsZero()
        {
            var game = CreateGame(100, 0, 0); // 100% lose
            for (int i = 0; i < 50; i++)
            {
                var result = game.Play(10);
                Assert.Equal(0, result);
            }
        }

        [Fact]
        public void Play_WithWinLowOnly_AlwaysReturnsBetween1xAnd2x()
        {
            var game = CreateGame(0, 100, 0); // 100% WinLow
            for (int i = 0; i < 50; i++)
            {
                var result = game.Play(10);
                var multiplier = result / 10;
                Assert.InRange(multiplier, 1.00m, 2.00m);
            }
        }

        [Fact]
        public void Play_WithWinHighOnly_AlwaysReturnsBetween2xAnd10x()
        {
            var game = CreateGame(0, 0, 100); // 100% WinHigh
            for (int i = 0; i < 50; i++)
            {
                var result = game.Play(10);
                var multiplier = result / 10;
                Assert.InRange(multiplier, 2.01m, 10.00m);
            }
        }

        [Fact]
        public void WinLow_Multiplier_Should_Be_LessThanOrEqual_2()
        {
            var game = CreateGame(0, 100, 0);

            for (int i = 0; i < 100; i++)
            {
                var result = game.Play(10);
                var multiplier = result / 10;

                Assert.InRange(multiplier, 1.00m, 2.00m);
            }
        }

        [Fact]
        public void WinHigh_Multiplier_Should_Be_GreaterThan_2()
        {
            var game = CreateGame(0, 0, 100);

            for (int i = 0; i < 100; i++)
            {
                var result = game.Play(10);
                var multiplier = result / 10;

                Assert.True(multiplier > 2.00m && multiplier <= 10.00m, $"Invalid multiplier: {multiplier}");
            }
        }
    }
}