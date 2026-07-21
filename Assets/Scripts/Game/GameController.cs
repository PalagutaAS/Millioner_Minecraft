using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YG;
using Random = UnityEngine.Random;

public enum GameState { Idle, Intro, QuestionActive, AnswerLocked, GameOver, GameWon }
public enum AnswerHighlightType { None, Selected, Correct }
public enum HintType { FiftyFifty, AudienceHelp, PhoneFriend, ReplaceQuestion }

public class GameController
{
    private readonly QuestionBank _questionBank;
    private readonly SaveService _saveService;
    private readonly AudioManager _audioManager;
    private readonly GameUI _gameUI;
    private readonly MenuUI _menuUI;
    private readonly HintPopupUI _hintPopupUI;
    private readonly PrizeLadderService _prizeLadder;
    private readonly GameConfig _config;

    private string _currentLanguage;
    private readonly List<int> _usedQuestionIds = new();
    private int _currentQuestionNumber;
    private int _currentQuestionId;
    private bool _fiftyFiftyUsed, _audienceHelpUsed, _phoneFriendUsed, _replaceQuestionUsed;
    private readonly bool[] _activeAnswers = new bool[4];
    private CurrentQuestion _currentQuestion;
    private RewardedAD _rewardedAD;

    public GameState State { get; private set; } = GameState.Idle;

    public GameController(
        QuestionBankHolder questionBank,
        SaveService saveService,
        AudioManager audioManager,
        GameUI gameUI,
        MenuUI menuUI,
        HintPopupUI hintPopupUI,
        PrizeLadderService prizeLadder,
        GameConfig config,
        RewardedAD rewardedAD)
    {
        _questionBank = questionBank.CurrentBank;
        _saveService = saveService;
        _audioManager = audioManager;
        _gameUI = gameUI;
        _menuUI = menuUI;
        _hintPopupUI = hintPopupUI;
        _prizeLadder = prizeLadder;
        _config = config;
        _currentLanguage = saveService.LoadLanguage();
        _rewardedAD = rewardedAD;
    }

    public void Initialize()
    {
        _saveService.Load();
        _saveService.Save();

        _gameUI.OnAnswerClicked += HandleAnswerClicked;
        _gameUI.OnHintClicked += HandleHintClicked;

        _menuUI.OnPlayClicked += HandlePlayClicked;

        _hintPopupUI.OnPopupClosed += HandlePopupClosed;

        _gameUI.Initialize(_config, _saveService);
        _gameUI.HideQuestionPanel();
        _prizeLadder.ResetAll();
    }

    private void HandlePlayClicked()
    {
        if (State != GameState.Idle) return;
        StartGameAsync().Forget();
    }

    private async UniTaskVoid StartGameAsync()
    {
        State = GameState.Intro;
        _gameUI.ShowGamePanel();
        _gameUI.HideQuestionPanel();
        _gameUI.HideResultText();
        _menuUI.HidePlayButton();
        _audioManager.PlayBackgroundMusic();
        _audioManager.PlayFirstQuestion();
        
        await UniTask.Delay(TimeSpan.FromSeconds(_config.IntroDuration));

        int savedQuestionId = -1;

        if (!_saveService.Data.hasActiveGame)
        {
            _currentQuestionNumber = 0;
            _usedQuestionIds.Clear();
            ResetHints();
        }
        else
        {
            _currentQuestionNumber = _saveService.Data.currentQuestionNumber;
            savedQuestionId = _saveService.Data.currentQuestionId;
            _usedQuestionIds.Clear();
            _usedQuestionIds.AddRange(_saveService.Data.usedQuestionIds);
            _fiftyFiftyUsed = _saveService.Data.usedFiftyFifty;
            _audienceHelpUsed = _saveService.Data.usedAudienceHelp;
            _phoneFriendUsed = _saveService.Data.usedPhoneFriend;
            _replaceQuestionUsed = _saveService.Data.usedReplaceQuestion;
        }

        State = GameState.QuestionActive;
        ShowNextQuestion(savedQuestionId >= 0 ? savedQuestionId : null);
        _gameUI.ShowQuestionPanel();
    }

