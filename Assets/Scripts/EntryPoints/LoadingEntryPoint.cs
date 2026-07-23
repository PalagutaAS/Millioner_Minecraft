using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;
using Random = UnityEngine.Random;

public class LoadingEntryPoint : IInitializable
{
    private LoadingUI _loadingUI;
    private IQuestionBankCreationService _questionBankCreationService;
    private ISceneLoader _sceneLoader;
    
    private const string MainSceneAddress = "Main";

    public LoadingEntryPoint(LoadingUI loadingUi, IQuestionBankCreationService questionBankCreationService, ISceneLoader sceneLoader)
    {
        _loadingUI = loadingUi;
        _questionBankCreationService = questionBankCreationService;
        _sceneLoader = sceneLoader;
    }

    public void Initialize()
    {
        LoadGameAsync().Forget();
    }

    private async UniTaskVoid LoadGameAsync()
    {
        _loadingUI.gameObject.SetActive(true);
        _loadingUI.SetProgress(0f);

        var realAssetTask = _questionBankCreationService.CreateQuestionBankAsync();
        
        var fakeTask = FakeLoadAsync();
        
        await UniTask.WhenAll(realAssetTask, fakeTask);
        
        _loadingUI.SetProgress(1f);
        
        await UniTask.NextFrame();
        
        var sceneInstance = await _sceneLoader.LoadSceneAsync(MainSceneAddress, LoadSceneMode.Additive, true);

        await _loadingUI.FadeOutAsync();

        await UniTask.NextFrame();
        
        await SceneManager.UnloadSceneAsync(_loadingUI.gameObject.scene);

        SceneManager.SetActiveScene(sceneInstance.Scene);
    }
    
    private async UniTask FakeLoadAsync()
    {
        float totalMoveDuration = 0.2f;
        
        float p1 = Random.Range(0.10f, 0.35f);
        float p2 = Random.Range(0.41f, 0.65f);
        float p3 = Random.Range(0.70f, 0.95f);
        
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

        await MoveProgressAsync(p3, 1f, t4);

        _loadingUI.SetProgress(1f);
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
}
