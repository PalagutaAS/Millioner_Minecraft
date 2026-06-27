using System.Collections.Generic;

/// <summary>
/// Интерфейс сервиса работы с категориями вопросов
/// </summary>
public interface IQuestionCategoryService
{
    IEnumerable<string> GetCategories(Dictionary<string, Dictionary<int, QuestionData>> questionsByCategory);
    IEnumerable<int> GetQuestionIds(Dictionary<string, Dictionary<int, QuestionData>> questionsByCategory, string category);
}