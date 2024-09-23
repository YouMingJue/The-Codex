using UnityEngine;
using System.Collections.Generic;
using System;

public class Player : MonoBehaviour
{
    public static event Action<int> OnPlayerDeath; // The event will send the team number (0 for Team A, 1 for Team B).

    public int teamNumber; // 0 for Team A, 1 for Team B.

    void Die()
    {
        // This function is called when the player dies.
        OnPlayerDeath?.Invoke(teamNumber); // Trigger the event and pass the team number.
    }
}


public class WinningConditionManager : MonoBehaviour
{
    private Dictionary<int, int> teamLifeStock; // Key: team number, Value: life stock

    // Start is called before the first frame update
    void Start()
    {
        teamLifeStock = new Dictionary<int, int>
        {
            { 0, 3 }, // Team A starts with 3 lives
            { 1, 3 }  // Team B starts with 3 lives
        };

        // Subscribe to the player death event
        Player.OnPlayerDeath += HandlePlayerDeath;
    }

    void OnDestroy()
    {
        // Unsubscribe from the event to prevent memory leaks
        Player.OnPlayerDeath -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath(int teamNumber)
    {
        if (teamLifeStock.ContainsKey(teamNumber))
        {
            teamLifeStock[teamNumber]--;

            if (teamLifeStock[teamNumber] <= 0)
            {
                TeamLose(teamNumber);
            }
        }
    }

    private void TeamLose(int teamNumber)
    {
        if (teamNumber == 0)
        {
            Debug.Log("Team A has lost the game!");
            // Add logic for Team A losing
        }
        else if (teamNumber == 1)
        {
            Debug.Log("Team B has lost the game!");
            // Add logic for Team B losing
        }
    }
}

