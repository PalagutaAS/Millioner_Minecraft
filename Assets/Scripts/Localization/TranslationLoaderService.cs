using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Localization
{
    public class TranslationLoaderService : ITranslationLoaderService
    {
        private string _csvText;
        private bool _isLoaded = false;
        
        public string CSV => _csvText;
        public bool IsLoaded => _isLoaded;

        public async UniTask LoadTranslationsAsync(string address)
        {
            try
            {
                AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>(address);
                await handle.Task;
                
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    _csvText = handle.Result.text;
                    _isLoaded = true;
                    Debug.Log($"Переводы успешно загружены из Addressables: {address}");
                    Addressables.Release(handle);
                }
                else
                {
                    _isLoaded = false;
                    Debug.LogError($"Не удалось загрузить Addressable: {address}");
                    Debug.LogError($"Ошибка: {handle.OperationException?.Message}");
                }
            }
            catch (Exception ex)
            {
                _isLoaded = false;
                Debug.LogError($"Исключение при загрузке переводов: {ex.Message}");
            }
        }
    }

    public interface ITranslationLoaderService
    {
        UniTask LoadTranslationsAsync(string address);
        string CSV { get; }
        bool IsLoaded { get; }
    }
}