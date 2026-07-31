using System;
using Cysharp.Threading.Tasks;

public class GameWonState : IGameState
{
    public GameState StateId => GameState.GameWon;

    private readonly GameUI _gameUI;
    private readonly GameConfig _config;
    private readonly GameStateMachine _machine;

    public GameWonState(
        GameUI gameUI,
        GameConfig config,
        GameStateMachine machine)
    {
        _gameUI = gameUI;
        _config = config;
        _machine = machine;
    }

    public async UniTask Enter()
    {
        int prize = _config.PrizeAmounts[^1];

        _gameUI.HideQuestionPanel();
        _gameUI.SetResultText(true, prize);

        await UniTask.Delay(TimeSpan.FromSeconds(_config.EndGameDelay));

        await _machine.TransitionTo(GameState.Idle);
    }

    public UniTask Exit() => UniTask.CompletedTask;
}
