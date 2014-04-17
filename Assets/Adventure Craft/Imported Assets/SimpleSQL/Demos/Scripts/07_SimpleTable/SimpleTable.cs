using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This script shows how to call a simple SQL query from a database using a simplified table structure
/// that allows you to store generic data. This is useful if you don't want to cast your results using 
/// a class-based ORM or a System.Data.DataTable.
/// 
/// In this example we overwrite the working database since no data is being changed. This is set in the 
/// SimpleSQLManager gameobject in the scene.
/// </summary>
public class SimpleTable : MonoBehaviour {

	// reference to our database manager object in the scene
	public SimpleSQL.SimpleSQLManager dbManager;
	
	// reference to the gui text object in our scene that will be used for output
	public GUIText outputText;
	
	void Start () 
	{
		// Gather a list of weapons and their type names pulled from the weapontype table		
		SimpleSQL.SimpleDataTable dt = dbManager.QueryGeneric(
														"SELECT " + 
															"W.WeaponID, " + 
															"W.WeaponName, " + 
															"W.Damage, " + 
															"W.Cost, " + 
															"W.Weight, " + 
															"W.WeaponTypeID, " + 
															"T.Description AS WeaponTypeDescription " + 
														"FROM " + 
															"Weapon W " + 
															"JOIN WeaponType T " + 
																"ON W.WeaponTypeID = T.WeaponTypeID " + 
														"ORDER BY " + 
															"W.WeaponID "
														);
		
		// output the list of weapons
		// note that we can reference the field/column by number (the order in the SELECT list, starting with zero) or by name
		outputText.text = "Weapons\n\n";
		int rowIndex = 0;
		foreach (SimpleSQL.SimpleDataRow dr in dt.rows)
		{
			outputText.text += "Name: '" + dr[1].ToString() + "' " + 
								"Damage:" + dr["Damage"].ToString() + " " + 
								"Cost:" + dr[3].ToString() + " " + 
								"Weight:" + dr["Weight"].ToString() + " " + 
								"Type:" + dr[6] + "\n";
						
			rowIndex++;
		}


        // get the weapon record that has a WeaponID > 4 with a single statement
		// warning, this will fail if no record exists, so we use a try catch block
		outputText.text += "\nFirst record where the WeaponID > 4: ";
		try
		{
        	outputText.text += dbManager.QueryGeneric("SELECT WeaponName FROM Weapon WHERE WeaponID > 4").rows[0][0].ToString() + "\n";
		}
		catch 
		{
            outputText.text += "No record found\n";
		}
	}
}
