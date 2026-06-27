using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(CanvasGroup))]
public class LoadingUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _duration = 1.0f;
    [SerializeField] private Image _progressImage;

    public void SetProgress(float value)
    {
        if (_progressImage != null)
            _progressImage.fillAmount = Mathf.Clamp01(value);
    }
    
    public async UniTask FadeOutAsync()
    {
        float elapsedTime = 0f;

        while (elapsedTime < _duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / _duration);
            
            _canvasGroup.alpha = alpha;
            
            await UniTask.Yield();
        }

        // Убедимся, что альфа точно 0
        _canvasGroup.alpha = 0f;

        // Отключаем объект после завершения анимации
        gameObject.SetActive(false);
    }

    public float Duration
    {
        get { return _duration; }
        set { _duration = value; }
    }
}
