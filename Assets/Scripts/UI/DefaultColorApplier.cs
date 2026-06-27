using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class DefaultColorApplier : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text[] _defaultText;
    [SerializeField] private TMP_Text[] _charText;
    private GameConfig _config;
    
    public Button Button => _button;
    public TMP_Text[] DefaultText => _defaultText;
    public TMP_Text[] CharText => _charText;

    [Inject]
    public void Construct(GameConfig config)
    {
        _config = config;
        ApplyColor();
    }

    private void ApplyColor()
    {
        if (_button != null)
        {
            var colors = _button.colors;
            colors.highlightedColor = _config.HighlightColor;
            _button.colors = colors;
        }
        
        if (_charText.Length > 0)
        {
            foreach (var tmpText in _charText)
            {
                tmpText.color = _config.SelectedColor;
            }
        }
        
        if (_defaultText.Length > 0)
        {
            foreach (var tmpText in _defaultText)
            {
                tmpText.color = _config.TextColor;
            }
        }

    }
}
