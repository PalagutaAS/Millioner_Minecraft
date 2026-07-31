using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YG;
using Random = UnityEngine.Random;

public class QuestionActiveState : IGameState
{
    public GameState StateId => GameState.QuestionActive;

    private readonly GameUI _gameUI;
    private readonly AudioManager _audioManager;
    private readonly GameConfig _config;
    private readonly SaveService _saveService;
    private readonly PrizeLadderService _prizeLadder;
    private readonly HintPopupUI _hintPopupUI;
    private readonly RewardedAD _rewardedAD;
    private readonly QuestionBank _questionBank;
    private readonly GameStateData _data;
    private readonly GameStateMachine _machine;

    public QuestionActiveState(
        GameUI gameUI,
        AudioManager audioManager,
        GameConfig config,
        SaveService saveService,
        PrizeLadderService prizeLadder,
        HintPopupUI hintPopupUI,
        RewardedAD rewardedAD,
        QuestionBank questionBank,
        GameStateData data,
        GameStateMachine machine)
    {
        _gameUI = gameUI;
        _audioManager = audioManager;
        _config = config;
        _saveService = saveService;
        _prizeLadder = prizeLadder;
        _hintPopupUI = hintPopupUI;
        _rewardedAD = rewardedAD;
        _questionBank = questionBank;
        _data = data;
        _machine = machine;
    }

    public UniTask Enter()
    {
        ShowNextQuestion();
        _gameUI.ShowQuestionPanel();
        _gameUI.ResetAnswerHighlights();
        _gameUI.SetAnswersInteractable(true);
        _prizeLadder.SetHighlightedRow(_data.CurrentQuestionNumber);
        UpdateHintButtons();
        SaveProgress();

        _gameUI.OnAnswerClicked += HandleAnswerClicked;
        _gameUI.OnHintClicked += HandleHintClicked;

        return UniTask.CompletedTask;
    }

    public UniTask Exit()
    {
        _gameUI.OnAnswerClicked -= HandleAnswerClicked;
        _gameUI.OnHintClicked -= HandleHintClicked;
        return UniTask.CompletedTask;
    }

    private void HandleAnswerClicked(int index)
    {
        if (!_data.ActiveAnswers[index]) return;

        _data.SelectedAnswerIndex = index;
        _machine.TransitionTo(GameState.AnswerLocked).Forget();
    }

    private void HandleHintClicked(HintType type)
    {
        _audioManager.PlayPressButtonHint();
        switch (type)
        {
            case HintType.FiftyFifty when !_data.FiftyFiftyUsed:
                UseFiftyFifty();
                break;
            case HintType.AudienceHelp when !_data.AudienceHelpUsed:
                UseAudienceHelp();
                break;
            case HintType.PhoneFriend when !_data.PhoneFriendUsed:
                UsePhoneFriend();
                break;
            case HintType.ReplaceQuestion when !_data.ReplaceQuestionUsed:
                _rewardedAD.RewardedAdvShow(UseReplaceQuestion);
                break;
        }
    }

    private void ShowNextQuestion()
    {
        int questionId = _saveService.Data.currentQuestionId;
        
        bool restored = (!_data.UsedQuestionIds.Contains(questionId) && _saveService.Data.hasActiveGame);
        
        LocalizedContent content;

        if (restored)
        {
            content = _questionBank.GetQuestionById(questionId, _data.CurrentLanguage);
            
            _data.CurrentQuestion = new CurrentQuestion(content, questionId, _saveService.Data.shuffleMap);
            
            _gameUI.ShowAnswerButtons(4);
            for (int i = 0; i < 4; i++)
            {
                _data.ActiveAnswers[i] = _saveService.Data.activeAnswers[i];
                if (!_data.ActiveAnswers[i])
                    _gameUI.HideAnswerButton(i);
            }
        }
        else
        {
            string category = GetCategoryForQuestion(_data.CurrentQuestionNumber);
            var availableIds = _questionBank.GetQuestionIds(category)
                .Where(id => !_data.UsedQuestionIds.Contains(id))
                .ToList();

            if (availableIds.Count == 0)
            {
                _data.UsedQuestionIds.Clear();
                availableIds = _questionBank.GetQuestionIds(category).ToList();
            }

            questionId = availableIds[Random.Range(0, availableIds.Count)];
            content = _questionBank.GetQuestion(category, questionId, _data.CurrentLanguage);
            _data.CurrentQuestion = new CurrentQuestion(content, questionId);
            ResetActiveAnswers(4);
            _gameUI.ShowAnswerButtons(4);
        }
        
        _gameUI.SetQuestionText(_data.CurrentQuestion.QuestionText);
        _gameUI.SetAnswers(_data.CurrentQuestion.ShuffledAnswers);
        _gameUI.ResetAnswerHighlights();
        _prizeLadder.SetHighlightedRow(_data.CurrentQuestionNumber);
    }

