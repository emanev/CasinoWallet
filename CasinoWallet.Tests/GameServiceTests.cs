using Xunit;
using Moq;
using CasinoWallet.Interfaces;
using CasinoWallet.Services;

namespace CasinoWallet.Tests
{
    public class GameServiceTests
    {
        [Fact]
        public void ExecuteBet_WhenWinIsZero_CallsApplyGameResultWithZero()
        {
            var walletMock = new Mock<IWallet>();
            var gameMock = new Mock<IGame>();

            gameMock.Setup(g => g.Play(10)).Returns(0);

            var service = new GameService(gameMock.Object, walletMock.Object);
            var result = service.ExecuteBet(10);

            Assert.Equal(0, result);
            walletMock.Verify(w => w.ApplyGameResult(10, 0), Times.Once);
        }

        [Fact]
        public void ExecuteBet_WhenWinIsPositive_CorrectlyAppliesWin()
        {
            var walletMock = new Mock<IWallet>();
            var gameMock = new Mock<IGame>();

            gameMock.Setup(g => g.Play(10)).Returns(25);

            var service = new GameService(gameMock.Object, walletMock.Object);
            var result = service.ExecuteBet(10);

            Assert.Equal(25, result);
            walletMock.Verify(w => w.ApplyGameResult(10, 25), Times.Once);
        }

        [Fact]
        public void ExecuteBet_AlwaysCallsGamePlayAndApplyGameResult()
        {
            var walletMock = new Mock<IWallet>();
            var gameMock = new Mock<IGame>();

            gameMock.Setup(g => g.Play(It.IsAny<decimal>())).Returns(15);

            var service = new GameService(gameMock.Object, walletMock.Object);
            var result = service.ExecuteBet(7);

            Assert.Equal(15, result);
            gameMock.Verify(g => g.Play(7), Times.Once);
            walletMock.Verify(w => w.ApplyGameResult(7, 15), Times.Once);
        }
    }
}