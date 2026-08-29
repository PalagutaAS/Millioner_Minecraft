using System;
using Cysharp.Threading.Tasks;
using Localization;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;
using YG;
using Random = UnityEngine.Random;

public class LoadingEntryPoint : IInitializable
{
    private LoadingUI _loadingUI;
    private IQuestionBankCreationService _questionBankCreationService;
    private ISceneLoader _sceneLoader;
    private ITranslationLoaderService _translationLoaderService;

    private const string MainSceneAddress = "Main";

    public LoadingEntryPoint(
        LoadingUI loadingUi,
        IQuestionBankCreationService questionBankCreationService,
        ISceneLoader sceneLoader, 
        ITranslationLoaderService translationLoaderService)
    {
        _loadingUI = loadingUi;
        _questionBankCreationService = questionBankCreationService;
        _sceneLoader = sceneLoader;
        _translationLoaderService = translationLoaderService;
    }

    public void Initialize()
    {
        LoadGameAsync().Forget();
    }

    private async UniTaskVoid LoadGameAsync()
    {
        _loadingUI.gameObject.SetActive(true);
        _loadingUI.SetProgress(0f);
        try
        {
            var realAssetTask = _questionBankCreationService.CreateQuestionBankAsync();
            var translationsTask = _translationLoaderService.LoadTranslationsAsync("localization");
            var fakeTask = FakeLoadAsync();
            var sdkTask = WaitForSDKInitializationAsync();
        
            // Ждем завершения всех задач
            await UniTask.WhenAll(realAssetTask, fakeTask, translationsTask, sdkTask);
            
            _loadingUI.SetProgress(0.9f);
            
            // Получаем результат загрузки сцены
            var sceneTask = _sceneLoader.LoadSceneAsync(MainSceneAddress, LoadSceneMode.Additive, true);
            var sceneInstance = await sceneTask;
            
            var progressTask = MoveProgressAsync(0.9f, 1f, _loadingUI.Duration);
            var fadeTask = _loadingUI.FadeOutAsync();

            await UniTask.WhenAll(progressTask, fadeTask);
            
            await UniTask.NextFrame();
        
            await SceneManager.UnloadSceneAsync(_loadingUI.gameObject.scene);

            SceneManager.SetActiveScene(sceneInstance.Scene);
        }
        catch(Exception ex)
        {
            Debug.LogError($"Ошибка при загрузке игры: {ex.Message}");
        }
    }
    
    private async UniTask FakeLoadAsync()
    {
        float totalMoveDuration = 0.2f;
        
        float p1 = Random.Range(0.10f, 0.30f);
        float p2 = Random.Range(0.45f, 0.60f);
        float p3 = Random.Range(0.65f, 0.80f);
        
        float t1 = p1 * totalMoveDuration;
        float t2 = (p2 - p1) * totalMoveDuration;
        float t3 = (1f - p2) * totalMoveDuration;
        float t4 = (1f - p3) * totalMoveDuration;
        
        await MoveProgressAsync(0f, p1, t1);

        float delay1 = Random.Range(0.1f, 0.3f);
        await UniTask.Delay(TimeSpan.FromSeconds(delay1));

        await MoveProgressAsync(p1, p2, t2);

        float delay2 = Random.Range(0.2f, 0.35f);
        await UniTask.Delay(TimeSpan.FromSeconds(delay2));

        await MoveProgressAsync(p2, p3, t3);
        
        float delay3 = Random.Range(0.05f, 0.2f);
        await UniTask.Delay(TimeSpan.FromSeconds(delay3));

        await MoveProgressAsync(p3, 0.9f, t4);
    }
    
    private async UniTask MoveProgressAsync(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float progress = Mathf.Lerp(from, to, t);
            _loadingUI.SetProgress(progress);
            await UniTask.Yield();
        }
        
        _loadingUI.SetProgress(to);
    }
    
    private async UniTask WaitForSDKInitializationAsync()
    {
        if (YG2.isSDKEnabled)
        {
            Debug.Log("✅ SDK уже инициализирован");
            return;
        }

        var tcs = new UniTaskCompletionSource<bool>();
        
        Action onSDKData = () => tcs.TrySetResult(true);
        
        YG2.onGetSDKData += onSDKData;
        
        // Создаем задачу с таймаутом
        var timeoutTask = UniTask.Delay(TimeSpan.FromSeconds(60));
        var resultTask = tcs.Task;
        
        (bool hasResultLeft, bool result) completedTask = await UniTask.WhenAny(resultTask, timeoutTask);
        
        YG2.onGetSDKData -= onSDKData;
        
        if (completedTask.hasResultLeft)
        {
            if (resultTask.Status == UniTaskStatus.Succeeded)
            {
                var success = await resultTask;
                if (success)
                {
                    Debug.Log("✅ SDK успешно инициализирован");
                }
            }
            else if (resultTask.Status == UniTaskStatus.Faulted)
            {
                throw new Exception("Ошибка инициализации SDK.");
            }
        }
        else
        {
            if (YG2.isSDKEnabled)
            {
                Debug.Log("✅ SDK успел инициализироваться до таймаута");
                return;
            }

            throw new TimeoutException($"Инициализация SDK не завершена за 60 секунд");
        }
    }
}