    private string GetCategoryForQuestion(int questionNumber)
    {
        if (questionNumber < 3) return "easy";
        if (questionNumber < 9) return "medium";
        if (questionNumber < 14) return "hard";
        return "very_hard";
    }

    private void UseFiftyFifty()
    {
        _data.FiftyFiftyUsed = true;

        var wrong = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            if (i != _data.CurrentQuestion.CorrectIndex)
                wrong.Add(i);
        }

        for (int i = wrong.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (wrong[i], wrong[j]) = (wrong[j], wrong[i]);
        }

        _gameUI.HideAnswerButton(wrong[0]);
        _gameUI.HideAnswerButton(wrong[1]);
        _data.ActiveAnswers[wrong[0]] = false;
        _data.ActiveAnswers[wrong[1]] = false;

        UpdateHintButtons();
        SaveProgress();
    }

    private void UseAudienceHelp()
    {
        _data.AudienceHelpUsed = true;

        //NOTE: readability over memory — генерация случайных значений для слайдеров
        var vals = new int[4];
        int sum = 0;

        for (int i = 0; i < 4; i++)
        {
            if (i == _data.CurrentQuestion.CorrectIndex) continue;

            vals[i] = Random.Range(3, 20);

            if (!_data.ActiveAnswers[i])
            {
                vals[_data.CurrentQuestion.CorrectIndex] += vals[i];
                vals[i] = 0;
            }
            sum += vals[i];
        }

        vals[_data.CurrentQuestion.CorrectIndex] += (100 - vals[_data.CurrentQuestion.CorrectIndex]) - sum;

        _hintPopupUI.ShowAudienceHelp(vals);
        UpdateHintButtons();
        SaveProgress();
    }

    private void UsePhoneFriend()
    {
        _data.PhoneFriendUsed = true;

        string[] letters = { "A", "B", "C", "D" };
        string hex = ColorUtility.ToHtmlStringRGB(_config.SelectedColor);
        string correctLetter = $"<color=#{hex}>{letters[_data.CurrentQuestion.CorrectIndex]}</color>";
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
        _data.ReplaceQuestionUsed = true;

        string category = GetCategoryForQuestion(_data.CurrentQuestionNumber);
        var availableIds = _questionBank.GetQuestionIds(category)
            .Where(id => !_data.UsedQuestionIds.Contains(id))
            .ToList();

        if (availableIds.Count == 0)
        {
            _data.UsedQuestionIds.Clear();
            availableIds = _questionBank.GetQuestionIds(category).ToList();
        }

        int newQuestionId = availableIds[Random.Range(0, availableIds.Count)];
        _data.UsedQuestionIds.Add(_data.CurrentQuestion.Id);

        var content = _questionBank.GetQuestion(category, newQuestionId, _data.CurrentLanguage);
        if (content == null) return;

        _data.CurrentQuestion = new CurrentQuestion(content, newQuestionId);
        _data.CurrentQuestion.EnsureCorrectInFirst(3);
        ResetActiveAnswers(3);
        _gameUI.SetQuestionText(_data.CurrentQuestion.QuestionText);
        _gameUI.SetAnswers(_data.CurrentQuestion.ShuffledAnswers[0], _data.CurrentQuestion.ShuffledAnswers[1], _data.CurrentQuestion.ShuffledAnswers[2]);
        _gameUI.ShowAnswerButtons(3);
        _gameUI.ResetAnswerHighlights();
        UpdateHintButtons();
        SaveProgress();
    }

    private void ResetActiveAnswers(int count)
    {
        for (int i = 0; i < 4; i++)
            _data.ActiveAnswers[i] = i < count;
    }

    private void UpdateHintButtons()
    {
        _gameUI.SetHintActive(HintType.FiftyFifty, !_data.FiftyFiftyUsed);
        _gameUI.SetHintActive(HintType.AudienceHelp, !_data.AudienceHelpUsed);
        _gameUI.SetHintActive(HintType.PhoneFriend, !_data.PhoneFriendUsed);
        _gameUI.SetHintActive(HintType.ReplaceQuestion, !_data.ReplaceQuestionUsed);
    }

    private void SaveProgress()
    {
        _saveService.Data.currentQuestionNumber = _data.CurrentQuestionNumber;
        _saveService.Data.usedQuestionIds = _data.UsedQuestionIds.ToArray();
        _saveService.Data.usedFiftyFifty = _data.FiftyFiftyUsed;
        _saveService.Data.currentQuestionId = _data.CurrentQuestion.Id;
        _saveService.Data.usedAudienceHelp = _data.AudienceHelpUsed;
        _saveService.Data.usedPhoneFriend = _data.PhoneFriendUsed;
        _saveService.Data.usedReplaceQuestion = _data.ReplaceQuestionUsed;
        _saveService.Data.hasActiveGame = true;
        _saveService.Data.shuffleMap = _data.CurrentQuestion?.ShuffleMap ?? new int[0];
        _saveService.Data.activeAnswers = (bool[])_data.ActiveAnswers.Clone();
        _saveService.Save();
    }
}
