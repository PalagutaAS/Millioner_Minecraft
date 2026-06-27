using System.Collections.Generic;

/// <summary>
/// Интерфейс сервиса получения конкретного вопроса по ID и языку
/// </summary>
public interface IQuestionRetrievalService
{
    LocalizedContent GetQuestion(
        Dictionary<string, Dictionary<int, QuestionData>> questionsByCategory,
        string category,
        int id,
        string languageCode
    );
}