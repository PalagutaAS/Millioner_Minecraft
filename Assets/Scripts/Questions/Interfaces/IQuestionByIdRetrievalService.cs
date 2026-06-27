using System.Collections.Generic;

/// <summary>
/// Интерфейс сервиса получения вопроса по ID и языковому коду
/// </summary>
public interface IQuestionByIdRetrievalService
{
    LocalizedContent GetQuestionById(
        Dictionary<string, Dictionary<int, QuestionData>> questionsByCategory,
        int id,
        string languageCode
    );
}