using System.Collections.Generic;

[System.Serializable]
public class LocalizedContent
{
    public string question;
    public List<string> answers;
}

[System.Serializable]
public class QuestionData
{
    public int id;
    public Dictionary<string, LocalizedContent> localizedData = new ();
}