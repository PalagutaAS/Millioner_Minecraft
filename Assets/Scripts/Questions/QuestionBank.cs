using System.Collections.Generic;

public interface IQuestionBank
{
    LocalizedContent GetQuestion(string category, int id, string languageCode);
    LocalizedContent GetQuestionById(int id, string languageCode);
    IEnumerable<string> GetCategories();
    IEnumerable<int> GetQuestionIds(string category);
}

public class QuestionBank : IQuestionBank
{
    private readonly QuestionsStorage _questionsStorage;

    private readonly IQuestionCategoryService _categoryService;
    private readonly IQuestionRetrievalService _retrievalService;
    private readonly IQuestionByIdRetrievalService _byIdRetrievalService;

    public QuestionBank(QuestionsStorage questionsStorage,
        IQuestionCategoryService categoryService,
        IQuestionRetrievalService retrievalService,
        IQuestionByIdRetrievalService byIdRetrievalService)
    {
        _questionsStorage = questionsStorage;
        _categoryService = categoryService;
        _retrievalService = retrievalService;
        _byIdRetrievalService = byIdRetrievalService;
    }

    public LocalizedContent GetQuestion(string category, int id, string languageCode)
    {
        return _retrievalService.GetQuestion(_questionsStorage?.GetData(), category, id, languageCode);
    }

    public LocalizedContent GetQuestionById(int id, string languageCode)
    {
        return _byIdRetrievalService.GetQuestionById(_questionsStorage?.GetData(), id, languageCode);
    }

    public IEnumerable<string> GetCategories() => _categoryService.GetCategories(_questionsStorage?.GetData());

    public IEnumerable<int> GetQuestionIds(string category) =>
        _categoryService.GetQuestionIds(_questionsStorage?.GetData(), category);
}
