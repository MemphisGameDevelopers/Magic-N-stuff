using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

/// <summary>
/// This script shows how to use the Update command with a class definition and also
/// through SQL statements.
/// 
/// In this example we will not overwrite the working database since we are updating the data. If
/// we were to overwrite, then changes would be lost each time the scene is run again.
/// </summary>
public class UpdateCommand : MonoBehaviour {

	// The list of player stats from the database
	private List<PlayerStats> _playerStatsList;
	
	// These variables will be used to store data from the GUI interface
	private string _newPlayerName;
	private string _newPlayerTotalKills;
	private string _newPlayerPoints;
	
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
		GUILayout.Label("Player Name:", GUILayout.Width(100.0f));
		_newPlayerName = GUILayout.TextField(_newPlayerName, GUILayout.Width(200.0f));
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Total Kills:", GUILayout.Width(100.0f));
		_newPlayerTotalKills = GUILayout.TextField(_newPlayerTotalKills, GUILayout.Width(200.0f));
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Points:", GUILayout.Width(100.0f));
		_newPlayerPoints = GUILayout.TextField(_newPlayerPoints, GUILayout.Width(200.0f));
		GUILayout.EndHorizontal();
		
		int totalKills;
		int points;
		
		if (!int.TryParse(_newPlayerTotalKills, out totalKills))
			totalKills = 0;
		
		if (!int.TryParse(_newPlayerPoints, out points))
			points = 0;
		
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Update First Record", GUILayout.Width(200.0f)))
		{
			UpdatePlayerStats_Simple(_playerID, _newPlayerName, totalKills, points);
			ResetGUI();
		}
		GUILayout.Label("OR", GUILayout.Width(30.0f));
		if (GUILayout.Button("Update First Record Query", GUILayout.Width(250.0f)))
		{
			UpdatePlayerStats_Query(_playerID, _newPlayerName, totalKills, points);
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
		// Reset the temporary GUI variables
		_newPlayerName = "";
		_newPlayerTotalKills = "";
		_newPlayerPoints = "";
		
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
	/// Updates the player stats table using the class definition. No need for SQL here.
	/// </summary>
	/// <param name='playerID'>
	/// The ID of the player to update
	/// </param>
	/// <param name='playerName'>
	/// Player name.
	/// </param>
	/// <param name='totalKills'>
	/// Total kills.
	/// </param>
	/// <param name='points'>
	/// Points.
	/// </param>
	private void UpdatePlayerStats_Simple(int playerID, string playerName, int totalKills, int points)
	{
		// Set up a player stats class, setting all values including the playerID
		PlayerStats playerStats = new PlayerStats { PlayerID = playerID, PlayerName = playerName, TotalKills = totalKills, Points = points };
		
		// the database manager will update all the fields except the primary key which it uses to look up the data
		dbManager.UpdateTable(playerStats);
	}
	
	/// <summary>
	/// Updates the player stats by executing a SQL statement. Note that no data is returned, this only modifies the table
	/// </summary>
	/// <param name='playerID'>
	/// The ID of the player to update
	/// </param>
	/// <param name='playerName'>
	/// Player name.
	/// </param>
	/// <param name='totalKills'>
	/// Total kills.
	/// </param>
	/// <param name='points'>
	/// Points.
	/// </param>
	private void UpdatePlayerStats_Query(int playerID, string playerName, int totalKills, int points)
	{
		// Call our SQL statement using ? to bind our variables
		dbManager.Execute("UPDATE PlayerStats SET PlayerName = ?, TotalKills = ?, Points = ? WHERE PlayerID = ?", playerName, totalKills, points, playerID);
	}
}
