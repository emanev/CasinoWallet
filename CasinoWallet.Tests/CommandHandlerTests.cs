//using Xunit;
//using Moq;
//using System.Collections.Generic;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Configuration;
//using CasinoWallet.Interfaces;
//using CasinoWallet.Configuration;
//using CasinoWallet.Services;
//using CasinoWallet.CommandProcessing;
//using System;

//namespace CasinoWallet.Tests
//{
//    public class CommandHandlerTests
//    {
//        private CommandHandler CreateHandler(
//            out Mock<IWallet> walletMock,
//            out Mock<IGameService> gameServiceMock,
//            out Mock<ILogger<CommandHandler>> loggerMock,
//            out Mock<IConsoleService> consoleMock,
//            GameSettings? settings = null)
//        {
//            walletMock = new Mock<IWallet>();
//            gameServiceMock = new Mock<IGameService>();
//            loggerMock = new Mock<ILogger<CommandHandler>>();
//            consoleMock = new Mock<IConsoleService>();

//            settings ??= new GameSettings { MinBet = 1, MaxBet = 10 };

//            return new CommandHandler(walletMock.Object, gameServiceMock.Object, loggerMock.Object, settings, consoleMock.Object);
//        }

//        [Fact]
//        public void Handle_BetCommand_WinZero_ShowsNoLuckMessage()
//        {
//            var handler = CreateHandler(out var walletMock, out var gameServiceMock, out _, out var consoleMock);
//            walletMock.Setup(w => w.CanBet(5)).Returns(true);
//            gameServiceMock.Setup(s => s.ExecuteBet(5)).Returns(0);

//            var result = handler.Handle("bet 5");

//            Assert.True(result);
//            consoleMock.Verify(c => c.WriteLine("No luck this time! Your current balance is: $0"), Times.Once);
//        }

//        [Fact]
//        public void Handle_BetCommand_CanBetFalse_LogsError()
//        {
//            var handler = CreateHandler(out var walletMock, out _, out var loggerMock, out _);
//            walletMock.Setup(w => w.CanBet(5)).Returns(false);

//            var result = handler.Handle("bet 5");

//            Assert.True(result);
//            loggerMock.Verify(
//                x => x.Log(
//                    LogLevel.Error,
//                    It.IsAny<EventId>(),
//                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Invalid bet")),
//                    It.IsAny<Exception>(),
//                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
//                Times.Once);
//        }

//        [Fact]
//        public void Handle_DepositCommand_ValidAmount_UpdatesBalance()
//        {
//            var handler = CreateHandler(out var walletMock, out _, out _, out _);
//            var result = handler.Handle("deposit 50");
//            Assert.True(result);
//            walletMock.Verify(w => w.Deposit(50), Times.Once);
//        }

//        [Fact]
//        public void Handle_DepositCommand_InvalidAmount_LogsError()
//        {
//            var handler = CreateHandler(out _, out _, out var loggerMock, out _);
//            var result = handler.Handle("deposit abc");
//            Assert.True(result);
//            loggerMock.Verify(
//                x => x.Log(
//                    LogLevel.Error,
//                    It.IsAny<EventId>(),
//                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Invalid deposit amount")),
//                    It.IsAny<Exception>(),
//                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
//                Times.Once);
//        }

//        [Fact]
//        public void Handle_ExitCommand_ReturnsFalse()
//        {
//            var handler = CreateHandler(out _, out _, out _, out var consoleMock);
//            var result = handler.Handle("exit");
//            Assert.False(result);
//            consoleMock.Verify(c => c.WriteLine("Thank you for playing! Hope to see you again soon."), Times.Once);
//        }

//        [Fact]
//        public void Handle_UnknownCommand_LogsError()
//        {
//            var handler = CreateHandler(out _, out _, out var loggerMock, out _);
//            var result = handler.Handle("nonsense");
//            Assert.True(result);
//            loggerMock.Verify(
//                x => x.Log(
//                    LogLevel.Error,
//                    It.IsAny<EventId>(),
//                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Unknown command")),
//                    It.IsAny<Exception>(),
//                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
//                Times.Once);
//        }

