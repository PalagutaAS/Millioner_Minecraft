using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Реализация сервиса получения вопроса по ID и языковому коду
/// </summary>
public class QuestionByIdRetrievalService : IQuestionByIdRetrievalService
{
    public LocalizedContent GetQuestionById(
        Dictionary<string, Dictionary<int, QuestionData>> questionsByCategory,
        int id,
        string languageCode
    )
    {
        if (questionsByCategory == null)
        {
            Debug.LogError("Данные ещё не загружены.");
            return null;
        }
        
        foreach (var categoryPair in questionsByCategory)
        {
            var categoryDict = categoryPair.Value;
            if (categoryDict.TryGetValue(id, out var questionData))
            {
                if (questionData.localizedData.TryGetValue(languageCode, out var content))
                {
                    return content;
                }
            }
        }

        return null;
    }
}