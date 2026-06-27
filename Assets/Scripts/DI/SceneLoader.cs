using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : ISceneLoader
{
    public async UniTask<SceneInstance> LoadSceneAsync(string sceneAddress, LoadSceneMode mode = LoadSceneMode.Single, bool activateOnLoad = false)
    {
        var handle = Addressables.LoadSceneAsync(sceneAddress, mode, activateOnLoad);
        SceneInstance sceneInstance = await handle.ToUniTask();
        return sceneInstance;
    }

    public void ActivateScene(SceneInstance sceneInstance)
    {
        sceneInstance.ActivateAsync();
    }

    public async UniTask UnloadSceneAsync(SceneInstance sceneInstance)
    {
        var handle = Addressables.UnloadSceneAsync(sceneInstance);
        await handle.ToUniTask();
    }
}

public interface ISceneLoader
{
    UniTask<SceneInstance> LoadSceneAsync(string sceneAddress, LoadSceneMode mode = LoadSceneMode.Single, bool activateOnLoad = false);
    
    void ActivateScene(SceneInstance sceneInstance);
    
    UniTask UnloadSceneAsync(SceneInstance sceneInstance);
}
