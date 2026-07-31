using System.Collections.Generic;
using YG;

public class GameStateFactory
{
    private readonly GameUI _gameUI;
    private readonly AudioManager _audioManager;
    private readonly GameConfig _config;
    private readonly SaveService _saveService;
    private readonly GameStateData _data;
    private readonly PrizeLadderService _prizeLadder;
    private readonly HintPopupUI _hintPopupUI;
    private readonly RewardedAD _rewardedAD;
    private readonly QuestionBankHolder _questionBankHolder;
    private readonly MenuUI _menuUI;

    public GameStateFactory(
        GameUI gameUI,
        AudioManager audioManager,
        GameConfig config,
        SaveService saveService,
        GameStateData data,
        PrizeLadderService prizeLadder,
        HintPopupUI hintPopupUI,
        RewardedAD rewardedAD,
        QuestionBankHolder questionBankHolder,
        MenuUI menuUI)
    {
        _gameUI = gameUI;
        _audioManager = audioManager;
        _config = config;
        _saveService = saveService;
        _data = data;
        _prizeLadder = prizeLadder;
        _hintPopupUI = hintPopupUI;
        _rewardedAD = rewardedAD;
        _questionBankHolder = questionBankHolder;
        _menuUI = menuUI;
    }

    public List<IGameState> CreateStates(GameStateMachine machine)
    {
        return new List<IGameState>
        {
            new IdleState(_menuUI),
            new IntroState(_gameUI, _audioManager, _config, _saveService, _data, _questionBankHolder.CurrentBank, machine),
            new QuestionActiveState(_gameUI, _audioManager, _config, _saveService, _prizeLadder,
                _hintPopupUI, _rewardedAD, _questionBankHolder.CurrentBank, _data, machine),
            new AnswerLockedState(_gameUI, _audioManager, _config, _data, _saveService, machine),
            new GameOverState(_gameUI, _config, _data, machine),
            new GameWonState(_gameUI, _config, machine)
        };
    }
}
