using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProblemManager : MonoBehaviour
{
	public TextMeshProUGUI problemText;
	public TextMeshProUGUI feedBackText;
	public TMP_InputField answerField;

	Problem currentProblem;
	List<Problem> problems;
	int currentMaxNumber = 20;

    // Start is called before the first frame update
    void Start()
    {
		problems = new List<Problem>();
		NextProblem();
	}

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.KeypadEnter))
		{
			Submit();
		}
    }

	public void Submit()
	{
		if (currentProblem.SubmitAnswer(int.Parse(answerField.text)))
		{
			feedBackText.text = "Good Job!";
			problems.Add(currentProblem);
		}
		else
		{
			feedBackText.text = "Try again";
			answerField.text = "";
		}
	}

	public void NextProblem()
	{
		currentProblem = new Problem(Problem.ProblemType.Plus, currentMaxNumber);
		problemText.text = currentProblem.ProblemText;
		answerField.text = "";
		feedBackText.text = "";

	}

	public void TypeNumber(int number)
	{
		answerField.text += number;
	}

}

public class Problem
{
	int number1;
	int number2;
	int answer;
	public enum ProblemType
	{
		Plus, Minus
	}
	ProblemType problemType;

	string problemText;

	float startTime;
	float endTime;
	float executionTime;
	int triesToComplete;
	bool passed;

	public string ProblemText { get => problemText; private set => problemText = value; }

	public Problem(ProblemType problemType, int MaxNumber)
	{
		startTime = Time.realtimeSinceStartup;

		switch (problemType)
		{
			case ProblemType.Plus:
				answer = Random.Range(1, MaxNumber);
				number1 = Random.Range(0, answer);
				number2 = answer - number1;
				ProblemText = string.Format("{0} + {1} = ", number1, number2);
				break;
			case ProblemType.Minus:
				number1 = Random.Range(1, MaxNumber);
				number2 = Random.Range(0, number1);
				answer = number1 - number2;
				ProblemText = string.Format("{0} - {1} = ", number1, number2);
				break;
			default:
				break;
		}

	}
	

	public bool SubmitAnswer(int attempt)
	{
		if (attempt == answer)
		{
			endTime = Time.realtimeSinceStartup;
			executionTime = endTime - startTime;

			return true;
		}
		else
		{
			return false;
		}
	}

}
