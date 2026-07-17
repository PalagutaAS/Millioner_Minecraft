using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/GameConfig")]
public class GameConfig : ScriptableObject
{
    [Header("Prize per Question")]
    [SerializeField] private int[] _prizeAmounts = new[]
    {
        500, 1000, 2000, 3000, 5000,
        10000, 15000, 25000, 50000, 100000,
        150000, 250000, 500000, 750000, 1000000,
        1250000, 1500000, 2000000, 3000000, 5000000
    };

    [Header("Safe Amounts")]
    [SerializeField] private int[] _safeAmountIndices = new[] { 4, 9, 14 };

    [Header("Timings")]
    [SerializeField] private float _introDuration = 2.5f;
    [SerializeField] private float _answerRevealDelay = 2.5f;
    [SerializeField] private float _resultDisplayDuration = 2f;
    [SerializeField] private float _endGameDelay = 3f;

    [Header("Colors")]
    [SerializeField] private Color _defaultAnswerColor = new(0.15f, 0.15f, 0.15f);
    [SerializeField] private Color _highlightColor = new(0.8f, 0.8f, 0.8f);
    [SerializeField] private Color _selectedColor = new(0.9f, 0.7f, 0.1f);
    [SerializeField] private Color _textColor = new(0.8f, 0.8f, 0.8f);
    
    [Header("Sprites")]
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _correctSprite;
    [SerializeField] private Sprite _selectedSprite;
    
    [Header("Save Keys")]
    [SerializeField] private string _saveKey = "game_save";
    [SerializeField] private string _languageKey = "game_language";
    [SerializeField] private string _volumeKey = "game_volume";

    [Header("Language")]
    [SerializeField] private string _defaultLanguage = "ru";
    [SerializeField] private string[] _availableLanguages = { "ru", "en" };
    
    [Header("YG")]
    [SerializeField]private string _rewardAd;

    public int QuestionsToWin => _prizeAmounts.Length;
    public int[] PrizeAmounts => _prizeAmounts;

    public int[] SafeAmountIndices => _safeAmountIndices;

    public float IntroDuration => _introDuration;
    public float AnswerRevealDelay => _answerRevealDelay;
    public float ResultDisplayDuration => _resultDisplayDuration;
    public float EndGameDelay => _endGameDelay;

    public Color DefaultAnswerColor => _defaultAnswerColor;
    public Color HighlightColor => _highlightColor;
    public Color SelectedColor => _selectedColor;
    public Color TextColor => _textColor;

    public Sprite DefaultSprite => _defaultSprite;
    public Sprite CorrectSprite => _correctSprite;
    public Sprite SelectedSprite => _selectedSprite;

    public string SaveKey => _saveKey;
    public string LanguageKey => _languageKey;
    public string VolumeKey => _volumeKey;

    public string DefaultLanguage => _defaultLanguage;
    public string[] AvailableLanguages => _availableLanguages;
    public string RewardID => _rewardAd;
}
