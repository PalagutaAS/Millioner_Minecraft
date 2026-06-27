using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Реализация сервиса получения конкретного вопроса по ID и языку
/// </summary>
public class QuestionRetrievalService : IQuestionRetrievalService
{
    public LocalizedContent GetQuestion(
        Dictionary<string, Dictionary<int, QuestionData>> questionsByCategory,
        string category,
        int id,
        string languageCode
    )
    {
        if (questionsByCategory == null)
        {
            Debug.LogError("Данные ещё не загружены.");
            return null;
        }

        if (!questionsByCategory.TryGetValue(category, out var categoryDict))
            return null;
        if (!categoryDict.TryGetValue(id, out var questionData))
            return null;
        if (!questionData.localizedData.TryGetValue(languageCode, out var content))
            return null;

        return content;
    }
}