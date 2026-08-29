using Localization;
using VContainer.Unity;
using YG;

public class EntryPoint : IInitializable
{
    private readonly GameController _gameController;
    private readonly LocalizationManager _localizationManager;

    public EntryPoint(GameController gameController, LocalizationManager localizationManager)
    {
        _gameController = gameController;
        _localizationManager = localizationManager;
    }

    public void Initialize()
    {
        _gameController.Initialize();
        _localizationManager.Initialize();
        YG2.GameReadyAPI();
    }
}
