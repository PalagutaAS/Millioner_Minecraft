using UnityEngine;
using UnityEngine.UI;

public class HintButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _imageInactive;
    
    public Button Button => _button;

    
    public void SetInteractable(bool interactable)
    {
        _button.interactable = interactable;
        _imageInactive.enabled = !interactable;
    }
}
