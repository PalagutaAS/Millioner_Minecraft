using Cysharp.Threading.Tasks;

/// <summary>
/// Интерфейс сервиса загрузки данных вопросов
/// </summary>
public interface IQuestionLoaderService
{
    UniTask<string> LoadQuestionsAsync(string key);
}