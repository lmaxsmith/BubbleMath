using System;
using UnityEngine;
using Random = System.Random;

[Serializable] public class ProblemData
{
    [SerializeField] int number1;
    [SerializeField] int number2;
    [SerializeField] int answer;
    public enum ProblemType
    {
        Plus, Minus, Times, DividedBy
    }
    [SerializeField] ProblemType problemType;

    //to display to the player
    [SerializeField] string problemText;

    //Tracks execution by the player. Let's us know which problems are easier/harder,
    //So we can understand player progress and adjust difficulty over time. 
    [SerializeField] float startTime;
    [SerializeField] float endTime;
    [SerializeField] float executionTime;
    [SerializeField] int triesToComplete = 1;
    [SerializeField] bool passed;

    public string ProblemText { get => problemText; }
    public int TriesToComplete { get => triesToComplete; }
    public float ExecutionTime { get => executionTime; }
    public bool Passed { get => passed; }
    public int DigitsInAnswer { get => digitsInAnswer; }

    int digitsInAnswer;

    /// <summary>
    /// create a new math problem at creation of object
    /// </summary>
    /// <param name="problemType"></param>
    /// <param name="MaxNumber"></param>
    public ProblemData(ProblemType problemType, int MaxNumber)
    {
        startTime = Time.realtimeSinceStartup;
        Random random = new Random();
        switch (problemType)
        {
            case ProblemType.Plus:
                answer = UnityEngine.Random.Range(MaxNumber / 4, MaxNumber);
                number1 = UnityEngine.Random.Range(1, answer);
                number2 = answer - number1;
                problemText = string.Format("{0} + {1} = ", number1, number2);
                break;
            case ProblemType.Minus:
                number1 = UnityEngine.Random.Range(MaxNumber / 4, MaxNumber * 3/4);
                number2 = UnityEngine.Random.Range(1, number1);
                answer = number1 - number2;
                problemText = string.Format("{0} - {1} = ", number1, number2);
                break;
            case ProblemType.Times:
                //control for weird breaking case
                if (MaxNumber < 10)
                    MaxNumber = 10;
                MaxNumber = Mathf.CeilToInt(MaxNumber * .75f);
                number1 = random.Next(2, MaxNumber / 2);
                number2 = random.Next(2, MaxNumber / number1);
                answer = number1 * number2;
                problemText = string.Format("{0} * {1} = ", number1, number2);
                break;
            case ProblemType.DividedBy:
                if (MaxNumber < 16)
                    MaxNumber = 16;
                MaxNumber = MaxNumber / 2;
                answer = random.Next(2, Mathf.CeilToInt(Mathf.Sqrt(MaxNumber)));
                number2 = random.Next(2, Mathf.CeilToInt(Mathf.Sqrt(MaxNumber)));
                number1 = answer * number2;
                problemText = string.Format("{0} / {1} = ", number1, number2);
                break;
            default:
                break;
        }

        digitsInAnswer = countDigits(answer);


    }

    /// <summary>
    /// Empty ctor for deserialization
    /// </summary>
    public ProblemData()
    {
		
    }

    public int countDigits(int number)
    {
        return number.ToString().Length;
    }

    public bool SubmitAnswer(int attempt)
    {

        if (attempt == answer)
        {
            endTime = Time.realtimeSinceStartup;
            executionTime = endTime - startTime;
            passed = true;
            return true;
        }
        else
        {
            triesToComplete++;
            return false;
        }
    }

}