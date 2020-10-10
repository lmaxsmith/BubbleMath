using System;
using System.Collections.Generic;
using UnityEngine;

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