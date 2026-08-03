using System.Collections.Generic;

public class GameStateData
{
    public string CurrentLanguage;
    public int CurrentQuestionNumber;
    public CurrentQuestion CurrentQuestion;
    public readonly List<int> UsedQuestionIds = new();
    public readonly bool[] ActiveAnswers = new bool[4];
    public bool FiftyFiftyUsed;
    public bool AudienceHelpUsed;
    public bool PhoneFriendUsed;
    public bool ReplaceQuestionUsed;
    public int SelectedAnswerIndex;

    public void Reset()
    {
        UsedQuestionIds.Clear();
        FiftyFiftyUsed = false;
        AudienceHelpUsed = false;
        PhoneFriendUsed = false;
        ReplaceQuestionUsed = false;
    }

    public void ResetCurrent()
    {
        CurrentQuestionNumber = 0;
    }
}
