using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _menuButton;
    [SerializeField] private GameObject _menuUI;
    [Inject] private GameConfig _gameConfig;

    public event Action OnPlayClicked;
    
    private void OnEnable()
    {
        _playButton.onClick.AddListener(HandlePlay);
        _menuButton.onClick.AddListener(HandleMenu);
    }

    private void HandleMenu()
    {
        _menuUI.SetActive(!gameObject.activeSelf);
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
