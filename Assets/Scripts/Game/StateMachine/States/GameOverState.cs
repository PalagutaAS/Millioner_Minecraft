using System;
using System.Linq;
using Cysharp.Threading.Tasks;

public class GameOverState : IGameState
{
    public GameState StateId => GameState.GameOver;

    private readonly GameUI _gameUI;
    private readonly GameConfig _config;
    private readonly GameStateData _data;
    private readonly GameStateMachine _machine;

    public GameOverState(
        GameUI gameUI,
        GameConfig config,
        GameStateData data,
        GameStateMachine machine)
    {
        _gameUI = gameUI;
        _config = config;
        _data = data;
        _machine = machine;
    }

    public async UniTask Enter()
    {
        int safe = GetSafeAmount();
        _data.ResetCurrent();
        _gameUI.HideQuestionPanel();
        _gameUI.SetResultText(false, safe);

        await UniTask.Delay(TimeSpan.FromSeconds(_config.EndGameDelay));

        await _machine.TransitionTo(GameState.Idle);
    }

    public UniTask Exit() => UniTask.CompletedTask;

    private int GetSafeAmount()
    {
        //NOTE: readability over memory — LINQ для поиска максимального индекса
        int bestIndex = _config.SafeAmountIndices
            .Where(i => (_data.CurrentQuestionNumber - 1) > i)
            .DefaultIfEmpty(-1)
            .Max();

        return bestIndex >= 0 ? _config.PrizeAmounts[bestIndex] : 0;
    }
}
