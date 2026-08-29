using Cysharp.Threading.Tasks;

public enum GameState { Idle, Intro, QuestionActive, AnswerLocked, GameOver, GameWon }
public enum AnswerHighlightType { None, Selected, Correct }
public enum HintType { FiftyFifty, AudienceHelp, PhoneFriend, ReplaceQuestion }

public class GameController
{
    private readonly GameStateMachine _stateMachine;
    private readonly GameStateData _data;
    private readonly MenuUI _menuUI;
    private readonly SaveService _saveService;
    private readonly GameUI _gameUI;
    
    public GameController(
        GameStateMachine stateMachine,
        GameStateData data,
        MenuUI menuUI,
        SaveService saveService,
        GameUI gameUI)
    {
        _stateMachine = stateMachine;
        _data = data;
        _menuUI = menuUI;
        _saveService = saveService;
        _gameUI = gameUI;
    }

    public void Initialize()
    {
        _saveService.Load();
        //_saveService.Save();
        
        _gameUI.HideQuestionPanel();
        _gameUI.HideResultText();

        _menuUI.OnPlayClicked += HandlePlayClicked;
        _stateMachine.TransitionTo(GameState.Idle).Forget();
    }

    private void HandlePlayClicked()
    {
        _stateMachine.TransitionTo(GameState.Intro).Forget();
    }
}
