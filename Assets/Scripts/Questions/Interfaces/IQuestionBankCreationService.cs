using Cysharp.Threading.Tasks;

/// <summary>
/// Интерфейс сервиса создания банка вопросов
/// </summary>
public interface IQuestionBankCreationService
{
    UniTask<QuestionBank> CreateQuestionBankAsync();
}