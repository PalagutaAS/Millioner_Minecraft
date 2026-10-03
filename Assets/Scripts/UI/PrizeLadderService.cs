using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class PrizeLadderService : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private PrizeRow[] _rows;
    [Inject] private GameConfig _gmConfig;
    [SerializeField] private Color _inactiveColor = new(1f, 1f, 1f, 0f);

    public int RowCount => _rows.Length;

    private void Start()
    {
        var d = _rows;
        for (int i = 0; i < d.Length; i++)
        {
            if (_rows[i].Text != null && i < _gmConfig.PrizeAmounts.Length)
            {
                _rows[i].Text.text = _gmConfig.PrizeAmounts[i].ToString("N0");
            }
        }
    }

    public void SetHighlightedRow(int index)
    {
        for (int i = 0; i < _rows.Length; i++)
        {
            if (_rows[i].Image == null) continue;
            _rows[i].Image.color = _inactiveColor;
            _rows[i].PointImage.gameObject.SetActive(i < index);
            HighlightSafeAmountText(index, i);
            if (i < index)
            {
                _rows[i].Text.color = _gmConfig.CorrectColor;
                _rows[i].NumberText.color = _gmConfig.CorrectColor;
            }
        }
        
        foreach (int amountIndex in _gmConfig.SafeAmountIndices)
        {
            if(index == amountIndex)
            {
                _rows[index].Text.color = _gmConfig.TextColor;
            }
        }
        
        _rows[index].Image.color = _gmConfig.SelectedColor;
    }

    private void HighlightSafeAmountText(int index, int i)
    {
        foreach (int amountIndex in _gmConfig.SafeAmountIndices)
        {
            if (amountIndex == i && index != i)
            {
                _rows[i].Text.color = _gmConfig.SelectedColor;
                _rows[i].PointImage.color = _gmConfig.SelectedColor;
            }
        }
    }

    public void ResetAll()
    {
        foreach (var row in _rows)
        {
            if (row.Image != null)
            {
                row.Text.color = _gmConfig.TextColor;
                row.NumberText.color = _gmConfig.TextColor;
                row.Image.color = _inactiveColor;
            }
        }
    }
}
