using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

/// <summary>
/// This script shows how to use the Delete command with a class definition and also
/// through SQL statements.
/// 
/// In this example we will not overwrite the working database since we are updating the data. If
/// we were to overwrite, then changes would be lost each time the scene is run again.
/// </summary>
public class DeleteCommand : MonoBehaviour {

	// The list of player stats from the database
	private List<PlayerStats> _playerStatsList;
	
	// Player ID key field pulled from the first record in the table
	private int _playerID;
	
	// reference to our db manager object
	public SimpleSQL.SimpleSQLManager dbManager;
	
	// reference to our output text object
	public GUIText outputText;
	
	void Start()
	{
		// clear out the output text since we are using GUI in this example
		outputText.text = "";

		// reset the GUI and reload
		ResetGUI();
	}
	
	void OnGUI()
	{
		GUILayout.BeginVertical();
		
		GUILayout.Space(10.0f);
		
		GUILayout.BeginHorizontal();
		
		GUILayout.Space(10.0f);
		
		GUILayout.BeginVertical();
		
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Delete First Record", GUILayout.Width(200.0f)))
		{
			DeletePlayerStats_Simple(_playerID);
			ResetGUI();
		}
		GUILayout.Label("OR", GUILayout.Width(30.0f));
		if (GUILayout.Button("Delete First Record Query", GUILayout.Width(250.0f)))
		{
			DeletePlayerStats_Query(_playerID);
			ResetGUI();
		}
		GUILayout.EndHorizontal();
		
		GUILayout.Space(20.0f);

		GUILayout.BeginHorizontal();
		GUILayout.Label("Player", GUILayout.Width(200.0f));
		GUILayout.Label("Total Kills", GUILayout.Width(150.0f));
		GUILayout.Label("Points", GUILayout.Width(150.0f));
		GUILayout.EndHorizontal();
		
		GUILayout.Label("-----------------------------------------------------------------------------------------------------------------------------------------");
		
		foreach (PlayerStats playerStats in _playerStatsList)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(playerStats.PlayerName, GUILayout.Width(200.0f));
			GUILayout.Label(playerStats.TotalKills.ToString(), GUILayout.Width(150.0f));
			GUILayout.Label(playerStats.Points.ToString(), GUILayout.Width(150.0f));
			GUILayout.EndHorizontal();
		}
		
		GUILayout.EndVertical();
		
		GUILayout.EndHorizontal();
		
		GUILayout.EndVertical();
	}
	
	private void ResetGUI()
	{
		// Loads the player stats from the database using Linq
		_playerStatsList = new List<PlayerStats> (from ps in dbManager.Table<PlayerStats> () select ps);

		_playerID = -1;
		if (_playerStatsList != null)
		{
			if (_playerStatsList.Count > 0)
			{
				_playerID = _playerStatsList[0].PlayerID;
			}
		}
	}
	
	/// <summary>
	/// Deletes the first player stat in the table using the class definition. No need for SQL here.
	/// </summary>
	/// <param name='playerID'>
	/// The ID of the player to update
	/// </param>
	private void DeletePlayerStats_Simple(int playerID)
	{
		// Set up a player stats class, setting the key field
		PlayerStats playerStats = new PlayerStats { PlayerID = playerID };
		
		dbManager.Delete<PlayerStats>(playerStats);
	}
	
	/// <summary>
	/// Deletes the first player stat by executing a SQL statement. Note that no data is returned, this only modifies the table
	/// </summary>
	/// <param name='playerID'>
	/// The ID of the player to update
	/// </param>
	private void DeletePlayerStats_Query(int playerID)
	{
		// Call our SQL statement using ? to bind our variables
		dbManager.Execute("DELETE FROM PlayerStats WHERE PlayerID = ?", playerID);
	}
}
