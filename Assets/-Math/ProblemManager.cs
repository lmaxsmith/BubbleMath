using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

/* Problem manager class sits in the scene and creates new math problems, 
 * Running the player through the game session. 
 * 
 * Note 'Problem' support class (not monobehavior) at the bottom of this script.
 */

public class ProblemManager : MonoBehaviour
{
	public GameObject RestartButton;
	public GameObject bubblePrefab;
	public GameObject playSpace;
	public LayerMask bubbleLayer;

	public AnswerDigit answerDigitOnes;
	public AnswerDigit answerDigitTens;
	public AnswerDigit answerDigitHundreds;
	public List< AnswerDigit> answerDigits;
	AnswerDigit currentDigit;

	//temporary public 
	public Bubble currentBubble;
	public ProblemData currentProblem;
	public DataHandler dataHandler;
	public RoundData currentRound;

	const int attemptsPerProblem = 3;
	public int problemsPerRound = 10;
	int problemsThisRound = 0;

	#region Unity Methods
	//setup relationships
	private void Awake()
	{
		dataHandler = FindObjectOfType<DataHandler>();
	}

	// Start is called before the first frame update
	void Start()
    {
		answerDigits = new List<AnswerDigit>{ answerDigitHundreds, answerDigitTens, answerDigitOnes };
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
			currentBubble.AnswerRight();
			StartCoroutine(WaitForNextProblem());
		}
		else
		{
			currentBubble.TryAgain();

			StartCoroutine(WaitForNextAttempt());
		}

		currentDigit = null;
	}

	IEnumerator WaitForNextAttempt()
	{
		yield return new WaitForSeconds(2);

		if (currentProblem.TriesToComplete <= attemptsPerProblem)
		{
			NextAttempt();
		}
		else if (currentRound.problems.Count < problemsPerRound)
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

		if ( currentRound.problems.Count < problemsPerRound)
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
		currentBubble.feedBackText.text = "";
	}

	//currently called by a button, but will be situational
	public void NextProblem()
	{
		int problemTypeInt = GetProblemTypeByMaxNumber(dataHandler.Player.maxNumber);
		Debug.Log("Problem Type: " + (ProblemData.ProblemType)problemTypeInt);
		currentProblem = new ProblemData(
			(ProblemData.ProblemType)problemTypeInt, 
			dataHandler.Player.maxNumber / (problemTypeInt + 1));
		Debug.Log(currentProblem.ProblemText);
		currentRound.problems.Add(currentProblem);

		//replace the bubble
		if (currentBubble != null)
		{
			Destroy(currentBubble.gameObject);
		}
		currentBubble = Instantiate(bubblePrefab).GetComponent<Bubble>();
		currentBubble.transform.position = RandomInPlayspace(playSpace);

		currentBubble.problemText.text = currentProblem.ProblemText;

		SetupDigits();
	}

	//as the max number increases, add more math types
	int GetProblemTypeByMaxNumber(int maxNumber)
	{
		int maxProblemType = 1;
		if (maxNumber >= 20)
			maxProblemType++;
		if (maxNumber >= 40)
			maxProblemType++;
		if (maxNumber >= 80)
			maxProblemType++;
		return Mathf.FloorToInt(UnityEngine.Random.value * maxProblemType);
	}

	//picks a random spot within the playspace box
	Vector3 RandomInPlayspace(GameObject playspace)
	{
		float x = (UnityEngine.Random.value - .5f) * playspace.transform.localScale.x;
		float y = (UnityEngine.Random.value - .5f) * playspace.transform.localScale.y;
		float z = (UnityEngine.Random.value - .5f) * playspace.transform.localScale.z;

		return new Vector3(x, y, z) + playspace.transform.position;

		
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

	//closes the round, calculates results, and displays feedback
	public void EndRound()
	{
		//clear display
		ClearDigits();
		RestartButton.SetActive(true);

		//calculate info based on round played
		currentRound.CalculateRoundInfo();
		if (currentRound.averageTime < 15)
		{
			dataHandler.Player.maxNumber++;
			
		}
		else if (currentRound.averageTime > 20)
		{
			dataHandler.Player.maxNumber--;
		}

		//display feedback
		currentBubble.feedBackText.text = string.Format(@"Great job! You answered {0} math problems in {1} seconds! That's {2} seconds per math problem!", 
			currentRound.correctProblemCount, 
			Mathf.Round(currentRound.totalTime), 
			Mathf.Round(currentRound.averageTime));

		dataHandler.SaveData();

		RestartButton.SetActive(true);
	}

	public void NewRound()
	{
		Debug.Log("New Round");
		RestartButton.SetActive(false);
		currentRound = dataHandler.AddRound();
		NextProblem();
	}

	//adds number to the answer place according to digit's turn when pressing number buttons
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




