using VContainer.Unity;
using YG;

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
        YG2.GameReadyAPI();
    }
}
