/// <summary>
/// Интерфейс сервиса парсинга JSON данных вопросов
/// </summary>
public interface IQuestionParserService
{
    QuestionsStorage ParseJson(string jsonText);
}