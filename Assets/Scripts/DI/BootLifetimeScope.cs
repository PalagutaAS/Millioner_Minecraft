using UnityEngine;
using VContainer;
using VContainer.Unity;

public class BootLifetimeScope : LifetimeScope
{
    [SerializeField] private LoadingUI _loadingUI;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<LoadingEntryPoint>();

        builder.Register<SceneLoader>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.Register<QuestionBankHolder>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.Register<QuestionBankCreationService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.Register<QuestionByIdRetrievalService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.Register<QuestionCategoryService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.Register<QuestionLoaderService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.Register<QuestionParserService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.Register<QuestionRetrievalService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

        builder.RegisterComponent(_loadingUI);

        DontDestroyOnLoad(gameObject);
    }
}
