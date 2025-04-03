using System;
using CasinoWallet.Configuration;
using CasinoWallet.Interfaces;

namespace CasinoWallet.Domain
{
    //public class Game : IGame
    //{
    //    private readonly Random _random = new();

    //    public decimal Play(decimal betAmount)
    //    {
    //        int chance = _random.Next(1, 101);

    //        if (chance <= 50)
    //        {
    //            return 0;
    //        }
    //        else if (chance <= 90)
    //        {                
    //            int randInt = _random.Next(100, 201); // [100, 200]
    //            decimal multiplier = randInt / 100.0m;
    //            return Math.Round(betAmount * multiplier, 2);
    //        }
    //        else
    //        {                
    //            int randInt = _random.Next(201, 1001); // [201, 1000]
    //            decimal multiplier = randInt / 100.0m;
    //            return Math.Round(betAmount * multiplier, 2);
    //        }
    //    }
    //}

    public class Game : IGame
    {
        private readonly Random _random = new();
        private readonly GameSettings _settings;

        public Game(GameSettings settings)
        {
            _settings = settings;
        }

        public decimal Play(decimal betAmount)
        {
            int chance = _random.Next(1, 101);

            if (chance <= _settings.WinChances.Lose)
            {
                return 0;
            }
            else if (chance <= _settings.WinChances.Lose + _settings.WinChances.WinLow)
            {
                int randInt = _random.Next(
                    (int)(_settings.Multipliers.WinLowMin * 100),
                    (int)(_settings.Multipliers.WinLowMax * 100) + 1
                );
                decimal multiplier = randInt / 100.0m;
                return Math.Round(betAmount * multiplier, 2);
            }
            else
            {
                int randInt = _random.Next(
                    (int)(_settings.Multipliers.WinHighMin * 100) + 1,
                    (int)(_settings.Multipliers.WinHighMax * 100) + 1
                );
                decimal multiplier = randInt / 100.0m;
                return Math.Round(betAmount * multiplier, 2);
            }
        }
    }
}