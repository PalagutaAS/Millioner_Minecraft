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
    public int[] usedQuestionIds = new int[0];
    public int[] shuffleMap = new int[0];
    public bool[] activeAnswers = new bool[0];
}

public class SaveService
{
    private readonly GameConfig _config;

    public SaveData Data { get; private set; } = new();

    public SaveService(GameConfig config)
    {
        _config = config;
    }

    public void Load()
    {
        //string json = PlayerPrefs.GetString(_config.SaveKey, "");
        string json = YG2.saves.progress;
        Data = string.IsNullOrEmpty(json) ? new SaveData() : JsonUtility.FromJson<SaveData>(json);
    }

    public void Save()
    {
        //PlayerPrefs.SetString(_config.SaveKey, JsonUtility.ToJson(Data));
        //PlayerPrefs.Save();
        YG2.saves.progress = JsonUtility.ToJson(Data);
        YG2.SaveProgress();
    }

    public void DeleteSave()
    {
        Data = new SaveData();
        //PlayerPrefs.DeleteKey(_config.SaveKey);
        //PlayerPrefs.Save();
        YG2.saves.progress = JsonUtility.ToJson(Data);
        YG2.SaveProgress();
    }

    public void SaveLanguage(string lang)
    {
        PlayerPrefs.SetString(_config.LanguageKey, lang);
        PlayerPrefs.Save();
    }

    public string LoadLanguage() => PlayerPrefs.GetString(_config.LanguageKey, _config.DefaultLanguage);

    public void SaveVolume(float vol)
    {
        PlayerPrefs.SetFloat(_config.VolumeKey, vol);
        PlayerPrefs.Save();
    }

    public float LoadVolume() => PlayerPrefs.GetFloat(_config.VolumeKey, 1f);
}
