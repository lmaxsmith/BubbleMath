using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

/* Problem manager class sits in the scene and creates new math problems, 
 * Running the player through the game session. 
 * 
 * Note 'Problem' support class (not monobehavior) at the bottom of this script.
 */

public class ProblemManager : MonoBehaviour
{
	public TextMeshProUGUI problemText;
	public TextMeshProUGUI feedBackText;
	public TMP_InputField answerField;

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
		if (answerField.text == "")
		{
			return;
		}

		else if (currentProblem.SubmitAnswer(int.Parse(answerField.text)))
		{
			feedBackText.text = "Good Job!";
			StartCoroutine(WaitForNextProblem());
		}
		else
		{
			feedBackText.text = "Try again";
			answerField.text = "";

			StartCoroutine(WaitForNextAttempt());
		}
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
		answerField.text = "";
		feedBackText.text = "";

	}

	//currently called by a button, but will be situational
	public void NextProblem()
	{
		currentProblem = new Problem(Problem.ProblemType.Plus, currentMaxNumber);
		problems.Add(currentProblem);
		problemsThisRound++;


		problemText.text = string.Format(@"({0}): {1}", problemsThisRound, currentProblem.ProblemText);
		answerField.text = "";
		feedBackText.text = "";

	}

	//closes the round and displays feedback
	public void EndRound()
	{
		answerField.text = "";

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
		answerField.text += number;
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

	//create a new math problem at creation of object
	public Problem(ProblemType problemType, int MaxNumber)
	{
		startTime = Time.realtimeSinceStartup;

		switch (problemType)
		{
			case ProblemType.Plus:
				answer = Random.Range(1, MaxNumber);
				number1 = Random.Range(0, answer);
				number2 = answer - number1;
				problemText = string.Format("{0} + {1} = ", number1, number2);
				break;
			case ProblemType.Minus:
				number1 = Random.Range(1, MaxNumber);
				number2 = Random.Range(0, number1);
				answer = number1 - number2;
				problemText = string.Format("{0} - {1} = ", number1, number2);
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
