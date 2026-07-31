using System;
using System.Linq;
using Cysharp.Threading.Tasks;

public class AnswerLockedState : IGameState
{
    public GameState StateId => GameState.AnswerLocked;

    private readonly GameUI _gameUI;
    private readonly AudioManager _audioManager;
    private readonly GameConfig _config;
    private readonly GameStateData _data;
    private readonly SaveService _saveService;
    private readonly GameStateMachine _machine;

    public AnswerLockedState(
        GameUI gameUI,
        AudioManager audioManager,
        GameConfig config,
        GameStateData data,
        SaveService saveService,
        GameStateMachine machine)
    {
        _gameUI = gameUI;
        _audioManager = audioManager;
        _config = config;
        _data = data;
        _saveService = saveService;
        _machine = machine;
    }

    public async UniTask Enter()
    {
        if (!_data.UsedQuestionIds.Contains(_data.CurrentQuestion.Id))
            _data.UsedQuestionIds.Add(_data.CurrentQuestion.Id);
        
        _data.CurrentQuestionNumber++;
        
        bool correct = _data.CurrentQuestion.IsCorrect(_data.SelectedAnswerIndex);

        if (correct)
        {
            if (_data.CurrentQuestionNumber >= _config.QuestionsToWin)
            {
                int prize = _config.PrizeAmounts[^1];
                _saveService.Data.wallet += prize;
                _saveService.Data.hasActiveGame = false;
                _data.Reset();
                _saveService.SaveLeaderboard();
                _saveService.Save();
            }
        }
        else
        {
            int safe = GetSafeAmount();
            _saveService.Data.wallet += safe;
            _saveService.Data.hasActiveGame = false;
            _data.Reset();
            _saveService.SaveLeaderboard();
            _saveService.Save();
        }
        
        _gameUI.HighlightAnswer(_data.SelectedAnswerIndex, AnswerHighlightType.Selected);
        _gameUI.SetAnswersInteractable(false);
        _audioManager.PlaySuspense();

        await UniTask.Delay(TimeSpan.FromSeconds(_config.AnswerRevealDelay));
        
        _gameUI.HighlightAnswer(_data.CurrentQuestion.CorrectIndex, AnswerHighlightType.Correct);

        if (correct)
            _audioManager.PlayCorrect();
        else
            _audioManager.PlayWrong();

        await UniTask.Delay(TimeSpan.FromSeconds(_config.ResultDisplayDuration));

        if (correct)
        {
            if (_data.CurrentQuestionNumber >= _config.QuestionsToWin)
                await _machine.TransitionTo(GameState.GameWon);
            else
                await _machine.TransitionTo(GameState.QuestionActive);
        }
        else
        {
            await _machine.TransitionTo(GameState.GameOver);
        }
    }

    public UniTask Exit() => UniTask.CompletedTask;
    
    private int GetSafeAmount()
    {
        //NOTE: readability over memory — LINQ для поиска максимального индекса
        int bestIndex = _config.SafeAmountIndices
            .Where(i => _data.CurrentQuestionNumber > i)
            .DefaultIfEmpty(-1)
            .Max();

        return bestIndex >= 0 ? _config.PrizeAmounts[bestIndex] : 0;
    }
}
