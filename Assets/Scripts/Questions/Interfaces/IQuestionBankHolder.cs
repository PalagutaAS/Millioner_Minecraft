using System.Collections.Generic;

public interface IQuestionBankHolder
{
    QuestionBank CurrentBank { get; }
    void SetBank(QuestionBank bank);
}

public class QuestionBankHolder : IQuestionBankHolder
{
    public QuestionBank CurrentBank { get; private set; }

    public void SetBank(QuestionBank bank)
    {
        CurrentBank = bank;
    }
}
