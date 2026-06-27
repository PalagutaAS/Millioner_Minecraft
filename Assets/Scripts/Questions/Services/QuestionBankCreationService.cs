using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Реализация сервиса создания банка вопросов
/// </summary>
public class QuestionBankCreationService : IQuestionBankCreationService
{
    private readonly IQuestionLoaderService _loaderService;
    private readonly IQuestionParserService _parserService;
    private readonly IQuestionCategoryService _categoryService;
    private readonly IQuestionRetrievalService _retrievalService;
    private readonly IQuestionByIdRetrievalService _byIdRetrievalService;
    private readonly IQuestionBankHolder _bankHolder;

    public QuestionBankCreationService(
        IQuestionLoaderService loaderService,
        IQuestionParserService parserService,
        IQuestionCategoryService categoryService,
        IQuestionRetrievalService retrievalService,
        IQuestionByIdRetrievalService byIdRetrievalService,
        IQuestionBankHolder bankHolder)
    {
        _loaderService = loaderService;
        _parserService = parserService;
        _categoryService = categoryService;
        _retrievalService = retrievalService;
        _byIdRetrievalService = byIdRetrievalService;
        _bankHolder = bankHolder;
    }

    public async UniTask<QuestionBank> CreateQuestionBankAsync()
    {
        string jsonText = await _loaderService.LoadQuestionsAsync("questions");
        if (jsonText != null)
        {
            _bankHolder.SetBank(new QuestionBank(_parserService.ParseJson(jsonText),
                _categoryService,
                _retrievalService,
                _byIdRetrievalService));
        }

        return _bankHolder.CurrentBank;
    }
}