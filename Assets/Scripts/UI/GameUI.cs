using System;
using UnityEngine;
using TMPro;
using YG;
using YG.Utils.LB;

public class GameUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _gamePanel;
    [SerializeField] private GameObject _questionPanel;
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private GameObject _progressPanel;

    [Header("Question")]
    [SerializeField] private TextMeshProUGUI _questionText;

    [Header("Answers")]
    [SerializeField] private DefaultColorApplier[] _answerButtons;
    [SerializeField] private TextMeshProUGUI[] _answerTexts;
    [Header("Hints")]
    [SerializeField] private HintButton _fiftyFifty;
    [SerializeField] private HintButton _audienceHelp;
    [SerializeField] private HintButton _phoneFriend;
    [SerializeField] private HintButton _replaceQuestion;

    [Header("Result Test")]
    [SerializeField] private TMP_Text _resultEndGameText;

    public event Action<int> OnAnswerClicked;
    public event Action<HintType> OnHintClicked;

    private GameConfig _config;
    private LBData _playerData;
    private SaveService _saveService;

    public void Initialize(GameConfig config, SaveService saveService)
    {
        _config = config;
        _saveService = saveService;
        YG2.onGetLeaderboard += OnGetLeaderboard;
    }

    private void OnGetLeaderboard(LBData playerData)
    {
        _playerData = playerData;
        _resultEndGameText.text += $"\n\nВаше место в рейтинге: {_playerData.currentPlayer.rank}";
    }

    private void OnEnable()
    {
        for (int i = 0; i < _answerButtons.Length; i++)
        {
            int index = i;
            _answerButtons[i].Button.onClick.AddListener(() => OnAnswerClicked?.Invoke(index));
        }

        _fiftyFifty.Button.onClick.AddListener(() => OnHintClicked?.Invoke(HintType.FiftyFifty));
        _audienceHelp.Button.onClick.AddListener(() => OnHintClicked?.Invoke(HintType.AudienceHelp));
        _phoneFriend.Button.onClick.AddListener(() => OnHintClicked?.Invoke(HintType.PhoneFriend));
        _replaceQuestion.Button.onClick.AddListener(() => OnHintClicked?.Invoke(HintType.ReplaceQuestion));
    }

    private void OnDisable()
    {
        foreach (var btn in _answerButtons)
            btn.Button.onClick.RemoveAllListeners();

        _fiftyFifty.Button.onClick.RemoveAllListeners();
        _audienceHelp.Button.onClick.RemoveAllListeners();
        _phoneFriend.Button.onClick.RemoveAllListeners();
        _replaceQuestion.Button.onClick.RemoveAllListeners();
    }

    public void ShowGamePanel() => _gamePanel.SetActive(true);

    public void ShowQuestionPanel()
    {
        _progressPanel.SetActive(true);
        _questionPanel.SetActive(true);
    }

    public void HideQuestionPanel()
    {
        _progressPanel.SetActive(false);
        _questionPanel.SetActive(false);
    }

    public void SetResultText(bool won, int amount)
    {
        _resultPanel.SetActive(true);
        _resultEndGameText.text = won
            ? $"<color=#2ecc71>Поздравляем!</color>\n\nВы выигрыш:\n<b>{amount:N0}</b>!\n\nОбщий выигрыш составляет:\n\n<color=#FFC125><b>{_saveService.Data.wallet:N0}</b></color>"
            : $"<color=#e74c3c>Игра окончена</color>\n\nВаш выигрыш:\n<b>{amount:N0}</b>\n\nОбщий выигрыш составляет:\n\n<color=#FFC125><b>{_saveService.Data.wallet:N0}</b></color>";
    }
    
    public void SetQuestionText(string text) => _questionText.text = text;

    public void SetAnswers(params string[] answers)
    {
        for (int i = 0; i < _answerTexts.Length; i++)
        {
            _answerTexts[i].text = i < answers.Length ? answers[i] : "";
        }
    }

    public void ShowAnswerButtons(int count)
    {
        for (int i = 0; i < _answerButtons.Length; i++)
            _answerButtons[i].Button.gameObject.SetActive(i < count);
    }

    public void ResetAnswerHighlights()
    {
        foreach (var button in _answerButtons)
        {
            button.Button.image.sprite = _config.DefaultSprite;
        }
        foreach (var button in _answerButtons)
        {
            foreach (var text in button.CharText)
            {
                text.color = _config.SelectedColor;
            }        
        }
    }

    public void HighlightAnswer(int index, AnswerHighlightType type)
    {
        if (index < 0 || index >= _answerButtons.Length) return;

        _answerButtons[index].Button.image.sprite = type switch
        {
            AnswerHighlightType.Selected => _config.SelectedSprite,
            AnswerHighlightType.Correct => _config.CorrectSprite,
            _ => _config.DefaultSprite
        };
        
        foreach (var text in _answerButtons[index].CharText)
        {
            text.color = (type is AnswerHighlightType.Selected or AnswerHighlightType.Correct) ? _config.TextColor : _config.SelectedColor;
        }
        
    }

    public void HideAnswerButton(int index)
    {
        if (index >= 0 && index < _answerButtons.Length)
            _answerButtons[index].gameObject.SetActive(false);
    }

    public void SetAnswersInteractable(bool interactable)
    {
        foreach (var btn in _answerButtons)
            btn.Button.interactable = interactable;
    }

    public void SetHintActive(HintType type, bool active)
    {
        HintButton btn = type switch
        {
            HintType.FiftyFifty => _fiftyFifty,
            HintType.AudienceHelp => _audienceHelp,
            HintType.PhoneFriend => _phoneFriend,
            HintType.ReplaceQuestion => _replaceQuestion,
            _ => null
        };

        if (btn != null)
            btn.SetInteractable(active);
    }

    public void HideResultText()
    {
        _resultPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        YG2.onGetLeaderboard -= OnGetLeaderboard;
    }
}