    private void ShowNextQuestion(int? restoreQuestionId = null)
    {
        int questionId;
        LocalizedContent content;

        if (restoreQuestionId.HasValue)
        {
            questionId = restoreQuestionId.Value;
            content = _questionBank.GetQuestionById(questionId, _currentLanguage);
        }
        else
        {
            string category = GetCategoryForQuestion(_currentQuestionNumber);
            var availableIds = _questionBank.GetQuestionIds(category)
                .Where(id => !_usedQuestionIds.Contains(id))
                .ToList();

            if (availableIds.Count == 0)
            {
                _usedQuestionIds.Clear();
                availableIds = _questionBank.GetQuestionIds(category).ToList();
            }

            questionId = availableIds[Random.Range(0, availableIds.Count)];
            content = _questionBank.GetQuestion(category, questionId, _currentLanguage);
        }

        _usedQuestionIds.Add(questionId);

        if (content == null)
        {
            Debug.LogError($"Question {questionId} not found for language {_currentLanguage}");
            return;
        }

        bool restored = restoreQuestionId.HasValue
            && _saveService.Data.shuffleMap.Length == 4
            && _saveService.Data.activeAnswers.Length == 4;

        if (restored)
        {
            _currentQuestion = new CurrentQuestion(content, _saveService.Data.shuffleMap);
            for (int i = 0; i < 4; i++)
                _activeAnswers[i] = _saveService.Data.activeAnswers[i];
        }
        else
        {
            _currentQuestion = new CurrentQuestion(content);
            ResetActiveAnswers(4);
        }

        _currentQuestionId = questionId;
        _gameUI.SetQuestionText(_currentQuestion.QuestionText);
        _gameUI.SetAnswers(_currentQuestion.ShuffledAnswers);
        _gameUI.ShowAnswerButtons(4);

        if (restored)
        {
            for (int i = 0; i < 4; i++)
            {
                if (!_activeAnswers[i])
                    _gameUI.HideAnswerButton(i);
            }
        }

        _gameUI.ResetAnswerHighlights();
        _gameUI.SetAnswersInteractable(true);
        _prizeLadder.SetHighlightedRow(_currentQuestionNumber);
        UpdateHintButtons();
        SaveProgress();
    }

    private string GetCategoryForQuestion(int questionNumber)
    {
        Debug.Log($"questionNumber: {questionNumber}");
        if (questionNumber < 4) return "easy";
        if (questionNumber < 9) return "medium";
        if (questionNumber < 14) return "hard";
        return "very_hard";
    }

    private void HandleAnswerClicked(int index)
    {
        if (State != GameState.QuestionActive || !_activeAnswers[index]) return;

        State = GameState.AnswerLocked;
        _gameUI.HighlightAnswer(index, AnswerHighlightType.Selected);
        _gameUI.SetAnswersInteractable(false);
        _audioManager.PlaySuspense();
        RevealAnswerAsync(index).Forget();
    }

