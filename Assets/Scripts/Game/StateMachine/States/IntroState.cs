using System;
using Cysharp.Threading.Tasks;

public class IntroState : IGameState
{
    public GameState StateId => GameState.Intro;

    private readonly GameUI _gameUI;
    private readonly AudioManager _audioManager;
    private readonly GameConfig _config;
    private readonly SaveService _saveService;
    private readonly GameStateData _data;
    private readonly QuestionBank _questionBank;
    private readonly GameStateMachine _machine;

    public IntroState(
        GameUI gameUI,
        AudioManager audioManager,
        GameConfig config,
        SaveService saveService,
        GameStateData data,
        QuestionBank questionBank,
        GameStateMachine machine)
    {
        _gameUI = gameUI;
        _audioManager = audioManager;
        _config = config;
        _saveService = saveService;
        _data = data;
        _questionBank = questionBank;
        _machine = machine;
    }

    public async UniTask Enter()
    {
        _gameUI.ShowGamePanel();
        _gameUI.HideQuestionPanel();
        _gameUI.HideResultText();
        _audioManager.PlayBackgroundMusic();
        _audioManager.PlayFirstQuestion();
        
        LoadOrRestoreGameData();

        await UniTask.Delay(TimeSpan.FromSeconds(_config.IntroDuration));
        
        await _machine.TransitionTo(GameState.QuestionActive);
    }

    public UniTask Exit() => UniTask.CompletedTask;

    private void LoadOrRestoreGameData()
    {
        if (!_saveService.Data.hasActiveGame)
            return;
        
        _data.CurrentQuestionNumber = _saveService.Data.currentQuestionNumber;
        _data.UsedQuestionIds.Clear();
        _data.UsedQuestionIds.AddRange(_saveService.Data.usedQuestionIds);
        _data.FiftyFiftyUsed = _saveService.Data.usedFiftyFifty;
        _data.AudienceHelpUsed = _saveService.Data.usedAudienceHelp;
        _data.PhoneFriendUsed = _saveService.Data.usedPhoneFriend;
        _data.ReplaceQuestionUsed = _saveService.Data.usedReplaceQuestion;
    }
}