//        [Fact]
//        public void Handle_BetCommand_ValidBet_Win_ShowsWinMessage()
//        {
//            var handler = CreateHandler(out var walletMock, out var gameMock, out _, out var consoleMock);
//            walletMock.Setup(w => w.CanBet(5)).Returns(true);
//            gameMock.Setup(g => g.Play(5)).Returns(10);
//            var result = handler.Handle("bet 5");
//            Assert.True(result);
//            consoleMock.Verify(c => c.WriteLine("Congrats - you won $10! Your current balance is: $0"), Times.Once);
//        }

//        [Fact]
//        public void Handle_BetCommand_InvalidAmount_LogsError()
//        {
//            var handler = CreateHandler(out _, out _, out var loggerMock, out _);
//            var result = handler.Handle("bet abc");
//            Assert.True(result);
//            loggerMock.Verify(
//                x => x.Log(
//                    LogLevel.Error,
//                    It.IsAny<EventId>(),
//                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Invalid bet")),
//                    It.IsAny<Exception>(),
//                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
//                Times.Once);
//        }

//        [Fact]
//        public void Handle_WithdrawCommand_ValidAmount_ShowsSuccess()
//        {
//            var handler = CreateHandler(out var walletMock, out _, out _, out var consoleMock);
//            walletMock.Setup(w => w.Withdraw(20)).Returns(true);
//            var result = handler.Handle("withdraw 20");
//            Assert.True(result);
//            consoleMock.Verify(c => c.WriteLine("Your withdrawal of $20 was successful. Your current balance is: $0"), Times.Once);
//        }

//        [Fact]
//        public void Handle_WithdrawCommand_InsufficientBalance_LogsError()
//        {
//            var handler = CreateHandler(out var walletMock, out _, out var loggerMock, out _);
//            walletMock.Setup(w => w.Withdraw(50)).Returns(false);
//            var result = handler.Handle("withdraw 50");
//            Assert.True(result);
//            loggerMock.Verify(
//                x => x.Log(
//                    LogLevel.Error,
//                    It.IsAny<EventId>(),
//                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Insufficient balance for withdrawal")),
//                    It.IsAny<Exception>(),
//                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
//                Times.Once);
//        }

//        [Fact]
//        public void Handle_WithdrawCommand_InvalidAmount_LogsError()
//        {
//            var handler = CreateHandler(out _, out _, out var loggerMock, out _);
//            var result = handler.Handle("withdraw -5");
//            Assert.True(result);
//            loggerMock.Verify(
//                x => x.Log(
//                    LogLevel.Error,
//                    It.IsAny<EventId>(),
//                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Invalid withdrawal amount")),
//                    It.IsAny<Exception>(),
//                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
//                Times.Once);
//        }

//        [Fact]
//        public void MissingGameSettings_ThrowsInvalidSettingsException()
//        {
//            var configBuilder = new ConfigurationBuilder();
//            var config = configBuilder.Build();
//            var ex = Assert.Throws<Exceptions.InvalidSettingsException>(() =>
//            {
//                var settings = config.GetSection("GameSettings").Get<GameSettings>();
//                GameSettingsValidator.Validate(settings);
//            });
//            Assert.Equal("Missing GameSettings section in configuration.", ex.Message);
//        }

//        [Fact]
//        public void MissingMinOrMaxBet_ThrowsInvalidSettingsException()
//        {
//            var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
//            {
//                { "GameSettings:MinBet", "0" },
//                { "GameSettings:MaxBet", "0" }
//            }).Build();

//            var ex = Assert.Throws<Exceptions.InvalidSettingsException>(() =>
//            {
//                var settings = config.GetSection("GameSettings").Get<GameSettings>();
//                GameSettingsValidator.Validate(settings);
//            });

