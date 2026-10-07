using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;


public class QuestionImageService
{
    private readonly Dictionary<int, Sprite> _cache = new();

    public async UniTask<Sprite> LoadSpriteAsync(int questionId)
    {
        if (_cache.TryGetValue(questionId, out Sprite cachedSprite))
            return cachedSprite;

        string address = $"{questionId}.jpg";
        
        AsyncOperationHandle<IList<IResourceLocation>> locationsHandle =
            Addressables.LoadResourceLocationsAsync(address);
        IList<IResourceLocation> locations = await locationsHandle.Task;

        bool exists = locations is { Count: > 0 };
        Addressables.Release(locationsHandle);

        if (!exists)
            return null;
        

        AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(address);
        await handle.Task;

        if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
        {
            Debug.LogWarning($"Не удалось загрузить изображение вопроса: {address}");
            return null;
        }

        Sprite sprite = handle.Result;
        _cache[questionId] = sprite;
        // NOTE: handle не освобождаем — спрайт кэшируется и используется повторно.
        return sprite;
    }
}
