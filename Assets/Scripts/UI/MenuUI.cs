using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using YG;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _menuButton;
    [SerializeField] private Button _languageButton;
    [SerializeField] private GameObject _menuUI;
    [Inject] private GameConfig _gameConfig;
    [Inject] private SaveService _saveService;

    public event Action OnPlayClicked;
    
    private void OnEnable()
    {
        _playButton.onClick.AddListener(HandlePlay);
        _menuButton.onClick.AddListener(HandleMenu);
        _languageButton.onClick.AddListener(HandleLanguage);
    }

    private void HandleLanguage()
    {
        _saveService.SwitchLanguage();
    }

    private void HandleMenu()
    {
        _menuUI.SetActive(!_menuUI.activeSelf);
        Time.timeScale = _menuUI.activeSelf ? 0 : 1;
    }

    private void OnDisable()
    {
        _playButton.onClick.RemoveAllListeners();
        _menuButton.onClick.RemoveAllListeners();
    }

    public void ShowPlayButton() => _playButton.gameObject.SetActive(true);
    public void HidePlayButton() => _playButton.gameObject.SetActive(false);

    private void HandlePlay() => OnPlayClicked?.Invoke();
}
