using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft;
using Newtonsoft.Json;


public class DataHandler : MonoBehaviour
{
	public string saveFolder;

	public PlayerData player;
	public List<PlayerData> players; //make private
	public FileInfo[] playerFiles; //make private

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

		LoadData();
		SwitchUser(0); //TODO: Make interface for switching users.
    }

	/// <summary>
	/// Deserialize all the data from the cloud. 
	/// </summary>
	public void LoadData()
	{
		int attempt = 0;

		while (players.Count < 1)
		{

			try
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(saveFolder);
				playerFiles = directoryInfo.GetFiles("*.plr");

			}
			catch (Exception)
			{
				throw;
			}

			if (playerFiles.Length == 0 || playerFiles == null)
			{
				AddNewPlayer();
			}
			else
			{
				foreach (var playerFile in playerFiles)
				{
					players.Add(
						JsonConvert.DeserializeObject<PlayerData>(
					File.ReadAllText(
						playerFile.FullName)));

				}
			}

			if (attempt > 5)
			{
				Debug.LogError("Failed to load player data. ");
				return;
			}
			attempt++;
		}

	}

	/// <summary>
	/// Create new player, add to collection and save data.
	/// </summary>
	public void AddNewPlayer()
	{
		Debug.Log("Adding Player " + players.Count);
		PlayerData playerData = new PlayerData(players.Count);
		players.Add(playerData);
		player.maxNumber = maxNumberDefault;
		SaveData();
	}
	
	/// <summary>
	/// Add new empty round data to the player's round collection.
	/// </summary>
	/// <returns>New empty round. (already in collection)</returns>
	public RoundData AddRound()
	{
		RoundData round = new RoundData();
		player.rounds.Add(round);
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
			Debug.Log(string.Format(@"Saving {0}/Player {1}.plr", saveFolder, i));
			File.WriteAllText(
				Path.Combine(saveFolder, string.Format(@"Player {0}.plr", i)), 
				JsonConvert.SerializeObject(player, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include }));

		}
		LoadData();//why do I call load right after save?
	}

	/// <summary>
	/// Sets a user to current. Including from null to selection. 
	/// </summary>
	/// <param name="userIndex"></param>
	public void SwitchUser(int userIndex)//TODO: Setup interface to go between multiple users
	{
		player = players[userIndex];
		Debug.Log(string.Format("Loaded player {0} {1}", userIndex, player.playerName));
	}
}



[Serializable] public class PlayerData
{
	[SerializeField] public string playerName;
	[SerializeField] public List<RoundData> rounds;
	[SerializeField] public int maxNumber = 10;
	[SerializeField] public float avgTimeAtCurrentMax;

	public PlayerData()
	{
		rounds = new List<RoundData>();
	}
	public PlayerData(int playerNumber)
	{
		rounds = new List<RoundData>();
		playerName = "Player " + playerNumber;
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
		Plus, Minus
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

	//create a new math problem at creation of object
	public ProblemData(ProblemType problemType, int MaxNumber)
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

