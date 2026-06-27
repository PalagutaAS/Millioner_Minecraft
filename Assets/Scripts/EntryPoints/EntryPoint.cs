using VContainer.Unity;

public class EntryPoint : IInitializable
{
    private readonly GameController _gameController;

    public EntryPoint(GameController gameController)
    {
        _gameController = gameController;
    }

    public void Initialize()
    {
        _gameController.Initialize();
    }
}
