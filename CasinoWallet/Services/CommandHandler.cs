using System;
using Microsoft.Extensions.Logging;
using CasinoWallet.Interfaces;
using CasinoWallet.Configuration;
using CasinoWallet.Exceptions;

namespace CasinoWallet.Services
{
    public class CommandHandler : ICommandHandler
    {
        private readonly IWallet _wallet;
        private readonly IGameService _gameService;
        private readonly ILogger<CommandHandler> _logger;
        private readonly GameSettings _settings;
        private readonly IConsoleService _console;

        public CommandHandler(IWallet wallet, IGameService gameService, 
            ILogger<CommandHandler> logger, GameSettings settings, IConsoleService console)
        {
            _wallet = wallet;
            _gameService = gameService;
            _logger = logger;
            _settings = settings;
            _console = console;
        }

        public bool Handle(string input)
        {
            try
            {
                switch (input)
                {
                    case string s when s.StartsWith("deposit "):
                        HandleDeposit(s);
                        break;
                    case string s when s.StartsWith("bet "):
                        HandleBet(s);
                        break;
                    case string s when s.StartsWith("withdraw "):
                        HandleWithdraw(s);
                        break;
                    case "exit":
                        _console.WriteLine("Thank you for playing! Hope to see you again soon.");
                        _console.WriteLine("");
                        _console.WriteLine("Press any key to exit.");
                        return false;
                    default:
                        throw new InvalidCommandException("Unknown command.");
                }
            }
            catch (InvalidCommandException ex)
            {
                _logger.LogError(ex.Message);
            }
            catch (InvalidBetException ex)
            {
                _logger.LogError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
            }

            _console.WriteLine();
            return true;
        }

        private void HandleDeposit(string input)
        {
            var parts = input.Split(" ");
            if (parts.Length == 2 && decimal.TryParse(parts[1], out var amount) && amount > 0)
            {
                _wallet.Deposit(amount);                
                _console.WriteLine($"Your deposit of {FormatAmount(amount)} was successful. Your current balance is: {FormatAmount(_wallet.Balance)}");
            }
            else
            {
                throw new InvalidCommandException("Invalid deposit amount.");
            }
        }

        private void HandleBet(string input)
        {
            var parts = input.Split(" ");
            if (parts.Length != 2 || !decimal.TryParse(parts[1], out var bet))
            {
                throw new InvalidBetException("Invalid bet. Bets must be a valid number.");
            }

            if (!_wallet.CanBet(bet))
            {
                throw new InvalidBetException("Invalid bet. Not enough balance or out of range.");
            }

            var win = _gameService.ExecuteBet(bet);
            _wallet.ApplyGameResult(bet, win);

            if (win > 0)
            {
                _console.WriteLine($"Congrats - you won {FormatAmount(win)}! Your current balance is: {FormatAmount(_wallet.Balance)}");
            }
            else
            {
                _console.WriteLine($"No luck this time! Your current balance is: {FormatAmount(_wallet.Balance)}");
            }
        }

        private void HandleWithdraw(string input)
        {
            var parts = input.Split(" ");
            if (parts.Length != 2 || !decimal.TryParse(parts[1], out var amount) || amount <= 0)
            {
                throw new InvalidCommandException("Invalid withdrawal amount.");
            }

            if (_wallet.Withdraw(amount))
            {
                _console.WriteLine($"Your withdrawal of {FormatAmount(amount)} was successful. Your current balance is: {FormatAmount(_wallet.Balance)}");
            }
            else
            {
                throw new InvalidCommandException("Insufficient balance for withdrawal.");
            }
        }

        private static string FormatAmount(decimal amount)
        {
            return amount % 1 == 0 ? $"${(int)amount}" : $"${amount:F2}";
        }
    }
}