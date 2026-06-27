using System.Collections.Generic;

public class QuestionsStorage
{
    private readonly Dictionary<string, Dictionary<int, QuestionData>> _questionsByCategory;

    public QuestionsStorage() => _questionsByCategory = new Dictionary<string, Dictionary<int, QuestionData>>();

    public void AddQuestion(string category, int id, QuestionData questionData)
    {
        if (!_questionsByCategory.ContainsKey(category))
        {
            _questionsByCategory[category] = new Dictionary<int, QuestionData>();
        }

        _questionsByCategory[category][id] = questionData;
    }
    
    public Dictionary<string, Dictionary<int, QuestionData>> GetData() => _questionsByCategory;
}