using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

/* Problem manager class sits in the scene and creates new math problems, 
 * Running the player through the game session. 
 * 
 * Note 'Problem' support class (not monobehavior) at the bottom of this script.
 */

public class ProblemManager : MonoBehaviour
{
	public TextMeshProUGUI problemText;
	public TextMeshProUGUI feedBackText;

	public AnswerDigit answerDigitOnes;
	public AnswerDigit answerDigitTens;
	public AnswerDigit answerDigitHundreds;
	public List< AnswerDigit> answerDigits;
	AnswerDigit currentDigit;

	Problem currentProblem;
	List<Problem> problems;

	const int attemptsPerProblem = 3;
	const int problemsPerRound = 10;
	int problemsThisRound = 0;

	int currentMaxNumber = 20;

	#region Unity Methods
	// Start is called before the first frame update
	void Start()
    {
		answerDigits = new List<AnswerDigit>{ answerDigitHundreds, answerDigitTens, answerDigitOnes };
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
	#endregion


	//checks correctness of answer. Currently called by button
	//TODO: call automatically when answer has enough digits to be correct. 
	public void Submit()
	{
		int answerInt = 
			answerDigitHundreds.AnswerdigitInt * 100 + 
			answerDigitTens.AnswerdigitInt * 10 + 
			answerDigitOnes.AnswerdigitInt;

		if (currentProblem.SubmitAnswer(answerInt))
		{
			feedBackText.text = "Good Job!";
			StartCoroutine(WaitForNextProblem());
		}
		else
		{
			feedBackText.text = "Try again";

			StartCoroutine(WaitForNextAttempt());
		}

		currentDigit = null;
	}

	IEnumerator WaitForNextAttempt()
	{
		yield return new WaitForSeconds(2);

		if (currentProblem.TriesToComplete < attemptsPerProblem)
		{
			NextAttempt();
		}
		else if (problemsThisRound < problemsPerRound)
		{
			NextProblem();
		}
		else
		{
			EndRound();
		}

	}

	IEnumerator WaitForNextProblem()
	{
		yield return new WaitForSeconds(2);

		if (problemsThisRound < problemsPerRound)
		{
			NextProblem();
		}
		else
		{
			EndRound();
		}
	}

	//Clears the screen prepares for new attempt
	public void NextAttempt()
	{
		SetupDigits();
		feedBackText.text = "";

	}

	//currently called by a button, but will be situational
	public void NextProblem()
	{
		currentProblem = new Problem(Problem.ProblemType.Plus, currentMaxNumber);
		problems.Add(currentProblem);
		problemsThisRound++;


		problemText.text = string.Format(@"({0}): {1}", problemsThisRound, currentProblem.ProblemText);

		SetupDigits();
	}
	
	//turn on the correct digit and initialize digit use
	void SetupDigits()
	{
		ClearDigits();
		switch (currentProblem.DigitsInAnswer)
		{
			case 1:
				answerDigitOnes.gameObject.SetActive(true);
				answerDigitTens.gameObject.SetActive(false);
				answerDigitHundreds.gameObject.SetActive(false);
				SetCurrentDigit(answerDigitOnes);
				break;
			case 2:
				answerDigitOnes.gameObject.SetActive(true);
				answerDigitTens.gameObject.SetActive(true);
				answerDigitHundreds.gameObject.SetActive(false);
				SetCurrentDigit(answerDigitTens);
				break;
			case 3:
				answerDigitOnes.gameObject.SetActive(true);
				answerDigitTens.gameObject.SetActive(true);
				answerDigitHundreds.gameObject.SetActive(true);
				SetCurrentDigit(answerDigitHundreds);
				break;

			default:
				break;
		}
	}

	//reset answer board
	void ClearDigits()
	{
		foreach (var digit in FindObjectsOfType<AnswerDigit>())
		{
			digit.ClearField();
		}
		currentDigit = null;
	}
	void SetCurrentDigit(AnswerDigit answerDigit)
	{
		currentDigit = answerDigit;
		answerDigit.HilightField();
	}

	//closes the round and displays feedback
	public void EndRound()
	{
		ClearDigits();

		IEnumerable<Problem> correctProblems =
			from problem in problems
			where problem.Passed
			select problem;

		Debug.Log("Correct problems: " + correctProblems);

		float totalTime = 0;
		foreach(Problem problem in problems)
		{
			totalTime += problem.ExecutionTime;
		}
		Debug.Log("Total time: " + totalTime);

		float averageTime = totalTime / correctProblems.Count<Problem>();
		Debug.Log("Average time: " + averageTime);

		feedBackText.text = string.Format(@"Great job! You answered {0} math problems in {1} seconds! That's {2} seconds per math problem!", correctProblems.Count<Problem>(), totalTime, averageTime);

		foreach (var problem in problems)
		{
			Debug.Log(string.Format(@"Problem: {0}, execution time: {1}, attempts required: {2}, correct: {3}.", problem.ProblemText, problem.ExecutionTime, problem.TriesToComplete, problem.Passed));
		}
	}

	public void TypeNumber(int number)
	{
		if (currentDigit)
		{
			//first fill the digit
			currentDigit.FillField(number);

			//then moveon according to state
			if (currentDigit == answerDigitHundreds)
			{
				SetCurrentDigit(answerDigitTens);
			}
			else if (currentDigit == answerDigitTens)
			{
				SetCurrentDigit(answerDigitOnes);
			}
			else if (currentDigit == answerDigitOnes)
			{
				Submit();
			}
		}
	}

	

}




//OOP style. Containing one math problem. 
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

	//to display to the player
	string problemText;

	//Tracks execution by the player. Let's us know which problems are easier/harder,
	//So we can understand player progress and adjust difficulty over time. 
	float startTime;
	float endTime;
	float executionTime;
	int triesToComplete = 1;
	bool passed;

	public string ProblemText { get => problemText;}
	public int TriesToComplete { get => triesToComplete;}
	public float ExecutionTime { get => executionTime;}
	public bool Passed { get => passed;}
	public int DigitsInAnswer { get => digitsInAnswer;}

	int digitsInAnswer;

	//create a new math problem at creation of object
	public Problem(ProblemType problemType, int MaxNumber)
	{
		startTime = Time.realtimeSinceStartup;

		switch (problemType)
		{
			case ProblemType.Plus:
				answer = UnityEngine.Random.Range(1, MaxNumber);
				number1 = UnityEngine.Random.Range(0, answer);
				number2 = answer - number1;
				problemText = string.Format("{0} + {1} = ", number1, number2);
				break;
			case ProblemType.Minus:
				number1 = UnityEngine.Random.Range(1, MaxNumber);
				number2 = UnityEngine.Random.Range(0, number1);
				answer = number1 - number2;
				problemText = string.Format("{0} - {1} = ", number1, number2);
				break;
			default:
				break;
		}

		digitsInAnswer = countDigits(answer);

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
