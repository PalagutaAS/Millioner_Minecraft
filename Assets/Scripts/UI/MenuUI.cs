using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _menuButton;
    [SerializeField] private GameObject _menuUI;
    
    private SaveService _saveService;

    public event Action OnPlayClicked;

    [Inject] private void Constructor(SaveService saveService)
    {
        _saveService = saveService;
    }
    
    private void OnEnable()
    {
        _playButton.onClick.AddListener(HandlePlay);
        _menuButton.onClick.AddListener(HandleMenu);
    }

    private void HandleMenu()
    {
        _menuUI.SetActive(!_menuUI.activeSelf);
        Time.timeScale = _menuUI.activeSelf ? 0 : 1;
        if (!_menuUI.activeSelf)
        {
            _saveService.Save();
        }
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