    private async UniTaskVoid RevealAnswerAsync(int answerIndex)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_config.AnswerRevealDelay));

        int correctIdx = _currentQuestion.CorrectIndex;

        bool correct = _currentQuestion.IsCorrect(answerIndex);
        _gameUI.HighlightAnswer(correctIdx, AnswerHighlightType.Correct);

        if (correct)
            _audioManager.PlayCorrect();
        else
            _audioManager.PlayWrong();

        await UniTask.Delay(TimeSpan.FromSeconds(_config.ResultDisplayDuration));

        if (correct)
        {
            _currentQuestionNumber++;

            if (_currentQuestionNumber >= _config.QuestionsToWin)
            {
                await WinGameAsync();
            }
            else
            {
                ShowNextQuestion();
                State = GameState.QuestionActive;
            }
        }
        else
        {
            int safe = GetSafeAmount();
            _saveService.Data.wallet += safe;
            _saveService.Data.hasActiveGame = false;
            _saveService.Save();

            _gameUI.HideQuestionPanel();
            _gameUI.ShowGamePanel();
            _gameUI.SetResultText(false, safe);
            State = GameState.GameOver;

            await UniTask.Delay(TimeSpan.FromSeconds(_config.EndGameDelay));
            ReturnToMenu();
        }
    }

    private async UniTask WinGameAsync()
    {
        int prize = _config.PrizeAmounts[^1];
        _saveService.Data.wallet += prize;
        _saveService.Data.hasActiveGame = false;
        _saveService.Save();

        _gameUI.HideQuestionPanel();
        _gameUI.SetResultText(true, prize);
        State = GameState.GameWon;

        await UniTask.Delay(TimeSpan.FromSeconds(_config.EndGameDelay));
        ReturnToMenu();
    }

    private int GetSafeAmount()
    {
        //NOTE: readability over memory — LINQ для поиска максимального индекса
        int bestIndex = _config.SafeAmountIndices
            .Where(i => _currentQuestionNumber > i)
            .DefaultIfEmpty(-1)
            .Max();

        return bestIndex >= 0 ? _config.PrizeAmounts[bestIndex] : 0;
    }

    private void HandleHintClicked(HintType type)
    {
        if (State != GameState.QuestionActive) return;

        _audioManager.PlayPressButtonHint();
        switch (type)
        {
            case HintType.FiftyFifty when !_fiftyFiftyUsed:
                UseFiftyFifty();
                break;
            case HintType.AudienceHelp when !_audienceHelpUsed:
                UseAudienceHelp();
                break;
            case HintType.PhoneFriend when !_phoneFriendUsed:
                UsePhoneFriend();
                break;
            case HintType.ReplaceQuestion when !_replaceQuestionUsed:
                _rewardedAD.RewardedAdvShow(UseReplaceQuestion);
                break;
        }
    }

    private void UseFiftyFifty()
    {
        _fiftyFiftyUsed = true;

        var wrong = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            if (i != _currentQuestion.CorrectIndex)
                wrong.Add(i);
        }

        for (int i = wrong.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (wrong[i], wrong[j]) = (wrong[j], wrong[i]);
        }

        _gameUI.HideAnswerButton(wrong[0]);
        _gameUI.HideAnswerButton(wrong[1]);
        _activeAnswers[wrong[0]] = false;
        _activeAnswers[wrong[1]] = false;

        UpdateHintButtons();
        SaveProgress();
    }

    private void UseAudienceHelp()
    {
        _audienceHelpUsed = true;

        //NOTE: readability over memory — генерация случайных значений для слайдеров
        var vals = new int[4];
        int sum = 0;

        for (int i = 0; i < 4; i++)
        {
            if (i == _currentQuestion.CorrectIndex) continue;
            
            vals[i] = Random.Range(3, 20);
            
            if (!_activeAnswers[i])
            {
                vals[_currentQuestion.CorrectIndex] += vals[i];
                vals[i] = 0;
            }
            sum += vals[i];
        }

        vals[_currentQuestion.CorrectIndex] += (100 - vals[_currentQuestion.CorrectIndex]) - sum;

        _hintPopupUI.ShowAudienceHelp(vals);
        UpdateHintButtons();
        SaveProgress();
    }

    private void UsePhoneFriend()
    {
        _phoneFriendUsed = true;

        string[] letters = { "A", "B", "C", "D" };
        string hex = ColorUtility.ToHtmlStringRGB(_config.SelectedColor);
        string correctLetter = $"<color=#{hex}>{letters[_currentQuestion.CorrectIndex]}</color>";
        string[] phrases =
        {
            $"Я думаю, правильный ответ — {correctLetter}.",
            $"Однозначно {correctLetter}, я уверен.",
            $"Не сомневайся, ответ {correctLetter}!",
            $"Хм... Я бы выбрал {correctLetter}.",
            $"Уверен на 100%, это ответ {correctLetter}!"
        };

        string phrase = phrases[Random.Range(0, phrases.Length)];
        _hintPopupUI.ShowPhoneFriend(phrase);
        UpdateHintButtons();
        SaveProgress();
    }

    private void UseReplaceQuestion()
    {
        _replaceQuestionUsed = true;

        string category = GetCategoryForQuestion(_currentQuestionNumber);
        var availableIds = _questionBank.GetQuestionIds(category)
            .Where(id => !_usedQuestionIds.Contains(id))
            .ToList();

        if (availableIds.Count == 0)
        {
            _usedQuestionIds.Clear();
            availableIds = _questionBank.GetQuestionIds(category).ToList();
        }

        int questionId = availableIds[Random.Range(0, availableIds.Count)];
        _usedQuestionIds.Add(questionId);

        var content = _questionBank.GetQuestion(category, questionId, _currentLanguage);
        if (content == null) return;

        _currentQuestion = new CurrentQuestion(content);
        _currentQuestionId = questionId;
        _currentQuestion.EnsureCorrectInFirst(3);
        ResetActiveAnswers(3);
        _gameUI.SetQuestionText(_currentQuestion.QuestionText);
        _gameUI.SetAnswers(_currentQuestion.ShuffledAnswers[0], _currentQuestion.ShuffledAnswers[1], _currentQuestion.ShuffledAnswers[2]);
        _gameUI.ShowAnswerButtons(3);
        _gameUI.ResetAnswerHighlights();
        UpdateHintButtons();
        SaveProgress();
    }

    private void ReturnToMenu()
    {
        _gameUI.HideQuestionPanel();
        _menuUI.ShowPlayButton();
        State = GameState.Idle;
    }

    private void HandlePopupClosed() { }

    private void ResetActiveAnswers(int count)
    {
        for (int i = 0; i < 4; i++)
            _activeAnswers[i] = i < count;
    }

    private void ResetHints()
    {
        _fiftyFiftyUsed = false;
        _audienceHelpUsed = false;
        _phoneFriendUsed = false;
        _replaceQuestionUsed = false;
    }

    private void UpdateHintButtons()
    {
        _gameUI.SetHintActive(HintType.FiftyFifty, !_fiftyFiftyUsed);
        _gameUI.SetHintActive(HintType.AudienceHelp, !_audienceHelpUsed);
        _gameUI.SetHintActive(HintType.PhoneFriend, !_phoneFriendUsed);
        _gameUI.SetHintActive(HintType.ReplaceQuestion, !_replaceQuestionUsed);
    }

    private void SaveProgress()
    {
        _saveService.Data.currentQuestionNumber = _currentQuestionNumber;
        _saveService.Data.currentQuestionId = _currentQuestionId;
        _saveService.Data.usedQuestionIds = _usedQuestionIds.ToArray();
        _saveService.Data.usedFiftyFifty = _fiftyFiftyUsed;
        _saveService.Data.usedAudienceHelp = _audienceHelpUsed;
        _saveService.Data.usedPhoneFriend = _phoneFriendUsed;
        _saveService.Data.usedReplaceQuestion = _replaceQuestionUsed;
        _saveService.Data.hasActiveGame = State == GameState.QuestionActive || State == GameState.AnswerLocked;
        _saveService.Data.shuffleMap = _currentQuestion?.ShuffleMap ?? new int[0];
        _saveService.Data.activeAnswers = (bool[])_activeAnswers.Clone();
        _saveService.Save();
    }

}
