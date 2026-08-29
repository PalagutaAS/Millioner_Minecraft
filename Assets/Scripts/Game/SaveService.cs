using UnityEngine;
using YG;

[System.Serializable]
public class SaveData
{
    public int wallet;
    public int currentQuestionNumber;
    public int currentQuestionId;
    public bool usedFiftyFifty;
    public bool usedAudienceHelp;
    public bool usedPhoneFriend;
    public bool usedReplaceQuestion;
    public bool hasActiveGame;
    public string currentLang = "en";
    public int[] usedQuestionIds = new int[0];
    public int[] shuffleMap = new int[0];
    public bool[] activeAnswers = new bool[0];

    public SaveData(string language)
    {
        switch (language)
        {
            case "en":
            case "ru":
                currentLang = language;
                break;
            default:
                currentLang = "en";
                break;
        }
    }
}

public class SaveService
{
    private readonly GameConfig _config;

    public SaveData Data { get; private set; }

    public SaveService(GameConfig config)
    {
        _config = config;
    }

    public void Load()
    {
        string json = YG2.saves.progress;
        Data = string.IsNullOrEmpty(json) ? new SaveData(YG2.envir.language) : JsonUtility.FromJson<SaveData>(json);
#if UNITY_WEBGL
        Data.currentLang = YG2.lang;
#elif UNITY_ANDROID
        
#endif
    }

    public void Save()
    {
        YG2.saves.progress = JsonUtility.ToJson(Data);
        YG2.SaveProgress();
    }

    public void SaveLeaderboard()
    {
        YG2.SetLeaderboard("amountWon", Data.wallet);
    }

    public void DeleteSave()
    {
        Data = new SaveData(YG2.envir.language);
        Save();
    }

    public void SwitchLanguage()
    {
        YG2.SwitchLanguage(YG2.lang == "ru" ? "en" : "ru");
        Data.currentLang = YG2.lang;
    }
}