//            Assert.Contains("MinBet", ex.Message);
//        }

//        [Fact]
//        public void MinBetGreaterThanMaxBet_ThrowsInvalidSettingsException()
//        {
//            var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
//            {
//                { "GameSettings:MinBet", "10" },
//                { "GameSettings:MaxBet", "5" }
//            }).Build();

//            var settings = config.GetSection("GameSettings").Get<GameSettings>();

//            var ex = Assert.Throws<Exceptions.InvalidSettingsException>(() =>
//            {
//                GameSettingsValidator.Validate(settings);
//            });

//            Assert.Contains("MinBet cannot be greater than MaxBet", ex.Message);
//        }
//    }
//}

using Xunit;
using Moq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using CasinoWallet.Interfaces;
using CasinoWallet.Configuration;
using CasinoWallet.Services;
using CasinoWallet.Exceptions;
using System;

namespace CasinoWallet.Tests
{
    public class CommandHandlerTests
    {
        private CommandHandler CreateHandler(
            out Mock<IWallet> walletMock,
            out Mock<IGameService> gameServiceMock,
            out Mock<ILogger<CommandHandler>> loggerMock,
            out Mock<IConsoleService> consoleMock,
            GameSettings? settings = null)
        {
            walletMock = new Mock<IWallet>();
            gameServiceMock = new Mock<IGameService>();
            loggerMock = new Mock<ILogger<CommandHandler>>();
            consoleMock = new Mock<IConsoleService>();

            settings ??= new GameSettings { MinBet = 1, MaxBet = 10 };

            return new CommandHandler(walletMock.Object, gameServiceMock.Object, loggerMock.Object, settings, consoleMock.Object);
        }

        [Fact]
        public void Handle_BetCommand_WinZero_ShowsNoLuckMessage()
        {
            var handler = CreateHandler(out var walletMock, out var gameServiceMock, out _, out var consoleMock);
            walletMock.Setup(w => w.CanBet(5)).Returns(true);
            gameServiceMock.Setup(s => s.ExecuteBet(5)).Returns(0);

            var result = handler.Handle("bet 5");

            Assert.True(result);
            consoleMock.Verify(c => c.WriteLine("No luck this time! Your current balance is: $0"), Times.Once);
        }

