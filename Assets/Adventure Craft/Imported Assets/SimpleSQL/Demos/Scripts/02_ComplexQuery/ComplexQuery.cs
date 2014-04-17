using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// This script shows how to pull relational data from a table and store it in our customized
/// classes. It also shows an alternative way of retrieving a full listing from a database
/// without using SQL. Parameters are used for efficiency and reusability.
/// 
/// In this example we overwrite the working database since no data is being changed. This is set in the 
/// SimpleSQLManager gameobject in the scene.
/// </summary>
public class ComplexQuery : MonoBehaviour {
	
	public SimpleSQL.SimpleSQLManager dbManager;
	public GUIText outputText;
	
	void Start () 
	{
		// alternate way of populating an entire table without using a SQL statement. This uses Linq.
		// You could also use "SELECT * FROM Location" with a Query function without Linq.
		List<Location> startingLocations = new List<Location> (from loc in dbManager.Table<Location> () select loc);

		// set up a sql query that we will reuse,
		// binding the parameter denoted by ? with the location id
		string sql = "SELECT " + 
						"CASE " + 
							"WHEN L.LocationID = M.LocationID1 THEN ML2.LocationName " + 
							"WHEN L.LocationID = M.LocationID2 THEN ML1.LocationName " +
							"END AS LocationName " +
						"FROM " + 
							"Location L " + 
							"JOIN LocationMapping M " + 
								"ON L.LocationID = M.LocationID1 " + 
								"OR L.LocationID = M.LocationID2 " + 
					 		"LEFT JOIN Location ML1 " +
						 		"ON M.LocationID1 = ML1.LocationID " +
							"LEFT JOIN Location ML2 " +
								"ON M.LocationID2 = ML2.LocationID " +
						"WHERE " + 
							"L.LocationID = ?";
		
		// loop through each starting location and gather the list of adjacent location based on our mapping table
		// using the premade query above.
		foreach (Location startingLocation in startingLocations)
		{
			startingLocation.AdjacentLocations = dbManager.Query<Location>(sql, startingLocation.LocationID);
		}
		
		// output our list of starting locations along with their corresponding adjacent locations
		outputText.text = "Map adjacent locations:\n\n";
		foreach (Location startingLocation in startingLocations)
		{
			outputText.text += startingLocation.LocationName + " is next to:  ";
			foreach (Location adjacentLocation in startingLocation.AdjacentLocations)
			{
				outputText.text += adjacentLocation.LocationName + ", ";
			}
			// trim off last comma
			outputText.text = outputText.text.Substring(0, outputText.text.Length - 2);
			outputText.text += "\n";
		}
	}
}
