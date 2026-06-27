using System.Collections.Generic;
using UnityEngine;

public class CurrentQuestion
{
    public string QuestionText { get; }
    public string[] ShuffledAnswers { get; }
    public int CorrectIndex { get; private set; }
    public int[] ShuffleMap { get; }

    public CurrentQuestion(LocalizedContent content)
    {
        QuestionText = content.question;

        var answers = new List<string>(content.answers);
        CorrectIndex = 0;
        ShuffleMap = new int[answers.Count];
        for (int i = 0; i < ShuffleMap.Length; i++)
            ShuffleMap[i] = i;

        //NOTE: readability over memory — Fisher-Yates shuffle
        for (int i = answers.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (answers[i], answers[j]) = (answers[j], answers[i]);
            (ShuffleMap[i], ShuffleMap[j]) = (ShuffleMap[j], ShuffleMap[i]);

            if (j == CorrectIndex)
                CorrectIndex = i;
            else if (i == CorrectIndex)
                CorrectIndex = j;
        }

        ShuffledAnswers = answers.ToArray();
    }

    public CurrentQuestion(LocalizedContent content, int[] shuffleMap)
    {
        QuestionText = content.question;
        ShuffleMap = shuffleMap;

        var answers = new List<string>(content.answers);
        var shuffled = new string[answers.Count];

        for (int i = 0; i < shuffleMap.Length; i++)
            shuffled[i] = answers[shuffleMap[i]];

        ShuffledAnswers = shuffled;

        for (int i = 0; i < shuffleMap.Length; i++)
        {
            if (shuffleMap[i] == 0)
            {
                CorrectIndex = i;
                break;
            }
        }
    }

    public bool IsCorrect(int index) => index == CorrectIndex;

    public void EnsureCorrectInFirst(int count)
    {
        if (CorrectIndex < count) return;

        int swapTarget = Random.Range(0, count);
        (ShuffledAnswers[CorrectIndex], ShuffledAnswers[swapTarget]) =
            (ShuffledAnswers[swapTarget], ShuffledAnswers[CorrectIndex]);
        (ShuffleMap[CorrectIndex], ShuffleMap[swapTarget]) =
            (ShuffleMap[swapTarget], ShuffleMap[CorrectIndex]);
        CorrectIndex = swapTarget;
    }
}
