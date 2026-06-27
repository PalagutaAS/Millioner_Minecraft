using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Реализация сервиса работы с категориями вопросов
/// </summary>
public class QuestionCategoryService : IQuestionCategoryService
{
    public IEnumerable<string> GetCategories(Dictionary<string, Dictionary<int, QuestionData>> questionsByCategory)
    {
        return questionsByCategory?.Keys ?? Enumerable.Empty<string>();
    }

    public IEnumerable<int> GetQuestionIds(Dictionary<string, Dictionary<int, QuestionData>> questionsByCategory, string category)
    {
        if (questionsByCategory != null && questionsByCategory.TryGetValue(category, out var dict))
        {
            return dict.Keys;
        }
        
        return new List<int>();
    }
}