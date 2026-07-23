using UnityEngine;
using VContainer;
using VContainer.Unity;
using YG;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private GameUI _gameUI;
    [SerializeField] private MenuUI _menuUI;
    [SerializeField] private HintPopupUI _hintPopupUI;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private PrizeLadderService _prizeLadder;
    [SerializeField] private GameConfig _gameConfig;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(_gameUI);
        builder.RegisterInstance(_menuUI);
        builder.RegisterInstance(_hintPopupUI);
        builder.RegisterInstance(_audioManager);
        builder.RegisterInstance(_prizeLadder);
        builder.RegisterInstance(_gameConfig);
        
        builder.Register<RewardedAD>(Lifetime.Singleton).AsSelf();
        builder.Register<SaveService>(Lifetime.Singleton);

        builder.Register<GameController>(Lifetime.Singleton);
        builder.RegisterEntryPoint<EntryPoint>();
    }
}
