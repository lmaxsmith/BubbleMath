using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft;
using Newtonsoft.Json;


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
		RoundData round = new RoundData(Player.maxNumber);
		Player.rounds.Add(round);
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

