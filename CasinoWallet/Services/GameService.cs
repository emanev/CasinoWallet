using CasinoWallet.Interfaces;

public class GameService : IGameService
{
    private readonly IGame _game;
    private readonly IWallet _wallet;

    public GameService(IGame game, IWallet wallet)
    {
        _game = game;
        _wallet = wallet;
    }

    public decimal ExecuteBet(decimal amount)
    {
        var win = _game.Play(amount);
        _wallet.ApplyGameResult(amount, win);
        return win;
    }
}
