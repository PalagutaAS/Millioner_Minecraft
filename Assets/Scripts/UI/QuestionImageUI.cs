using UnityEngine;
using UnityEngine.UI;

public class QuestionImageUI : MonoBehaviour
{
    [SerializeField] private Image _image;
    
    public void Show(Sprite sprite)
    {
        if (sprite == null)
        {
            Hide();
            return;
        }

        if (_image == null)
            _image = GetComponent<Image>();

        _image.sprite = sprite;
        _image.preserveAspect = true;
        gameObject.SetActive(true);
    }
    
    public void Hide() => gameObject.SetActive(false);
}
