using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class QuestionLoaderService : IQuestionLoaderService
{
    public async UniTask<string> LoadQuestionsAsync(string address)
    {
        AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>(address);
        await handle.Task;
        string jsonText = null;
        
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            jsonText = handle.Result.text;
            Addressables.Release(handle);
        }
        else
        {
            Debug.LogError($"Не удалось загрузить Addressable: {address}");
        }
        
        return jsonText;
    }
}