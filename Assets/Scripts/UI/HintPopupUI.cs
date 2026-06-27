using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HintPopupUI : MonoBehaviour
{
    [Header("Raycast Blocker")]
    [SerializeField] private Image _panelRaycastBlocker;
    
    [Header("Audience Help")]
    [SerializeField] private GameObject _audiencePopup;
    [SerializeField] private Slider[] _audienceSliders;
    [SerializeField] private TextMeshProUGUI[] _audienceLabels;
    [SerializeField] private Button _audienceCloseButton;

    [Header("Phone Friend")]
    [SerializeField] private GameObject _phonePopup;
    [SerializeField] private TextMeshProUGUI _phoneText;
    [SerializeField] private Button _phoneCloseButton;

    public event Action OnPopupClosed;

    private void OnEnable()
    {
        _audienceCloseButton.onClick.AddListener(CloseAudiencePopup);
        _phoneCloseButton.onClick.AddListener(ClosePhonePopup);
    }

    private void OnDisable()
    {
        _audienceCloseButton.onClick.RemoveAllListeners();
        _phoneCloseButton.onClick.RemoveAllListeners();
    }

    public void ShowAudienceHelp(float[] values)
    {
        for (int i = 0; i < _audienceSliders.Length && i < values.Length; i++)
        {
            _audienceSliders[i].value = values[i] / 100f;
            if (_audienceLabels.Length > 0)
            {
                _audienceLabels[i].text = $"{values[i]:F0}%";
            }
        }

        _audiencePopup.SetActive(true);
        _panelRaycastBlocker.enabled = true;
    }

    public void ShowPhoneFriend(string message)
    {
        _phoneText.text = message;
        _phonePopup.SetActive(true);
        _panelRaycastBlocker.enabled = true;
    }

    private void CloseAudiencePopup()
    {
        _audiencePopup.SetActive(false);
        _panelRaycastBlocker.enabled = false;
        OnPopupClosed?.Invoke();
    }

    private void ClosePhonePopup()
    {
        _phonePopup.SetActive(false);
        _panelRaycastBlocker.enabled = false;
        OnPopupClosed?.Invoke();
    }
}
