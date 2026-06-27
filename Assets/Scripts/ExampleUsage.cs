using UnityEngine;
using VContainer;

public class ExampleUsage : MonoBehaviour
{
    private IQuestionBankHolder _questionBankHolder;
    private QuestionBank _questionBank;

    [Inject]
    public void Construct(IQuestionBankHolder questionBankHolder)
    {
        _questionBankHolder = questionBankHolder;
    }

    private void Awake()
    {
        _questionBank = _questionBankHolder.CurrentBank;
    }

    private void Start()
    {
        _questionBank = _questionBankHolder.CurrentBank;
    }
}