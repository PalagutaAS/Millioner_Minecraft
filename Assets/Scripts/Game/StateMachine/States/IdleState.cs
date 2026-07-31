using Cysharp.Threading.Tasks;

public class IdleState : IGameState
{
    public GameState StateId => GameState.Idle;

    private readonly MenuUI _menuUI;

    public IdleState(MenuUI menuUI)
    {
        _menuUI = menuUI;
    }

    public UniTask Enter()
    {
        _menuUI.ShowPlayButton();
        return UniTask.CompletedTask;
    }

    public UniTask Exit()
    {
        _menuUI.HidePlayButton();
        return UniTask.CompletedTask;
    }
}
