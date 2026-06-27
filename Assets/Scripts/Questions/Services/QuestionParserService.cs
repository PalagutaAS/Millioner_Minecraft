using Newtonsoft.Json.Linq;

/// <summary>
/// Реализация сервиса парсинга JSON данных вопросов
/// </summary>
public class QuestionParserService : IQuestionParserService
{
    public QuestionsStorage ParseJson(string jsonText)
    {
        JObject root = JObject.Parse(jsonText);
        var storage = new QuestionsStorage();

        foreach (var categoryProperty in root.Properties())
        {
            string category = categoryProperty.Name;
            JArray questionsArray = (JArray)categoryProperty.Value;

            foreach (JObject questionObj in questionsArray)
            {
                var questionData = new QuestionData();
                questionData.id = questionObj["id"].Value<int>();

                foreach (var property in questionObj.Properties())
                {
                    if (property.Name == "id") continue;

                    string languageCode = property.Name;
                    JObject langObj = (JObject)property.Value;
                    LocalizedContent content = langObj.ToObject<LocalizedContent>();
                    questionData.localizedData[languageCode] = content;
                }

                storage.AddQuestion(category, questionData.id, questionData);
            }
        }

        return storage;
    }
}