        [Fact]
        public void Handle_BetCommand_CanBetFalse_LogsError()
        {
            var handler = CreateHandler(out var walletMock, out _, out var loggerMock, out _);
            walletMock.Setup(w => w.CanBet(5)).Returns(false);

            var result = handler.Handle("bet 5");

            Assert.True(result);
            loggerMock.Verify(
                x => x.Log(LogLevel.Error, It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Invalid bet")),
                    It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void Handle_DepositCommand_ValidAmount_UpdatesBalance()
        {
            var handler = CreateHandler(out var walletMock, out _, out _, out _);
            var result = handler.Handle("deposit 50");
            Assert.True(result);
            walletMock.Verify(w => w.Deposit(50), Times.Once);
        }

        [Fact]
        public void Handle_DepositCommand_InvalidAmount_LogsError()
        {
            var handler = CreateHandler(out _, out _, out var loggerMock, out _);
            var result = handler.Handle("deposit abc");
            Assert.True(result);
            loggerMock.Verify(
                x => x.Log(LogLevel.Error, It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Invalid deposit amount")),
                    It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void Handle_ExitCommand_ReturnsFalse()
        {
            var handler = CreateHandler(out _, out _, out _, out var consoleMock);
            var result = handler.Handle("exit");
            Assert.False(result);
            consoleMock.Verify(c => c.WriteLine("Thank you for playing! Hope to see you again soon."), Times.Once);
        }

        [Fact]
        public void Handle_UnknownCommand_LogsError()
        {
            var handler = CreateHandler(out _, out _, out var loggerMock, out _);
            var result = handler.Handle("nonsense");
            Assert.True(result);
            loggerMock.Verify(
                x => x.Log(LogLevel.Error, It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Unknown command")),
                    It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void Handle_BetCommand_ValidBet_Win_ShowsWinMessage()
        {
            var handler = CreateHandler(out var walletMock, out var gameServiceMock, out _, out var consoleMock);
            walletMock.Setup(w => w.CanBet(5)).Returns(true);
            gameServiceMock.Setup(g => g.ExecuteBet(5)).Returns(10);

            var result = handler.Handle("bet 5");
            Assert.True(result);

            consoleMock.Verify(c => c.WriteLine("Congrats - you won $10! Your current balance is: $0"), Times.Once);
        }

        [Fact]
        public void Handle_BetCommand_InvalidAmount_LogsError()
        {
            var handler = CreateHandler(out _, out _, out var loggerMock, out _);
            var result = handler.Handle("bet abc");
            Assert.True(result);
            loggerMock.Verify(
                x => x.Log(LogLevel.Error, It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Invalid bet")),
                    It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void Handle_WithdrawCommand_ValidAmount_ShowsSuccess()
        {
            var handler = CreateHandler(out var walletMock, out _, out _, out var consoleMock);
            walletMock.Setup(w => w.Withdraw(20)).Returns(true);

            var result = handler.Handle("withdraw 20");
            Assert.True(result);

            consoleMock.Verify(c => c.WriteLine("Your withdrawal of $20 was successful. Your current balance is: $0"), Times.Once);
        }

        [Fact]
        public void Handle_WithdrawCommand_InsufficientBalance_LogsError()
        {
            var handler = CreateHandler(out var walletMock, out _, out var loggerMock, out _);
            walletMock.Setup(w => w.Withdraw(50)).Returns(false);

            var result = handler.Handle("withdraw 50");
            Assert.True(result);

            loggerMock.Verify(
                x => x.Log(LogLevel.Error, It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Insufficient balance for withdrawal")),
                    It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void Handle_WithdrawCommand_InvalidAmount_LogsError()
        {
            var handler = CreateHandler(out _, out _, out var loggerMock, out _);
            var result = handler.Handle("withdraw -5");
            Assert.True(result);
            loggerMock.Verify(
                x => x.Log(LogLevel.Error, It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Invalid withdrawal amount")),
                    It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void MissingGameSettings_ThrowsInvalidSettingsException()
        {
            var configBuilder = new ConfigurationBuilder();
            var config = configBuilder.Build();

            var ex = Assert.Throws<InvalidSettingsException>(() =>
            {
                var settings = config.GetSection("GameSettings").Get<GameSettings>();
                GameSettingsValidator.Validate(settings);
            });

            Assert.Equal("Missing GameSettings section in configuration.", ex.Message);
        }

        [Fact]
        public void MissingMinOrMaxBet_ThrowsInvalidSettingsException()
        {
            var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "GameSettings:MinBet", "0" },
                { "GameSettings:MaxBet", "0" }
            }).Build();

            var ex = Assert.Throws<InvalidSettingsException>(() =>
            {
                var settings = config.GetSection("GameSettings").Get<GameSettings>();
                GameSettingsValidator.Validate(settings);
            });

            Assert.Contains("MinBet", ex.Message);
        }

        [Fact]
        public void MinBetGreaterThanMaxBet_ThrowsInvalidSettingsException()
        {
            var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "GameSettings:MinBet", "10" },
                { "GameSettings:MaxBet", "5" }
            }).Build();

            var settings = config.GetSection("GameSettings").Get<GameSettings>();

            var ex = Assert.Throws<InvalidSettingsException>(() =>
            {
                GameSettingsValidator.Validate(settings);
            });

            Assert.Contains("MinBet cannot be greater than MaxBet", ex.Message);
        }
    }
}
