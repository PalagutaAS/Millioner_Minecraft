using Cysharp.Threading.Tasks;
using UI;

public enum GameState { Idle, Intro, QuestionActive, AnswerLocked, GameOver, GameWon }
public enum AnswerHighlightType { None, Selected, Correct }
public enum HintType { FiftyFifty, AudienceHelp, PhoneFriend, ReplaceQuestion }

public class GameController
{
    private readonly GameStateMachine _stateMachine;
    private readonly LocalizationUiService _localizationUi;
    private readonly AudioManager _audioManager;
    private readonly MenuUI _menuUI;
    private readonly SaveService _saveService;
    private readonly GameUI _gameUI;
    
    public GameController(
        GameStateMachine stateMachine,
        LocalizationUiService localizationUi,
        AudioManager audioManager,
        MenuUI menuUI,
        SaveService saveService,
        GameUI gameUI
        )
    {
        _stateMachine = stateMachine;
        _menuUI = menuUI;
        _audioManager = audioManager;
        _localizationUi = localizationUi;
        _saveService = saveService;
        _gameUI = gameUI;
    }

    public void Initialize()
    {
        _saveService.Load();
        
        _gameUI.HideQuestionPanel();
        _gameUI.HideResultText();
        _localizationUi.UpdateDropdown();
        _audioManager.UpdateSlider(_saveService.Data.volume);
        _menuUI.OnPlayClicked += HandlePlayClicked;
        _stateMachine.TransitionTo(GameState.Idle).Forget();
    }

    private void HandlePlayClicked()
    {
        _stateMachine.TransitionTo(GameState.Intro).Forget();
    }
}
