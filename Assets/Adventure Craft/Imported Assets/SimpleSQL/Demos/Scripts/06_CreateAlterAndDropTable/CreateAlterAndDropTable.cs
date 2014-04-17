using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This script demonstrates how to create a table programmatically at runtime. You can create
/// a table directly from a class structure, or by calling a SQL statement.
/// 
/// In this example we will not overwrite the working database since we are updating the data. If
/// we were to overwrite, then changes would be lost each time the scene is run again.
/// </summary>
public class CreateAlterAndDropTable : MonoBehaviour {
	
	// flag to tell our GUI whether to show the results
	private bool _ranCommand = false;
	
	// reference to the database manager in the scene
	public SimpleSQL.SimpleSQLManager dbManager;
	
	// reference to the output text in the scene
	public GUIText outputText;
	
	void Start () 
	{
		outputText.text = "";
	}
	
	void OnGUI()
	{
		GUILayout.BeginVertical();
		
		GUILayout.BeginHorizontal();
		if (GUILayout.Button ("Create Table", GUILayout.Width(150.0f)))
		{
			_ranCommand = true;
			CreateTable();
		}
		GUILayout.Label("OR", GUILayout.Width(30.0f));
		if (GUILayout.Button ("Create Table", GUILayout.Width(150.0f)))
		{
			_ranCommand = true;
			CreateTable();
		}
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		if (GUILayout.Button ("Add Column", GUILayout.Width(150.0f)))
		{
			_ranCommand = true;
			AddColumn();
		}
		if (GUILayout.Button ("Drop Column", GUILayout.Width(150.0f)))
		{
			_ranCommand = true;
			DropColumn();
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		if (GUILayout.Button ("Drop Table", GUILayout.Width(150.0f)))
		{
			_ranCommand = true;
			DropTable();
		}
		GUILayout.EndHorizontal();

		
		if (_ranCommand)
		{
			GUILayout.Label("Success!");
		}
		
		GUILayout.EndVertical();
	}
	
	/// <summary>
	/// Creates the table using the class definition
	/// </summary>
	private void CreateTable()
	{
		// Check out the StarShip class to see the various attributes
		// and how they can be used to set up your table.
		dbManager.CreateTable<StarShip>();
	}
	
	/// <summary>
	/// Creates the table using a SQL statement
	/// </summary>
	private void CreateTable_Query()
	{
		string sql;
		
		// Start a transaction to batch the commands together
		dbManager.BeginTransaction();
		
		// Create the table
		sql = "CREATE TABLE \"StarShip\" " + 
				"(\"StarShipID\" INTEGER PRIMARY KEY  NOT NULL, " + 
				"\"StarShipName\" varchar(60) NOT NULL, " + 
				"\"HomePlanet\" varchar(100) DEFAULT Earth, " +
				"\"Range\" FLOAT NOT NULL, " + 
				"\"Armor\" FLOAT DEFAULT 120, " + 
				"\"Firepower\" FLOAT)";
		dbManager.Execute(sql);
		
		// Create an index on the starship name
		sql = "CREATE INDEX \"StarShip_StarShipName\" on \"StarShip\"(\"StarShipName\")";
		dbManager.Execute(sql);
		
		// Commit the transaction and run all the commands
		dbManager.Commit();
	}
	
	/// <summary>
	/// Adds a column to a data table
	/// </summary>
	private void AddColumn()
	{
		string sql;

		sql = "ALTER TABLE \"StarShip\" ADD COLUMN \"NewField\" INTEGER";
		dbManager.Execute(sql);
	}
	
	/// <summary>
	/// Drops a column from a data table. Note that there is no simple way to drop a column
	/// from a table, so we first backup our current table, create a new table with the
	/// new structure, copy the data from the backup into the new table, then drop
	/// the backup.
	/// 
	/// This method also works for renaming a column or changing its type
	/// </summary>
	private void DropColumn()
	{
		string sql;
		
		// start a transaction to speed up processing
		dbManager.BeginTransaction();
		
		// rename our table to a backup name
		sql = "ALTER TABLE \"StarShip\" RENAME TO \"Temp_StarShip\"";
		dbManager.Execute(sql);
		
		// create a new table with our desired structure, leaving out the dropped column(s)
		sql = "CREATE TABLE \"StarShip\" " + 
					"(\"StarShipID\" integer PRIMARY KEY  NOT NULL , " + 
					"\"StarShipName\" varchar(60) NOT NULL , " + 
					"\"HomePlanet\" varchar(100) DEFAULT Earth , " + 
					"\"Range\" float NOT NULL , " + 
					"\"Armor\" float DEFAULT 120 , " + 
					"\"Firepower\" float) ";
		dbManager.Execute (sql);

		// copy the data from the backup table to our new table
		sql = "INSERT INTO \"StarShip\" " + 
					"SELECT " + "" +
					"\"StarShipID\", " + 
					"\"StarShipName\", " + 
					"\"HomePlanet\", " + 
					"\"Range\", " + 
					"\"Armor\", " + 
					"\"Firepower\" " +
					"FROM \"Temp_StarShip\"";
		dbManager.Execute(sql);
		
		// drop the backup table
		sql = "DROP TABLE \"Temp_StarShip\"";
		dbManager.Execute (sql);
		
		// commit the transaction and run all the commands
		dbManager.Commit();
	}
	
	/// <summary>
	/// Removes the table from the database
	/// </summary>
	private void DropTable()
	{
		string sql;
		
		// Drop the table
		sql = "DROP TABLE \"StarShip\"";
		dbManager.Execute(sql);
	}
}
