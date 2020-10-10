using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft;
using Newtonsoft.Json;
using Random = System.Random;


public class DataHandler : MonoBehaviour
{
	public string saveFolder;

	public int currentPlayerIndex;
	public List<PlayerData> players; //make private
	public FileInfo[] playerFiles; //make private
	private PlayerData player;

	public PlayerData Player => players[currentPlayerIndex];

	const int maxNumberDefault = 10;

    // Start is called before the first frame update
    void Start()
    {
		players = new List<PlayerData>();
		if (saveFolder == null || saveFolder == "")
		{
			saveFolder = Application.persistentDataPath;
		}
		Debug.Log("Save folder: " + saveFolder);

		players = LoadPlayerData();
		SaveData();
		SwitchPlayer(0); //TODO: Make interface for switching users.
    }

	/// <summary>
	/// Deserialize all the data from the cloud. 
	/// </summary>
	public List<PlayerData> LoadPlayerData()
	{
		int attempt = 0;
		List<PlayerData> playerDatas = new List<PlayerData>();
		
		while (playerDatas.Count < 1)
		{
			try
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(saveFolder);
				playerFiles = directoryInfo.GetFiles("Player*");

			}
			catch (Exception)
			{
				throw;
			}

			if (playerFiles.Length == 0 || playerFiles == null)
			{
				playerDatas.Add(new PlayerData(playerDatas.Count, maxNumberDefault));
			}
			else
			{
				foreach (var playerFile in playerFiles)
				{
					playerDatas.Add(
						JsonConvert.DeserializeObject<PlayerData>(
					File.ReadAllText(
						playerFile.FullName)));

				}
			}

			if (attempt > 5)
			{
				Debug.LogError("Failed to load player data. ");
				return null;
			}
			attempt++;
		}

		return playerDatas;
	}

	
	/// <summary>
	/// Add new empty round data to the player's round collection.
	/// </summary>
	/// <returns>New empty round. (already in collection)</returns>
	public RoundData AddRound()
	{
		RoundData round = new RoundData();
		players[currentPlayerIndex].rounds.Add(round);
		return round;
	}

	/// <summary>
	/// serialize player data including all rounds to filesystem. 
	/// </summary>
	public void SaveData()
	{
		//save players
		for (int i = 0; i < players.Count; i++)
		{
			Debug.Log(string.Format(@"Saving {0}/Player {1}.json", saveFolder, i));
			File.WriteAllText(
				Path.Combine(saveFolder, string.Format(@"Player {0}.json", i)), 
				JsonConvert.SerializeObject(players[i]));

		}
	}

	/// <summary>
	/// Sets a user to current. Including from null to selection. 
	/// </summary>
	/// <param name="userIndex"></param>
	public void SwitchPlayer(int userIndex)//TODO: Setup interface to go between multiple users
	{
		currentPlayerIndex = userIndex;
		Debug.Log(string.Format("Loaded player {0} {1}", userIndex, players[currentPlayerIndex].playerName));
	}
}



[Serializable] public class PlayerData
{
	[SerializeField] public string playerName;
	[SerializeField] public List<RoundData> rounds;
	[SerializeField] public int maxNumber;
	[SerializeField] public float avgTimeAtCurrentMax;

	public PlayerData()
	{
		rounds = new List<RoundData>();
	}
	public PlayerData(int playerNumber, int maxNumber)
	{
		rounds = new List<RoundData>();
		playerName = "Player " + playerNumber;
		this.maxNumber = maxNumber;
	}
}

//All the info for a round including all the problems
[Serializable] public class RoundData
{
	[SerializeField] public List<ProblemData> problems;
	[SerializeField] public DateTime dateTime;

	[SerializeField] public float totalTime;
	[SerializeField] public float averageTime;
	[SerializeField] public int correctProblemCount;

	public RoundData()
	{
		dateTime = DateTime.Now;
		problems = new List<ProblemData>();
	}

	public void CalculateRoundInfo()
	{
		IEnumerable<ProblemData> correctProblems =
	from problem in problems
	where problem.Passed
	select problem;
		correctProblemCount = correctProblems.Count<ProblemData>();

		totalTime = 0;
		foreach (ProblemData problem in problems)
		{
			totalTime += problem.ExecutionTime;
		}

		averageTime = totalTime / correctProblemCount;
	}
}


//OOP style. Containing one math problem. 
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

