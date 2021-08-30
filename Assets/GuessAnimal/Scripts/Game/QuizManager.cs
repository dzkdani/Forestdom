using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizManager : MonoBehaviour
{
    private static QuizManager _instance;
    public static QuizManager instance { get { return _instance; } }
    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
    }

    [Header("Quiz Components")]
    [SerializeField] int answer;
    public QuestionList questions;

    int randIdx()
    {
        return Random.RandomRange(0, questions.questionList.Count);
    }

    public Questions GetQuestion()
    {
        answer = randIdx();
        if (GameModeManager.instance.ActiveGameMode() == GameMode.Audio)
        {
            if (questions.questionList[answer].haveSound)
            {
                return questions.questionList[answer];
            }
            else
            {
                GetQuestion();
            }
        }
        else
        {
            return questions.questionList[answer];
        }

        return questions.questionList[answer];
    }
    
    int prevOptsIdx;
    public Questions GetOptions()
    {
        int falseOptsIdx = randIdx();
        if (falseOptsIdx != answer)
        {
            if (prevOptsIdx == falseOptsIdx)
            {
                GetOptions();
            }
            else
            {
                return questions.questionList[falseOptsIdx];
            }
        }
        else
        {
            GetOptions();
        }

        prevOptsIdx = falseOptsIdx;
        return GetOptions();
    }

    public bool CheckAnswer(int idx)
    {
        return idx == questions.questionList[answer].animalId;
    }
}
