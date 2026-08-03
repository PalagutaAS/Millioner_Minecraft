using System;
using Cysharp.Threading.Tasks;

public class GameWonState : IGameState
{
    public GameState StateId => GameState.GameWon;

    private readonly GameUI _gameUI;
    private readonly GameConfig _config;
    private readonly SaveService _saveService;
    private readonly GameStateData _data;
    private readonly GameStateMachine _machine;

    public GameWonState(
        GameUI gameUI,
        GameConfig config,
        SaveService saveService,
        GameStateData data,
        GameStateMachine machine)
    {
        _gameUI = gameUI;
        _config = config;
        _saveService = saveService;
        _data = data;
        _machine = machine;
    }

    public async UniTask Enter()
    {
        int prize = _config.PrizeAmounts[^1];
        _saveService.Data.wallet += prize;
        _saveService.Data.hasActiveGame = false;
        _saveService.SaveLeaderboard();
        _saveService.Save();
        _data.Reset();
        _data.ResetCurrent();
        _gameUI.HideQuestionPanel();
        _gameUI.SetResultText(true, prize);

        await UniTask.Delay(TimeSpan.FromSeconds(_config.EndGameDelay));

        await _machine.TransitionTo(GameState.Idle);
    }

    public UniTask Exit() => UniTask.CompletedTask;
}
